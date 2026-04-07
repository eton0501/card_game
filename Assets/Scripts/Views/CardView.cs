using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 負責手牌卡片的互動，負責懸停放大、拖曳出牌、手動指定目標與釋放判定。
/// </summary>


public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text mana;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private LayerMask dropLayer;

    public Card Card { get; private set; }

    private HandView ownerHand;
    private Vector3 dragStartPosition;
    private Quaternion dragStartRotation;
    private SortingGroup sortingGroup;

    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
        if (sortingGroup == null)
        {
            sortingGroup = gameObject.AddComponent<SortingGroup>();
        }
    }

    public void Setup(Card card)
    {
        Card = card;
        title.text = card.Title;
        description.text = card.Description;
        mana.text = card.Mana.ToString();
        imageSR.sprite = card.Image;
    }

    public void SetOwnerHand(HandView handView)
    {
        ownerHand = handView;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        if (sortingGroup == null)
        {
            sortingGroup = GetComponent<SortingGroup>();
            if (sortingGroup == null) sortingGroup = gameObject.AddComponent<SortingGroup>();
        }
        sortingGroup.sortingOrder = sortingOrder;
    }

    private void OnMouseEnter()
    {
        if (!Interactions.Instance.PlayerCanHover()) return;
        ownerHand?.OnCardHoverEnter(this);
    }

    private void OnMouseExit()
    {
        if (!Interactions.Instance.PlayerCanHover()) return;
        ownerHand?.OnCardHoverExit(this);
    }

    private void OnMouseDown()
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;

        if (Card.ManualTargetEffect != null)
        {
            ownerHand?.ClearHover();
            ManualTargetSystem.Instance.StartTargeting(transform.position);
            return;
        }

        Interactions.Instance.PlayerIsDragging = true;
        ownerHand?.OnCardDragStart(this);

        wrapper.SetActive(true);
        transform.localScale = Vector3.one;

        dragStartPosition = transform.position;
        dragStartRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
    }

    private void OnMouseDrag()
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;
        if (Card.ManualTargetEffect != null) return;

        transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
    }

    private void OnMouseUp()
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;

        if (Card.ManualTargetEffect != null)
        {
            EnemyView target = ManualTargetSystem.Instance.EndTargeting(
                MouseUtil.GetMousePositionInWorldSpace(-1)
            );
            if (target != null && ManaSystem.Instance.HasEnoughMana(Card.Mana))
            {
                PlayCardGA playCardGA = new(Card, target);
                ActionSystem.Instance.Perform(playCardGA);
            }
            return;
        }

        bool canPlay =
            ManaSystem.Instance.HasEnoughMana(Card.Mana) &&
            Physics.Raycast(transform.position, Vector3.forward, out _, 10f, dropLayer);

        if (canPlay)
        {
            PlayCardGA playCardGA = new(Card);
            ActionSystem.Instance.Perform(playCardGA);
        }
        else
        {
            transform.position = dragStartPosition;
            transform.rotation = dragStartRotation;
            transform.localScale = Vector3.one;
        }

        Interactions.Instance.PlayerIsDragging = false;
        ownerHand?.OnCardDragEnd(this);
    }
}

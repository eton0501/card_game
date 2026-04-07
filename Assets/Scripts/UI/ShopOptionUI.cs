using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 顯示商店單張卡片選項、價格，並處理購買點擊。
/// </summary>


public class ShopOptionUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Button button;
    [SerializeField] private CardStaticView cardStaticView;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private GameObject soldOverlay;
    [SerializeField] private string soldText = "SOLD";

    public CardData CardData { get; private set; }
    public int Price { get; private set; }
    public bool IsSold { get; private set; }

    private Action<ShopOptionUI> onClicked;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
            if (button == null)
            {
                button = GetComponentInChildren<Button>(true);
            }
        }

        if (cardStaticView == null)
        {
            cardStaticView = GetComponentInChildren<CardStaticView>(true);
        }
    }

    public void Setup(CardData cardData, int price, Action<ShopOptionUI> onClick)
    {
        CardData = cardData;
        Price = Mathf.Max(0, price);
        IsSold = false;
        onClicked = onClick;

        if (cardStaticView != null)
        {
            cardStaticView.Setup(cardData);
        }

        if (priceText != null)
        {
            priceText.text = cardData == null ? string.Empty : $"{Price}g";
        }

        if (soldOverlay != null)
        {
            soldOverlay.SetActive(false);
            TMP_Text soldOverlayText = soldOverlay.GetComponentInChildren<TMP_Text>();
            if (soldOverlayText != null)
            {
                soldOverlayText.text = soldText;
            }
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = cardData != null;
            if (cardData != null)
            {
                button.onClick.AddListener(TryClick);
            }
        }
        else
        {
        }
    }

    public void MarkSold()
    {
        IsSold = true;
        if (button != null)
        {
            button.interactable = false;
        }
        if (soldOverlay != null)
        {
            soldOverlay.SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        TryClick();
    }

    private void TryClick()
    {
        if (CardData == null) return;
        if (IsSold) return;
        onClicked?.Invoke(this);
    }
}

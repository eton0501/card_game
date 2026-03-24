using System;
using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text mana;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private LayerMask dropLayer;
    public Card Card{get;private set;}
    private Vector3 dragStartPosition;
    private Quaternion dragStartRotation;
    public void Setup(Card card)//初始化卡牌方法，接受card物件來設定視覺呈現
    {
        Card=card;//把傳入的卡牌資料存到Card
        title.text=card.Title;//更換UI上的title文字
        description.text=card.Description;
        mana.text=card.Mana.ToString();
        imageSR.sprite=card.Image;
    }
    void OnMouseEnter()//當使用者的滑鼠進入這張卡牌的Box Collider
    {
        if(!Interactions.Instance.PlayerCanHover())return;
        wrapper.SetActive(false);//將這張卡牌的視覺全部隱藏，這樣手排區的這張牌就不見了
        Vector3 pos=new(transform.position.x,-2,0);//計算放大卡牌要顯示的位置
        CardViewHoverSystem.Instance.Show(Card,pos);//將這張卡牌的資料和剛剛算好的位置傳到放大卡牌的系統
    }
    void OnMouseExit()//當使用者的滑鼠離開Box Collider
    {
        if(!Interactions.Instance.PlayerCanHover())return;
        CardViewHoverSystem.Instance.Hide();//通知放大卡牌系統將放大的卡牌隱藏
        wrapper.SetActive(true);//把手排區的這張牌顯示
    }
    void OnMouseDown()
    {
        if(!Interactions.Instance.PlayerCanInteract())return;
        Interactions.Instance.PlayerIsDragging=true;
        wrapper.SetActive(true);
        CardViewHoverSystem.Instance.Hide();
        dragStartPosition=transform.position;
        dragStartRotation=transform.rotation;
        transform.rotation=Quaternion.Euler(0,0,0);
        transform.position=MouseUtil.GetMousePositionInWorldSpace(-1);
    }
    void OnMouseDrag()
    {
        if(!Interactions.Instance.PlayerCanInteract())return;
        transform.position=MouseUtil.GetMousePositionInWorldSpace(-1);
    }
    void OnMouseUp()
    {
        if(!Interactions.Instance.PlayerCanInteract())return;
        if(ManaSystem.Instance.HasEnoughMana(Card.Mana)&&Physics.Raycast(transform.position,Vector3.forward,out RaycastHit hit, 10f,dropLayer))
        {
            PlayCardGA playCardGA=new(Card);
            ActionSystem.Instance.Perform(playCardGA);
        }
        else
        {
            transform.position=dragStartPosition;
            transform.rotation=dragStartRotation;
        }
        Interactions.Instance.PlayerIsDragging=false;
    }
}

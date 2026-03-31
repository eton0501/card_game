using System;
using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;//卡牌名稱的UI
    [SerializeField] private TMP_Text description;//卡牌描述的UI
    [SerializeField] private TMP_Text mana;//法力消耗的UI
    [SerializeField] private SpriteRenderer imageSR;//卡牌圖片
    [SerializeField] private GameObject wrapper;//卡牌所有視覺元素的父物件
    [SerializeField] private LayerMask dropLayer;//判斷卡牌是否有拖移到放置區域的Layer遮罩
    public Card Card{get;private set;}//此腳本對應的卡牌資料，僅允許外部讀取
    private Vector3 dragStartPosition;//紀錄開始拖移前卡牌的原始位置
    private Quaternion dragStartRotation;//紀錄開始拖移前卡牌的原始旋轉
    public void Setup(Card card)//初始化卡牌方法，接受card物件來設定視覺呈現
    {
        Card=card;//把傳入的卡牌資料存到Card
        title.text=card.Title;//更換UI上的title文字
        description.text=card.Description;//設定卡牌的描述
        mana.text=card.Mana.ToString();//設定卡牌的法力消耗
        imageSR.sprite=card.Image;//設定卡牌圖片
    }
    void OnMouseEnter()//當使用者的滑鼠進入這張卡牌的Box Collider
    {
        if(!Interactions.Instance.PlayerCanHover())return;//如果目前的狀態不允許懸停互動(例如正在移其他卡牌)，直接返回
        wrapper.SetActive(false);//將這張卡牌的視覺全部隱藏，這樣手排區的這張牌就不見了
        Vector3 pos=new(transform.position.x,-2,0);//計算放大卡牌要顯示的位置
        CardViewHoverSystem.Instance.Show(Card,pos);//將這張卡牌的資料和剛剛算好的位置傳到放大卡牌的系統
    }
    void OnMouseExit()//當使用者的滑鼠離開Box Collider
    {
        if(!Interactions.Instance.PlayerCanHover())return;//如果目前的狀態不允許懸停互動(例如正在移其他卡牌)，直接返回
        CardViewHoverSystem.Instance.Hide();//通知放大卡牌系統將放大的卡牌隱藏
        wrapper.SetActive(true);//把手排區的這張牌顯示
    }
    void OnMouseDown()//當玩家按下滑鼠左鍵點擊卡牌時
    {
        if(!Interactions.Instance.PlayerCanInteract())return;//如果目前狀態不允許互動(例如敵人回合)，直接返回
        if (Card.ManualTargetEffect != null)//如果此卡牌需要手動選擇目標(例如單體攻擊)
        {
            ManualTargetSystem.Instance.StartTargeting(transform.position);//以卡牌目前位置為起點，啟動目標選擇系統(顯示箭頭)
        }
        else//如果此卡牌不須手動選擇目標(例如全體攻擊)
        {
            Interactions.Instance.PlayerIsDragging=true;//標記玩家正在拖移卡牌
            wrapper.SetActive(true);//讓卡牌可以一直看到正在拖移的卡牌
            CardViewHoverSystem.Instance.Hide();//放大預覽關掉
            dragStartPosition=transform.position;//儲存拖移的起點
            dragStartRotation=transform.rotation;//儲存拖移的原始旋轉
            transform.rotation=Quaternion.Euler(0,0,0);//拖移時卡牌旋轉歸零
            transform.position=MouseUtil.GetMousePositionInWorldSpace(-1);//將卡牌位置移到滑鼠游標處(Z=-1確保在背景前方)
        }
        
    }
    void OnMouseDrag()//當玩家持續按住滑鼠並拖動時每幀觸發
    {
        if(!Interactions.Instance.PlayerCanInteract())return;//若目前狀態不允許互動，直接返回
        if(Card.ManualTargetEffect!=null) return;//如果此卡牌為手動指定目標，則直接返回(交給ManualTargetSystem處理)
        transform.position=MouseUtil.GetMousePositionInWorldSpace(-1);//將卡牌跟隨滑鼠移動
    }
    void OnMouseUp()//當玩家放開滑鼠左鍵時
    {
        if(!Interactions.Instance.PlayerCanInteract())return;//如果目前狀態不允許互動，直接返回
        if (Card.ManualTargetEffect != null)//如果為手動指定目標卡牌
        {
            EnemyView target=ManualTargetSystem.Instance.EndTargeting(MouseUtil.GetMousePositionInWorldSpace(-1));//結束目標選擇，取得玩家最終指向的敵人
            if (target != null && ManaSystem.Instance.HasEnoughMana(Card.Mana))//如果有選中敵人且法力足夠，則執行出牌行動
            {
                PlayCardGA playCardGA=new(Card,target);//建立帶有指定目標的出牌行動
                ActionSystem.Instance.Perform(playCardGA);//交給行動系統執行
            }
        }
        else//如果不須指定目標的卡牌
        {
            if(ManaSystem.Instance.HasEnoughMana(Card.Mana)&&Physics.Raycast(transform.position,Vector3.forward,out RaycastHit hit, 10f,dropLayer))//判斷法力是否足夠，且卡牌是否移到有效的放置區域(從卡牌位置向前發射射線，檢測是否擊中指定Layer的區域)
            {
                PlayCardGA playCardGA=new(Card);//建立出牌行動
                ActionSystem.Instance.Perform(playCardGA);//交給行動系統執行
            }
            else//如果法力不足或是未放到有效區域
            {
                transform.position=dragStartPosition;//將卡牌回歸原始位置
                transform.rotation=dragStartRotation;//將卡牌回歸原始旋轉
            }
            Interactions.Instance.PlayerIsDragging=false;//清除拖移狀態
        }
    }
}

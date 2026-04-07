using UnityEngine;

/// <summary>
/// 管理放大預覽卡顯示。
/// </summary>


public class CardViewHoverSystem : Singleton<CardViewHoverSystem>
{
    [SerializeField] private CardView cardViewHover;//專門用來顯示放大預覽的CardView物件
    public void Show(Card card,Vector3 position)//在指定位置顯示放大的卡牌預覽
    {
        cardViewHover.gameObject.SetActive(true);//啟用預覽物件
        cardViewHover.Setup(card);//將卡牌資料填入預覽，更新名稱、描述等等資料
        cardViewHover.transform.position=position;//將預覽移動到指定位置
    }
    public void Hide()//隱藏放大的卡牌預覽
    {
        cardViewHover.gameObject.SetActive(false);//停用預覽物件
    }
}    

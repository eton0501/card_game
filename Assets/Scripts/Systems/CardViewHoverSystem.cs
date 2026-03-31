using UnityEngine;
/// <summary>
/// 負責管理滑鼠懸停時顯示的放大卡牌預覽
/// 因為整個遊戲只有一個放大預覽視窗，所以使用Singleton
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

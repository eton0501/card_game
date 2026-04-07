using DG.Tweening;
using UnityEngine;

/// <summary>
/// 依照Card資料在場景建立CardView物件，作為手牌的視覺實例工廠。
/// </summary>


public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;//卡牌視圖的Prefab
    public CardView CreateCardView(Card card,Vector3 position,Quaternion rotation)//在指定位置和旋轉角度建立一個新的CardView，並撥放出現動畫
    {
        CardView cardView=Instantiate(cardViewPrefab,position,rotation);//在指定位置和旋轉角度生成一個指定的的Prefab
        cardView.transform.localScale=Vector3.zero;//將卡牌縮放設為0
        cardView.transform.DOScale(Vector3.one,0.15f);//用0.15秒將卡牌從縮放0放大到正常大小的彈出效果
        cardView.Setup(card);//將卡牌資料綁定到CardView，更新名稱、描述等UI
        return cardView;//回傳建立好的CardView
    }
}
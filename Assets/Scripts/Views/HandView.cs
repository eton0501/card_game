using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class HandView : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;//定義手牌曲線的Spline
    private readonly List<CardView> cards=new();//目前手牌中所有CardView的清單
    public IEnumerator AddCard(CardView cardView)//將一張新卡牌加入手牌
    {
        cards.Add(cardView);//將新卡牌加入清單
        yield return UpdateCardPositions(0.15f);//等待所有卡牌移動動畫結束
    }
    public CardView RemoveCard(Card card)//從手牌移除指定的卡牌，並且觸發剩餘卡牌的重新排列動畫
    {
        CardView cardView=GetCardView(card);//根據card資料找到對應的CardView
        if(cardView==null)return null;//若找不到則回傳null
        cards.Remove(cardView);//從清單中移除
        StartCoroutine(UpdateCardPositions(0.15f));//觸發剩餘卡牌重新排列動畫
        return cardView;//回傳被移除的cardView
    }
    private CardView GetCardView(Card card)//根據card資料取得清單中的CardView
    {
        return cards.Where(CardView=>CardView.Card==card).FirstOrDefault();//找到對應的CardView，找不到則回傳null
    }
    private IEnumerator UpdateCardPositions(float duration)//根據目前手牌數量重新計算每張牌在Spline上的位置和旋轉
    {
        if(cards.Count==0)yield break;//如果手牌數量為0則直接結束
        float cardSpacing=1f/10f;//每張牌之間在Spline上的間距
        float firstCardPosition=0.5f-(cards.Count-1)*cardSpacing/2;//計算第一張牌的起始位置
        Spline spline=splineContainer.Spline;//取得Spline資料
        for(int i = 0; i < cards.Count; i++)
        {
            float p=firstCardPosition+i*cardSpacing;//計算第i張牌在spline上的參數位置(0~1之間)
            Vector3 splinePosition=spline.EvaluatePosition(p);//從Spline取得該參數位置對應的世界座標
            Vector3 forward=spline.EvaluateTangent(p);//取得該點的切線方向(Spline的前進方向)
            Vector3 up=spline.EvaluateUpVector(p);//取得該點的上方向(決定卡牌的傾斜角度)
            Quaternion rotation=Quaternion.LookRotation(-up,Vector3.Cross(-up,forward).normalized);//計算出卡牌的旋轉角度
            cards[i].transform.DOMove(splinePosition+transform.position+0.01f*i*Vector3.back,duration);//平滑移動卡牌到Spline上的對應位置
            cards[i].transform.DORotate(rotation.eulerAngles,duration);//平滑旋轉卡牌到Spline曲線對應的角度
        }
        yield return new WaitForSeconds(duration);//等待動畫撥放完畢
    }
}

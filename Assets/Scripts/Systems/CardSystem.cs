using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using DG.Tweening;
/// <summary>
/// 管理整個卡牌系統的核心邏輯
/// </summary>
public class CardSystem : Singleton<CardSystem>
{
    [SerializeField] private HandView handView;//手牌區的視覺元件
    [SerializeField] private Transform drawPilePoint;//抽牌堆在場景的位置
    [SerializeField] private Transform discardPilePoint;//棄牌堆在場景的位置
    private readonly List<Card> drawPile=new();//抽牌堆
    private readonly List<Card> discardPile=new();//棄牌堆
    private readonly List<Card> hand=new();//手牌
    void OnEnable()//啟用時向ActionSystem註冊所有卡牌相關行動的執行者
    {
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);//抽牌行動
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);//棄掉所有手牌行動
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);//出牌行動
    }
    void OnDisable()//停用時取消所有註冊的執行者
    {
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
    }
    public void Setup(List<CardData> deckData)//初始化卡牌系統
    {
        foreach(var cardData in deckData)
        {
            Card card=new(cardData);
            drawPile.Add(card);//將卡牌加入抽牌堆
        }
    }
    private IEnumerator DrawCardsPerformer(DrawCardsGA drawCardsGA)//抽牌的執行邏輯
    {
        int actualAmount=Mathf.Min(drawCardsGA.Amount,drawPile.Count);//計算實際能抽的張數
        int notDrawnAmount=drawCardsGA.Amount-actualAmount;//計算因抽牌堆不足而無法抽到的張數
        for(int i = 0; i < actualAmount; i++)//先抽取牌堆中現有的牌
        {
            yield return DrawCard();
        }
        if (notDrawnAmount > 0)//若還有未抽到的牌
        {
            RefillDeck();//將棄牌堆洗回抽牌堆
            for(int i = 0; i < notDrawnAmount; i++)//繼續抽
            {
                yield return DrawCard();
            }
        }
    }
    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardAllCardsGA)//棄掉所有手牌的執行邏輯
    {
        foreach(var card in hand)
        {
            CardView cardView=handView.RemoveCard(card);//從手牌的視覺中移除
            yield return DiscardCard(cardView);//撥放棄牌動畫並加入棄牌堆
        }
        hand.Clear();//清空手牌資料清單
    }
    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)//出牌行動的執行邏輯
    {
        hand.Remove(playCardGA.Card);//從手牌資料移除這張牌
        CardView cardView=handView.RemoveCard(playCardGA.Card);//從手牌視覺中移除
        yield return DiscardCard(cardView);//播放移至棄牌堆的動畫
        SpendManaGA spendManaGA=new(playCardGA.Card.Mana);//加入消耗法力的反應行動
        ActionSystem.Instance.AddReaction(spendManaGA);
        if (playCardGA.Card.ManualTargetEffect != null)//如果這張牌要手動指定目標
        {
            PerformEffectGA performEffectGA=new(
            playCardGA.Card.ManualTargetEffect,//要執行的效果
            new(){playCardGA.ManualTarget});//玩家手動選擇的目標
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
        foreach(var effectWrapper in playCardGA.Card.OtherEffects)//依序觸發這張牌的所有自動目標效果
        {
            List<CombatantView> targets=effectWrapper.TargetMode.GetTargets();//根據目標模式取得目標清單
            PerformEffectGA performEffectGA=new(effectWrapper.Effect,targets);
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
    }
    private IEnumerator DrawCard()//從牌堆抽一張牌
    {
        Card card=drawPile.Draw();//從牌堆抽取最上面的牌
        hand.Add(card);//將牌加入手牌清單
        CardView cardView=CardViewCreator.Instance.CreateCardView(card,drawPilePoint.position,drawPilePoint.rotation);//在抽牌堆的位置生成卡牌視覺物件
        yield return handView.AddCard(cardView);//等待卡牌飛入手牌區動畫完成
    }
    private void RefillDeck()//將棄牌堆中所有牌洗回抽牌堆
    {
        drawPile.AddRange(discardPile);//將棄牌堆所有牌加入抽牌堆
        discardPile.Clear();//清空棄牌堆
    }
    private IEnumerator DiscardCard(CardView cardView)//播放卡牌移至棄牌堆的動畫
    {
        discardPile.Add(cardView.Card);//將卡牌資料加入棄牌堆
        cardView.transform.DOScale(Vector3.zero,0.15f);//讓卡牌慢慢縮小至0
        Tween tween=cardView.transform.DOMove(discardPilePoint.position,0.15f);//讓卡牌移動至棄牌堆
        yield return tween.WaitForCompletion();//等待播放動畫播完
        Destroy(cardView.gameObject);//銷毀到棄牌堆卡牌的視覺物件
    }
}

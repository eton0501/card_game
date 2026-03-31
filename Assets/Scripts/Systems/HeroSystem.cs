using UnityEngine;
/// <summary>
/// 負責管理英雄相關邏輯的系統
/// </summary>
public class HeroSystem : Singleton<HeroSystem>
{
    [field:SerializeField] public HeroView HeroView {get;private set;}//英雄的視覺元件
    void OnEnable()
    {
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction,ReactionTiming.PRE);//敵人回合開始前，先棄掉所有手牌
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction,ReactionTiming.POST);//敵人回合結束後，結算燒傷和摸新手牌
    }
    void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction,ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction,ReactionTiming.POST);
    }
    public void Setup(HeroData heroData)//初始化英雄
    {
        HeroView.Setup(heroData);//將HeroData傳給HeroView設定血量和圖片
    }
    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)//敵人回合開始前先棄掉所有手牌
    {
        DiscardAllCardsGA discardAllCardsGA=new();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)//敵人回合結束後觸發
    {
        int burnStacks=HeroView.GetStatusEffectStacks(StatusEffectType.BURN);//取得英雄身上的燒傷層數
        if (burnStacks > 0)
        {
            ApplyBurnGA applyBurnGA=new(burnStacks,HeroView);
            ActionSystem.Instance.AddReaction(applyBurnGA);
        }
        DrawCardsGA drawCardsGA=new(5);//抽五張牌
        ActionSystem.Instance.AddReaction(drawCardsGA);
    }
}

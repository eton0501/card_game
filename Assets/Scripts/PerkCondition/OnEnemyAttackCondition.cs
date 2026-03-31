using System;
using UnityEngine;
/// <summary>
/// 定義敵人攻擊時的天賦觸發條件
/// </summary>
public class OnEnemyAttackCondition : PerkCondition
{
    public override bool SubConditionIsMet(GameAction gameAction)//檢查此行動是否符合條件
    {
       return true; 
    }
    public override void SubscribeCondition(Action<GameAction> reaction)//訂閱條件，開始監聽AttackHeroGA
    {
        ActionSystem.SubscribeReaction<AttackHeroGA>(reaction,reactionTiming);
    }
    public override void UnsubscribeCondition(Action<GameAction> reaction)//取消訂閱條件，停止監聽
    {
        ActionSystem.UnsubscribeReaction<AttackHeroGA>(reaction,reactionTiming);
    }
}

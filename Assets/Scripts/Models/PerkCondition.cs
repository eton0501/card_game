using UnityEngine;
using System;

/// <summary>
/// Perk觸發條件的抽象基底，定義如何訂閱事件、退訂與判斷是否成立。
/// </summary>


public abstract class PerkCondition 
{
    [SerializeField] protected ReactionTiming reactionTiming;//設定要監聽的時機
    public abstract void SubscribeCondition(Action<GameAction> reaction);//訂閱條件監聽
    public abstract void UnsubscribeCondition(Action<GameAction> reaction);//移除條件監聽
    public abstract bool SubConditionIsMet(GameAction gameAction);//判斷行動是否符合此條件
}

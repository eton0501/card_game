using UnityEngine;

/// <summary>
/// 一張牌被打出的行動資料，包含卡牌本體與可選的手動目標。
/// </summary>


public class PlayCardGA : GameAction
{
    public EnemyView ManualTarget{get;private set;}//手動選擇目標的敵人
    public Card Card{get;set;}//這次出牌行動的卡牌
    public PlayCardGA(Card card)//不須手動選擇目標的卡牌
    {
        Card=card;
        ManualTarget=null;
    }
    public PlayCardGA(Card card,EnemyView target)//需要手動選擇目標的卡牌
    {
        Card=card;
        ManualTarget=target;//儲存玩家指定的目標
    }
}

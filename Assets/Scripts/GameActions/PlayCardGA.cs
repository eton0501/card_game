using UnityEngine;
/// <summary>
/// 代表出牌的遊戲行動
/// 根據卡牌是否需要手動選擇目標，提供兩種建構方式
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

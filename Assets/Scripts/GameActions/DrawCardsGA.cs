using UnityEngine;
/// <summary>
/// 代表抽牌的遊戲行動
/// </summary>
public class DrawCardsGA : GameAction
{
    public int Amount{get;set;}//這次行動要抽幾張牌
    public DrawCardsGA(int amount)//建立一個抽牌行動，指定要抽的牌數
    {
        Amount=amount;//將傳入的摸牌數量存入Amount
    }
}

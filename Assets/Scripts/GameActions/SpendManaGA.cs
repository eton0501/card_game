using UnityEngine;
/// <summary>
/// 代表消耗法力的遊戲行動
/// </summary>
public class SpendManaGA : GameAction
{
    public int Amount{get;set;}//這次要消耗的法力
    public SpendManaGA(int amount)//建立一個法力消耗的行動
    {
        Amount=amount;//傳入Amount
    }
}

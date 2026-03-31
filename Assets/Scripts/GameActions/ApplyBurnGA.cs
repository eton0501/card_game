using UnityEngine;
/// <summary>
/// 套用燃燒傷害的遊戲行動資料
/// </summary>
public class ApplyBurnGA : GameAction
{
    public int BurnDamage{get;private set;}//本次燃燒要造成的傷害
    public CombatantView Target{get;private set;}//目標
    public ApplyBurnGA(int burnDamage,CombatantView target)//建立燃燒行動資料
    {
        BurnDamage=burnDamage;
        Target=target;
    }
}


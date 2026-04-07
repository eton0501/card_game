using UnityEngine;

/// <summary>
/// 描述一次燃燒結算行動：燃燒傷害值與受影響目標。
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


using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 主要是實作全體敵人的目標選取模式
/// </summary>
public class AllEnemiesTM : TargetMode
{
    public override List<CombatantView> GetTargets()//回傳目前所有敵人的清單
    {
        return new(EnemySystem.Instance.Enemies);
    }
}

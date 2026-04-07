using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 回傳目前場上所有敵人，給全體效果使用。
/// </summary>

public class AllEnemiesTM : TargetMode
{
    public override List<CombatantView> GetTargets()//回傳目前所有敵人的清單
    {
        return new(EnemySystem.Instance.Enemies);
    }
}

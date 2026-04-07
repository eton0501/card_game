using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 不回傳任何目標，給不需要指定目標的效果使用。
/// </summary>


public class NoTM : TargetMode
{
    public override List<CombatantView> GetTargets()
    {
        return null;
    }

}

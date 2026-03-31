using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 主要是定義選取目標的抽象規則基底
/// </summary>
[System.Serializable]
public abstract class TargetMode
{
    public abstract List<CombatantView> GetTargets();
}

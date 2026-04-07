using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 目標選擇的抽象基底，讓不同卡牌可替換不同選目標規則。
/// </summary>

[System.Serializable]
public abstract class TargetMode
{
    public abstract List<CombatantView> GetTargets();
}

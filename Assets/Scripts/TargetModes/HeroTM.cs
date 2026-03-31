using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 實作英雄自己目標選取模式
/// </summary>
public class HeroTM :TargetMode
{
    public override List<CombatantView> GetTargets()//取得本模式下的目標清單
    {
        List<CombatantView> targets = new()//建立目標清單
        {
            HeroSystem.Instance.HeroView//將英雄加入目標
        };
        return targets;//回傳包含英雄的清單
    }
}

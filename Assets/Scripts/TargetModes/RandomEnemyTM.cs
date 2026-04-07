using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 從現存敵人中隨機挑選一個目標。
/// </summary>

public class RandomEnemyTM : TargetMode
{
    public override List<CombatantView> GetTargets()//從目前敵人清單隨機挑1位並以List回傳
    {
        CombatantView target=EnemySystem.Instance.Enemies[Random.Range(0,EnemySystem.Instance.Enemies.Count)];
        return new(){target};
    }
}

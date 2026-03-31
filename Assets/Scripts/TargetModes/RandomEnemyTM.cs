using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 實作隨機單一敵人的目標選取模式
/// </summary>
public class RandomEnemyTM : TargetMode
{
    public override List<CombatantView> GetTargets()//從目前敵人清單隨機挑1位並以List回傳
    {
        CombatantView target=EnemySystem.Instance.Enemies[Random.Range(0,EnemySystem.Instance.Enemies.Count)];
        return new(){target};
    }
}

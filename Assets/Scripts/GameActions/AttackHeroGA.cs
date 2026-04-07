using UnityEngine;

/// <summary>
/// 描述敵人對英雄發動攻擊的行動資料。
/// </summary>


public class AttackHeroGA : GameAction,IHaveCaster
{
    public EnemyView Attacker{get;private set;}//發動攻擊的敵人
    public CombatantView Caster{get;private set;}//行動發起者
    public AttackHeroGA(EnemyView attacker)//建立一個敵人攻擊英雄的行動
    {
        Attacker=attacker;
        Caster=Attacker;
    }
}

using UnityEngine;
/// <summary>
/// 負責保存本次被擊殺的EnemyView，給ActionSystem使用
/// </summary>
public class KillEnemyGA : GameAction
{
    public EnemyView EnemyView{get;private set;}
   public KillEnemyGA(EnemyView enemyView)
    {
        EnemyView=enemyView;
    }
}

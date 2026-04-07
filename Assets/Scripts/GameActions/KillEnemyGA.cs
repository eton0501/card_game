using UnityEngine;

/// <summary>
/// 擊殺指定敵人的行動資料。
/// </summary>


public class KillEnemyGA : GameAction
{
    public EnemyView EnemyView{get;private set;}
   public KillEnemyGA(EnemyView enemyView)
    {
        EnemyView=enemyView;
    }
}

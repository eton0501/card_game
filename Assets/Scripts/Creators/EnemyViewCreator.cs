using UnityEngine;

/// <summary>
/// 依 EnemyData 與遭遇倍率建立 EnemyView，負責把敵人預製體正確初始化到戰場。
/// </summary>


public class EnemyViewCreator : Singleton<EnemyViewCreator>
{
    [SerializeField] private EnemyView enemyViewPrefab;

    public EnemyView CreateEnemyView(
        EnemyData enemyData,
        Vector3 position,
        Quaternion rotation,
        float healthMultiplier = 1f,
        float attackMultiplier = 1f
    )
    {
        EnemyView enemyView = Instantiate(enemyViewPrefab, position, rotation);
        enemyView.Setup(enemyData, healthMultiplier, attackMultiplier);
        return enemyView;
    }
}

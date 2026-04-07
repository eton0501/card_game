using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 管理敵人在畫面上的生成、刪除。
/// </summary>


public class EnemyBoardView : MonoBehaviour
{
    [SerializeField] private List<Transform> slots;
    public List<EnemyView> EnemyViews { get; private set; } = new();

    public void AddEnemy(EnemyData enemyData, float healthMultiplier = 1f, float attackMultiplier = 1f)
    {
        Transform slot = slots[EnemyViews.Count];
        EnemyView enemyView = EnemyViewCreator.Instance.CreateEnemyView(
            enemyData,
            slot.position,
            slot.rotation,
            healthMultiplier,
            attackMultiplier
        );
        enemyView.transform.parent = slot;
        EnemyViews.Add(enemyView);
    }

    public IEnumerator RemoveEnemy(EnemyView enemyView)
    {
        EnemyViews.Remove(enemyView);
        Tween tween = enemyView.transform.DOScale(Vector3.zero, 0.25f);
        yield return tween.WaitForCompletion();
        Destroy(enemyView.gameObject);
    }

    public void ClearAllEnemiesImmediate()
    {
        List<EnemyView> currentEnemies = EnemyViews.ToList();
        foreach (EnemyView enemyView in currentEnemies)
        {
            if (enemyView != null)
            {
                Destroy(enemyView.gameObject);
            }
        }

        EnemyViews.Clear();
    }
}

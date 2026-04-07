using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定義地圖選項資料，供遭遇選擇流程使用。
/// </summary>

[System.Serializable]
public class MapEncounterOption
{
    [SerializeField] private string title = "Battle";
    [SerializeField] private List<EnemyData> enemyDatas = new();

    public string Title => title;
    public List<EnemyData> EnemyDatas => enemyDatas;
}

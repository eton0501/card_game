using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定義路線地圖單一節點資料。
/// </summary>

[System.Serializable]
public class MapNodeData
{
    [SerializeField] private string id = "node_1";
    [SerializeField] private string title = "Battle";
    [SerializeField] private MapNodeType nodeType = MapNodeType.Battle;
    [SerializeField] private bool isStartNode = false;
    [SerializeField] private List<EnemyData> enemyDatas = new();
    [SerializeField] private List<string> nextNodeIds = new();

    public string Id => id;
    public string Title => title;
    public MapNodeType NodeType => nodeType;
    public bool IsStartNode => isStartNode;
    public List<EnemyData> EnemyDatas => enemyDatas;
    public List<string> NextNodeIds => nextNodeIds;
}

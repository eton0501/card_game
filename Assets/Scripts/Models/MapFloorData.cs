using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定義地圖樓層資料，包含該樓層可選的遭遇選項集合。
/// </summary>

[System.Serializable]

public class MapFloorData
{
    [SerializeField] private string floorTitle = "Floor";
    [SerializeField] private List<MapEncounterOption> options = new();

    public string FloorTitle => floorTitle;
    public List<MapEncounterOption> Options => options;
}

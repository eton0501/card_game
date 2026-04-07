using System.Collections.Generic;
using UnityEngine;
using SerializeReferenceEditor;

/// <summary>
/// 儲存卡牌的靜態設定資料，包含費用、描述、圖片、目標效果與商店價格。
/// </summary>

[CreateAssetMenu(menuName ="Data/Card")]
public class CardData : ScriptableObject
{
    [field:SerializeField] public string Description{get;private set;}
    [field:SerializeField] public int Mana{get;private set;}
    [field:SerializeField] public int ShopCost{get;private set;} = 45;
    [field:SerializeField] public Sprite Image{get;private set;}
    [field:SerializeReference,SR] public Effect ManualTargetEffect{get;private set;}=null;
    [field:SerializeField] public List<AutoTargetEffect> OtherEffects{get; private set;}
}

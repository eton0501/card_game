using UnityEngine;
using SerializeReferenceEditor;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
/// <summary>
/// 定義Perk的資料資產
/// </summary>
[CreateAssetMenu(menuName ="Data/Perk")]

public class PerkData : ScriptableObject
{
    [field:SerializeField] public Sprite Image{get;private set;}//天賦顯示用圖片
    [field:SerializeReference,SR]public PerkCondition PerkCondition{get;private set;}//天賦觸發條件
    [field:SerializeReference,SR] public AutoTargetEffect AutoTargetEffect{get;private set;}//觸發後要執行的自動選目標效果資料
    [field:SerializeField] public bool UseAutoTarget{get;private set;}=true;//是否啟用自動選目標
    [field:SerializeField] public bool UseActionCasterAsTarget{get;private set;}=false;//是否把行動施放者本身當作效果目標
}

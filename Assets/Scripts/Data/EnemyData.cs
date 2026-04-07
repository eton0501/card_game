using UnityEngine;

/// <summary>
/// 儲存敵人的靜態數值與外觀資料，提供生成敵人時使用。
/// </summary>

[CreateAssetMenu(menuName ="Data/Enemy")]
public class EnemyData : ScriptableObject
{
    [field:SerializeField] public Sprite Image{get;private set;}//敵人的圖片
    [field:SerializeField] public int Health{get; private set;}//敵人的血量
    [field:SerializeField] public int AttackPower{get;private set;}//敵人的攻擊力
}

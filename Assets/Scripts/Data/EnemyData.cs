using UnityEngine;
/// <summary>
/// 敵人的資料容器
/// </summary>
[CreateAssetMenu(menuName ="Data/Enemy")]
public class EnemyData : ScriptableObject
{
    [field:SerializeField] public Sprite Image{get;private set;}//敵人的圖片
    [field:SerializeField] public int Health{get; private set;}//敵人的血量
    [field:SerializeField] public int AttackPower{get;private set;}//敵人的攻擊力
}

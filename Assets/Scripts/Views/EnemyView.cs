using TMPro;
using UnityEngine;
/// <summary>
/// 敵人的視覺元件
/// </summary>
public class EnemyView :CombatantView
{
    [SerializeField] private TMP_Text attackText;//顯示敵人攻擊力的元件
    public int AttackPower{get;set;}//敵人的攻擊力
    public void Setup(EnemyData enemyData)//初始化敵人
    {
        AttackPower=enemyData.AttackPower;//設定攻擊力
        UpdateAttackText();//更新UI
        SetupBase(enemyData.Health,enemyData.Image);//呼叫父類別(CombatanView)初始化血量和圖片
    }
    private void UpdateAttackText()//更新攻擊力的UI
    {
        attackText.text="ATK: "+AttackPower;
    }
}

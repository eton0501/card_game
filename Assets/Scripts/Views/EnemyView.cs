using TMPro;
using UnityEngine;

/// <summary>
/// 敵人的視圖與戰鬥資料，包含攻擊力與意圖UI更新。
/// </summary>


public class EnemyView : CombatantView
{
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private EnemyIntentUI enemyIntentUI;
    [SerializeField, Range(0f, 1f)] private float attackIntentChance = 0.7f;
    [SerializeField, Range(0f, 1f)] private float defendIntentChance = 0.2f;
    [SerializeField] private int defendArmorAmount = 3;
    [SerializeField] private int burnStackAmount = 1;

    public int AttackPower { get; private set; }
    public EnemyIntentType IntentType { get; private set; } = EnemyIntentType.Attack;
    public int IntentValue { get; private set; } = 0;

    public void Setup(EnemyData enemyData, float healthMultiplier = 1f, float attackMultiplier = 1f)
    {
        if (enemyData == null)
        {
            AttackPower = 0;
            UpdateAttackText();
            SetupBase(1, null);
            return;
        }

        int scaledHealth = Mathf.Max(1, Mathf.CeilToInt(enemyData.Health * Mathf.Max(0.01f, healthMultiplier)));
        int scaledAttack = Mathf.Max(1, Mathf.CeilToInt(enemyData.AttackPower * Mathf.Max(0.01f, attackMultiplier)));

        AttackPower = scaledAttack;
        UpdateAttackText();
        SetupBase(scaledHealth, enemyData.Image);
    }

    public void RollNextIntent()
    {
        float roll = Random.value;
        float attackThreshold = Mathf.Clamp01(attackIntentChance);
        float defendThreshold = Mathf.Clamp01(attackIntentChance + defendIntentChance);

        if (roll < attackThreshold)
        {
            SetIntent(EnemyIntentType.Attack, AttackPower);
            return;
        }

        if (roll < defendThreshold)
        {
            SetIntent(EnemyIntentType.Defend, defendArmorAmount);
            return;
        }

        SetIntent(EnemyIntentType.ApplyBurn, burnStackAmount);
    }

    private void SetIntent(EnemyIntentType intentType, int value)
    {
        IntentType = intentType;
        IntentValue = value;
        enemyIntentUI?.SetIntent(intentType, value);
    }

    private void UpdateAttackText()
    {
        if (attackText != null)
        {
            attackText.text = "ATK: " + AttackPower;
        }
    }
}

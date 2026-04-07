using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// 英雄和敵人共用戰鬥基底，負責血量、受傷、護甲抵擋、狀態堆疊與傷害修正。
/// </summary>


public class CombatantView : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private StatusEffectsUI statusEffectsUI;

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    private readonly Dictionary<StatusEffectType, int> statusEffects = new();

    protected void SetupBase(int health, Sprite image)
    {
        MaxHealth = CurrentHealth = health;
        spriteRenderer.sprite = image;
        UpdateHealthText();
    }

    public void Damage(int damageAmount)
    {
        int remainingDamage = damageAmount;
        int currentArmor = GetStatusEffectStacks(StatusEffectType.ARMOR);

        if (currentArmor > 0)
        {
            if (currentArmor >= remainingDamage)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, remainingDamage);
                remainingDamage = 0;
            }
            else
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, currentArmor);
                remainingDamage -= currentArmor;
            }
        }

        if (remainingDamage > 0)
        {
            CurrentHealth -= remainingDamage;
            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
            }
        }

        transform.DOShakePosition(0.2f, 0.5f);
        UpdateHealthText();
    }

    public int ModifyOutgoingAttackDamage(int baseDamage)
    {
        int damage = Mathf.Max(0, baseDamage);
        int weakStacks = GetStatusEffectStacks(StatusEffectType.WEAK);
        if (weakStacks > 0)
        {
            damage = Mathf.FloorToInt(damage * 0.75f);
        }
        return Mathf.Max(0, damage);
    }

    public int ModifyIncomingAttackDamage(int baseDamage)
    {
        int damage = Mathf.Max(0, baseDamage);
        int vulnerableStacks = GetStatusEffectStacks(StatusEffectType.VULNERABLE);
        if (vulnerableStacks > 0)
        {
            damage = Mathf.FloorToInt(damage * 1.5f);
        }
        return Mathf.Max(0, damage);
    }

    public void TickTurnBasedDebuffs()
    {
        ReduceStatusIfPresent(StatusEffectType.WEAK, 1);
        ReduceStatusIfPresent(StatusEffectType.VULNERABLE, 1);
    }

    public void HealByPercent(float percent)
    {
        if (percent <= 0f) return;
        int healAmount = Mathf.CeilToInt(MaxHealth * percent);
        Heal(healAmount);
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        UpdateHealthText();
    }

    public void AddStatusEffect(StatusEffectType type, int stackCount)
    {
        if (stackCount <= 0) return;

        if (statusEffects.ContainsKey(type))
        {
            statusEffects[type] += stackCount;
        }
        else
        {
            statusEffects.Add(type, stackCount);
        }

        statusEffectsUI?.UpdateStatusEffectUI(type, GetStatusEffectStacks(type));
    }

    public void RemoveStatusEffect(StatusEffectType type, int stackCount)
    {
        if (stackCount <= 0) return;

        if (statusEffects.ContainsKey(type))
        {
            statusEffects[type] -= stackCount;
            if (statusEffects[type] <= 0)
            {
                statusEffects.Remove(type);
            }
        }

        statusEffectsUI?.UpdateStatusEffectUI(type, GetStatusEffectStacks(type));
    }

    public int GetStatusEffectStacks(StatusEffectType type)
    {
        if (statusEffects.ContainsKey(type))
        {
            return statusEffects[type];
        }
        return 0;
    }

    private void UpdateHealthText()
    {
        healthText.text = "HP: " + CurrentHealth;
    }

    private void ReduceStatusIfPresent(StatusEffectType type, int amount)
    {
        if (GetStatusEffectStacks(type) <= 0) return;
        RemoveStatusEffect(type, amount);
    }
}

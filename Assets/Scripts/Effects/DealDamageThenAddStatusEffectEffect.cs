using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 先對目標造成傷害，再接續套用狀態。
/// </summary>


public class DealDamageThenAddStatusEffectEffect : Effect
{
    [SerializeField] private int damageAmount;
    [SerializeField] private StatusEffectType statusEffectType;
    [SerializeField] private int stackCount;

    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        List<CombatantView> safeTargets = targets ?? new List<CombatantView>();
        DealDamageGA dealDamageGA = new(damageAmount, safeTargets, caster);
        AddStatusEffectGA addStatusEffectGA = new(statusEffectType, stackCount, safeTargets);
        dealDamageGA.PerformReactions.Add(addStatusEffectGA);
        return dealDamageGA;
    }
}

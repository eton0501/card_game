using System.Collections.Generic;

/// <summary>
/// 描述一次附加狀態行動：要加哪種狀態、幾層、套用到哪些目標。
/// </summary>


public class AddStatusEffectGA : GameAction
{
    public StatusEffectType StatusEffectType { get; private set; }
    public int StackCount { get; private set; }
    public List<CombatantView> Targets { get; private set; }

    public AddStatusEffectGA(StatusEffectType statusEffectType, int stackCount, List<CombatantView> targets)
    {
        StatusEffectType = statusEffectType;
        StackCount = stackCount;
        Targets = targets == null ? new List<CombatantView>() : new List<CombatantView>(targets);
    }
}

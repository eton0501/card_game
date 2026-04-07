using System.Collections;
using UnityEngine;

/// <summary>
/// 註冊AddStatusEffectGA的執行器，把指定狀態與層數套用到目標。
/// </summary>


public class StatusEffectStstem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<AddStatusEffectGA>(AddStatusEffectPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<AddStatusEffectGA>();
    }

    private IEnumerator AddStatusEffectPerformer(AddStatusEffectGA addStatusEffectGA)
    {
        if (addStatusEffectGA == null || addStatusEffectGA.Targets == null) yield break;

        foreach (CombatantView target in addStatusEffectGA.Targets)
        {
            if (target == null) continue;
            target.AddStatusEffect(addStatusEffectGA.StatusEffectType, addStatusEffectGA.StackCount);
            yield return null;
        }
    }
}

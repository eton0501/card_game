using System.Collections;
using UnityEngine;
/// <summary>
/// 註冊並執行新增狀態效果行動的Performer
/// </summary>
public class StatusEffectStstem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<AddStatusEffectGA>(AddStatusEffectPerformer);//向ActionSystem註冊AddStatusEffectGA
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<AddStatusEffectGA>();//取消註冊
    }
    private IEnumerator AddStatusEffectPerformer(AddStatusEffectGA addStatusEffectGA)//AddStatusEffectGA的實際執行邏輯
    {
        foreach(var target in addStatusEffectGA.Targets)//逐一處理每個目標
        {
            target.AddStatusEffect(addStatusEffectGA.StatusEffectType,addStatusEffectGA.StackCount);//對目標增加狀態和層數
            yield return null;
        }
    }
}

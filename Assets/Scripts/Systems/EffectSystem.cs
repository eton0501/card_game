using System.Collections;
using UnityEngine;

/// <summary>
/// 接收PerformEffectGA並呼叫Effect產生對應的GameAction，再加入ActionSystem反應串。
/// </summary>


public class EffectSystem : MonoBehaviour
{
    void OnEnable()//啟用時向ActionSystem註冊效果執行者
    {
        ActionSystem.AttachPerformer<PerformEffectGA>(PerformEffectPerformer);
    }
    void OnDisable()//停用時取消註冊
    {
        ActionSystem.DetachPerformer<PerformEffectGA>();
    }
    private IEnumerator PerformEffectPerformer(PerformEffectGA performEffectGA)//效果執行的核心邏輯
    {
        GameAction effectAction=performEffectGA.Effect.GetGameAction(
        performEffectGA.Targets//效果的目標清單
        ,HeroSystem.Instance.HeroView);//發動者是Hero
        ActionSystem.Instance.AddReaction(effectAction);//將產生的行動加入反應序列
        yield return null;
    }
}


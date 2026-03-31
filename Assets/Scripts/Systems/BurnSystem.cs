using System.Collections;
using UnityEngine;
/// <summary>
/// 執行燃燒傷害的行動
/// </summary>
public class BurnSystem : MonoBehaviour
{
    [SerializeField] private GameObject burnVFX;//燃燒特效
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ApplyBurnGA>(ApplyBurnPerformer);//註冊ApplyBurnGA
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ApplyBurnGA>();//取消註冊
    }
    private IEnumerator ApplyBurnPerformer(ApplyBurnGA applyBurnGA)//實係執行邏輯
    {
        CombatantView target=applyBurnGA.Target;//取出燃燒行動的目標
        Instantiate(burnVFX,target.transform.position,Quaternion.identity);//在目標位置生成燃燒特效
        target.Damage(applyBurnGA.BurnDamage);//對目標造成燃燒傷害
        target.RemoveStatusEffect(StatusEffectType.BURN,1);//移除目標一層燃燒狀態
        yield return new WaitForSeconds(1f);
    }
}

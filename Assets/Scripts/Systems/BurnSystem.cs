using System.Collections;
using UnityEngine;

/// <summary>
/// 執行ApplyBurnGA：播放燃燒特效、扣血、減少燃燒層數。
/// </summary>


public class BurnSystem : MonoBehaviour
{
    [SerializeField] private GameObject burnVFX;
    [SerializeField] private float burnVfxLifetime = 1f;
    [SerializeField] private float burnTickDelay = 1f;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ApplyBurnGA>(ApplyBurnPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ApplyBurnGA>();
    }

    private IEnumerator ApplyBurnPerformer(ApplyBurnGA applyBurnGA)
    {
        CombatantView target = applyBurnGA.Target;
        if (target == null) yield break;

        if (burnVFX != null)
        {
            GameObject vfx = Instantiate(burnVFX, target.transform.position, Quaternion.identity);
            Destroy(vfx, burnVfxLifetime);
        }

        target.Damage(applyBurnGA.BurnDamage);
        target.RemoveStatusEffect(StatusEffectType.BURN, 1);

        
        if (target.CurrentHealth <= 0 && target is EnemyView enemyView)
        {
            ActionSystem.Instance.AddReaction(new KillEnemyGA(enemyView));
        }

        yield return new WaitForSeconds(burnTickDelay);
    }
}

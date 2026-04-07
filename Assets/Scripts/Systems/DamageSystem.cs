using System.Collections;
using UnityEngine;

/// <summary>
/// 結算DealDamageGA，套用Weak或Vulnerable等修正後造成傷害並處理目標死亡。
/// </summary>


public class DamageSystem : MonoBehaviour
{
    [SerializeField] private GameObject damageVFX;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
    }

    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)
    {
        if (dealDamageGA == null || dealDamageGA.Targets == null) yield break;

        foreach (CombatantView target in dealDamageGA.Targets)
        {
            if (target == null) continue;

            int finalDamage = Mathf.Max(0, dealDamageGA.Amount);
            if (dealDamageGA.Caster != null)
            {
                finalDamage = dealDamageGA.Caster.ModifyOutgoingAttackDamage(finalDamage);
            }

            finalDamage = target.ModifyIncomingAttackDamage(finalDamage);
            target.Damage(finalDamage);

            if (damageVFX != null)
            {
                Instantiate(damageVFX, target.transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(0.15f);

            if (target.CurrentHealth <= 0 && target is EnemyView enemyView)
            {
                KillEnemyGA killEnemyGA = new(enemyView);
                ActionSystem.Instance.AddReaction(killEnemyGA);
            }
        }
    }
}

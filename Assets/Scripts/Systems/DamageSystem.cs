using System.Collections;
using UnityEngine;
/// <summary>
/// 負責執行傷害結算的系統
/// </summary>
public class DamageSystem : MonoBehaviour
{
    [SerializeField] private GameObject damageVFX;//受傷時撥放的特效
    void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
    }
    void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
    }
    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)//傷害結算的執行邏輯
    {
        foreach(var target in dealDamageGA.Targets)
        {
            target.Damage(dealDamageGA.Amount);//對目標造成傷害
            Instantiate(damageVFX,target.transform.position,Quaternion.identity);//在目標位置生成受傷特效
            yield return new WaitForSeconds(0.15f);
            if (target.CurrentHealth <= 0)//判斷目標血量是否歸零
            {
                if(target is EnemyView enemyView)//如果目標是敵人
                {
                    KillEnemyGA killEnemyGA=new(enemyView);
                    ActionSystem.Instance.AddReaction(killEnemyGA);//加入擊殺敵人的行動
                }
                else
                {
                    
                }
            }
        }
    }
}

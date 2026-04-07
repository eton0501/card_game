using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 管理敵人回合執行邏輯：意圖、攻擊、防禦、上燃燒與回合後重骰意圖。
/// </summary>


public class EnemySystem : Singleton<EnemySystem>
{
    [SerializeField] private EnemyBoardView enemyBoardView;
    [SerializeField] private GameObject defendCastVFX;
    [SerializeField] private GameObject burnCastVFX;
    [SerializeField] private float castVfxLifeTime = 1.2f;
    [SerializeField] private float intentActionDelay = 0.2f;

    public List<EnemyView> Enemies => enemyBoardView.EnemyViews;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<AttackHeroGA>(AttackHeroPerformer);
        ActionSystem.AttachPerformer<KillEnemyGA>(KillEnemyPerformer);
        ActionSystem.AttachPerformer<RefreshEnemyIntentsGA>(RefreshEnemyIntentsPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackHeroGA>();
        ActionSystem.DetachPerformer<KillEnemyGA>();
        ActionSystem.DetachPerformer<RefreshEnemyIntentsGA>();
    }

    public void Setup(
        List<EnemyData> enemyDatas,
        float healthMultiplier = 1f,
        float attackMultiplier = 1f
    )
    {
        enemyBoardView.ClearAllEnemiesImmediate();

        if (enemyDatas == null) return;
        int spawnedCount = 0;
        foreach (var enemyData in enemyDatas)
        {
            if (enemyData == null) continue;
            enemyBoardView.AddEnemy(enemyData, healthMultiplier, attackMultiplier);
            EnemyView spawnedEnemy = enemyBoardView.EnemyViews[enemyBoardView.EnemyViews.Count - 1];
            spawnedEnemy.RollNextIntent();
            spawnedCount++;
        }

        if (spawnedCount == 0)
        {
            Debug.LogWarning("no EnemyData.");
        }
    }

    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        foreach (var enemy in enemyBoardView.EnemyViews)
        {
            if (enemy == null) continue;

            int burnStacks = enemy.GetStatusEffectStacks(StatusEffectType.BURN);
            bool willDieFromBurnTick = false;
            if (burnStacks > 0)
            {
                int armorStacks = enemy.GetStatusEffectStacks(StatusEffectType.ARMOR);
                int expectedBurnDamage = Mathf.Max(0, burnStacks - armorStacks);
                willDieFromBurnTick = enemy.CurrentHealth - expectedBurnDamage <= 0;

                ApplyBurnGA applyBurnGA = new(burnStacks, enemy);
                ActionSystem.Instance.AddReaction(applyBurnGA);
            }

            
            if (willDieFromBurnTick)
            {
                yield return new WaitForSeconds(intentActionDelay);
                continue;
            }

            switch (enemy.IntentType)
            {
                case EnemyIntentType.Attack:
                {
                    AttackHeroGA attackHeroGA = new(enemy);
                    ActionSystem.Instance.AddReaction(attackHeroGA);
                    yield return new WaitForSeconds(intentActionDelay);
                    break;
                }
                case EnemyIntentType.Defend:
                {
                    yield return PlayIntentAnimation(enemy);
                    if (defendCastVFX != null)
                    {
                        SpawnTemporaryVfx(defendCastVFX, enemy.transform.position);
                    }
                    AddStatusEffectGA addArmorGA = new(
                        StatusEffectType.ARMOR,
                        enemy.IntentValue,
                        new() { enemy }
                    );
                    ActionSystem.Instance.AddReaction(addArmorGA);
                    yield return new WaitForSeconds(intentActionDelay);
                    break;
                }
                case EnemyIntentType.ApplyBurn:
                {
                    yield return PlayIntentAnimation(enemy);
                    if (burnCastVFX != null)
                    {
                        SpawnTemporaryVfx(burnCastVFX, HeroSystem.Instance.HeroView.transform.position);
                    }
                    AddStatusEffectGA addBurnGA = new(
                        StatusEffectType.BURN,
                        enemy.IntentValue,
                        new() { HeroSystem.Instance.HeroView }
                    );
                    ActionSystem.Instance.AddReaction(addBurnGA);
                    yield return new WaitForSeconds(intentActionDelay);
                    break;
                }
            }
        }

        
        List<EnemyView> currentEnemies = new(enemyBoardView.EnemyViews);
        foreach (EnemyView enemy in currentEnemies)
        {
            if (enemy == null) continue;
            enemy.TickTurnBasedDebuffs();
        }

        ActionSystem.Instance.AddReaction(new RefreshEnemyIntentsGA());

        yield return null;
    }

    private IEnumerator AttackHeroPerformer(AttackHeroGA attackHeroGA)
    {
        EnemyView attacker = attackHeroGA.Attacker;
        if (attacker == null || attacker.CurrentHealth <= 0)
        {
            yield break;
        }

        Tween tween = attacker.transform.DOMoveX(attacker.transform.position.x - 1f, 0.15f);
        yield return tween.WaitForCompletion();

        attacker.transform.DOMoveX(attacker.transform.position.x + 1f, 0.25f);

        DealDamageGA dealDamageGA = new(
            attacker.AttackPower,
            new() { HeroSystem.Instance.HeroView },
            attackHeroGA.Caster
        );
        ActionSystem.Instance.AddReaction(dealDamageGA);
    }

    private IEnumerator KillEnemyPerformer(KillEnemyGA killEnemyGA)
    {
        yield return enemyBoardView.RemoveEnemy(killEnemyGA.EnemyView);
    }

    private IEnumerator RefreshEnemyIntentsPerformer(RefreshEnemyIntentsGA refreshEnemyIntentsGA)
    {
        List<EnemyView> currentEnemies = new(enemyBoardView.EnemyViews);
        foreach (var enemy in currentEnemies)
        {
            enemy.RollNextIntent();
        }
        yield return null;
    }

    private IEnumerator PlayIntentAnimation(EnemyView enemy)
    {
        Vector3 startPosition = enemy.transform.position;
        Tween moveForward = enemy.transform.DOMoveY(startPosition.y + 0.2f, 0.1f);
        yield return moveForward.WaitForCompletion();

        Tween moveBack = enemy.transform.DOMoveY(startPosition.y, 0.12f);
        yield return moveBack.WaitForCompletion();
    }

    private void SpawnTemporaryVfx(GameObject vfxPrefab, Vector3 position)
    {
        GameObject spawnedVfx = Instantiate(vfxPrefab, position, Quaternion.identity);
        Destroy(spawnedVfx, castVfxLifeTime);
    }
}

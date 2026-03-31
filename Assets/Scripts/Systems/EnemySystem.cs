using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Collections;
/// <summary>
/// 負責管理所有敵人相關的遊戲行動
/// </summary>
public class EnemySystem : Singleton<EnemySystem>
{
    [SerializeField] private EnemyBoardView enemyBoardView;//負責管理場上敵人視覺的物件
    public List<EnemyView> Enemies=>enemyBoardView.EnemyViews;//直接從enemyBoardView取得目前場上敵人的EnemyView清單
    void OnEnable()//當此物件啟用時，向ActionSystem註冊個行動對應的執行者
    {
       ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);//敵人回合行動
       ActionSystem.AttachPerformer<AttackHeroGA>(AttackHeroPerformer); //敵人攻擊行動
       ActionSystem.AttachPerformer<KillEnemyGA>(KillEnemyPerformer);//敵人死亡行動
    }
    void OnDisable()//當此物件停用時，從ActionSystem取消所有已註冊的執行者
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackHeroGA>();
        ActionSystem.DetachPerformer<KillEnemyGA>();
    }
    public void Setup(List<EnemyData> enemyDatas)//初始化戰鬥，根據關卡資料將所有敵人加入場上
    {
        foreach(var enemyData in enemyDatas)
        {
            enemyBoardView.AddEnemy(enemyData);//逐一將敵人加入場上的視覺
        }
    }
    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)//敵人回合的執行邏輯
    {
        foreach(var enemy in enemyBoardView.EnemyViews)//逐一跑過每個敵人
        {
           int burnStacks=enemy.GetStatusEffectStacks(StatusEffectType.BURN);//取得這個敵人身上的燃燒層數
            if (burnStacks > 0)//如果燃燒層數大於0
            {
                ApplyBurnGA applyBurnGA=new(burnStacks,enemy);
                ActionSystem.Instance.AddReaction(applyBurnGA);//加入對自己造成燃燒傷害的反應行動
            }
           AttackHeroGA attackHeroGA=new(enemy);
           ActionSystem.Instance.AddReaction(attackHeroGA);//加入攻擊英雄的反應行動
        }
        yield return null;//等待一幀
    }
    private IEnumerator AttackHeroPerformer(AttackHeroGA attackHeroGA)//敵人攻擊英雄的執行邏輯
    {
        EnemyView attacker=attackHeroGA.Attacker;
        Tween tween=attacker.transform.DOMoveX(attacker.transform.position.x-1f,0.15f);//敵人向左移動1單位在0.15秒完成
        yield return tween.WaitForCompletion();//等待動畫播完
        attacker.transform.DOMoveX(attacker.transform.position.x+1f,0.25f);//敵人退回原位在0.25秒完成
        DealDamageGA dealDamageGA=new(attacker.AttackPower,//傷害值
        new(){HeroSystem.Instance.HeroView},//傷害目標
        attackHeroGA.Caster);//行動發起者
        ActionSystem.Instance.AddReaction(dealDamageGA);//加入對英雄造成傷害的反應行動
    }
    private IEnumerator KillEnemyPerformer(KillEnemyGA killEnemyGA)//敵人的死亡邏輯
    {
        yield return enemyBoardView.RemoveEnemy(killEnemyGA.EnemyView);//等待移除動畫播完後才執行後續行動
    }
}

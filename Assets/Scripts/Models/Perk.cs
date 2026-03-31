using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 主要是天賦實例的執行邏輯
/// </summary>
public class Perk 
{
    public Sprite Image=>data.Image;//對外提供天賦圖片
    private readonly PerkData data;//天賦原始資料
    private readonly PerkCondition condition;//天賦觸發條件
    private readonly AutoTargetEffect effect;//天賦觸發後要執行的目標效果與目標模式
    public Perk(PerkData perkData)//建立天賦實例
    {
        data=perkData;
        condition=data.PerkCondition;
        effect=data.AutoTargetEffect;
    }
    public void OnAdd()
    {
        condition.SubscribeCondition(Reaction);//訂閱條件事件
    }
    public void OnRemove()
    {
        condition.UnsubscribeCondition(Reaction);//取消訂閱
    }
    private void Reaction(GameAction gameAction)//收到條件事件的回呼
    {
        if (condition.SubConditionIsMet(gameAction))//先檢查是否符合條件
        {
            List<CombatantView> targets=new();//建立本次天賦效果的目標清單
            if(data.UseActionCasterAsTarget&&gameAction is IHaveCaster haveCaster)//若設定要把施放者當作目標
            {
                targets.Add(haveCaster.Caster);//將施放者加入目標
            }
            if (data.UseAutoTarget)//若設定自動選目標
            {
                targets.AddRange(effect.TargetMode.GetTargets());//依TargetMode取得目標並加入清單
            }
            GameAction perkEffectAction=effect.Effect.GetGameAction(targets,HeroSystem.Instance.HeroView);//用目標清單與施放者建立天賦效果行動
            ActionSystem.Instance.AddReaction(perkEffectAction);//把天賦效果插入ActionSystem執行
        }
    }
}

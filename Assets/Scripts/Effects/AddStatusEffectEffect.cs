using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 主要是把新增狀態的效果資料轉成可執行的遊戲行動
/// </summary>
public class AddStatusEffectEffect : Effect
{
    [SerializeField] private StatusEffectType statusEffectType;//要附加的狀態效果
    [SerializeField] private int stackCount;//要附加的層數
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)//把效果轉成GameAction
    {
        return new AddStatusEffectGA(statusEffectType,stackCount,targets);//建立新增狀態行動
    }
}

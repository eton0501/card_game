using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 負責把造成傷害效果轉換成可執行的遊戲行動(DealDamageGA)
/// 當效果被觸發時，會依照設定的damageAmount和targets和caster建立對應的傷害行動
/// </summary>
public class DealDamageEffect : Effect
{
    [SerializeField] private int damageAmount;
    public override GameAction GetGameAction(List<CombatantView> targets,CombatantView caster)
    {
        DealDamageGA dealDamageGA=new(damageAmount,targets,caster);
        return dealDamageGA;
    }
}

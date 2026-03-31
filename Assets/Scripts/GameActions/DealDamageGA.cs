using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 代表造成傷害的遊戲行動
/// </summary>
public class DealDamageGA : GameAction,IHaveCaster
{
    public int Amount{get;set;}//造成的傷害量
    public List<CombatantView> Targets{get;set;}//傷害目標的清單
    public CombatantView Caster{get;private set;}//傷害的發動者
    public DealDamageGA(int amount,List<CombatantView>targets,CombatantView caster)//建立一個造成傷害的行動
    {
        Amount=amount;
        Targets=new(targets);
        Caster=caster;
    }
}

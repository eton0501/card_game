using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
/// <summary>
/// 定義新增狀態效果的遊戲行動資料
/// 保存要新增的狀態類型、層數、目標清單，給ActionSystem使用
/// </summary>
public class AddStatusEffectGA : GameAction
{
    public StatusEffectType StatusEffectType{get;private set;}//要新增的狀態效果類型
    public int StackCount{get;private set;}//要增加的層數
    public List<CombatantView> Targets{get;private set;}//目標清單
    public AddStatusEffectGA(StatusEffectType statusEffectType,int stackCount,List<CombatantView> targets)//建立新增狀態行動
    {
        StatusEffectType=statusEffectType;
        StackCount=stackCount;
        Targets=targets;
    }
}

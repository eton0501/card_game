using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effect的執行請求，包含要執行的效果與目標清單。
/// </summary>


public class PerformEffectGA : GameAction
{
    public Effect Effect{get;set;}//要執行的效果
    public List<CombatantView> Targets{get;set;}//效果要作用的目標清單
    public PerformEffectGA(Effect effect,List<CombatantView> targets)//建立一個執行效果的行動
    {
        Effect=effect;
        Targets=targets==null?null:new(targets);//targets==null就直接存null，targets!=null建立一份新的List
    }
}

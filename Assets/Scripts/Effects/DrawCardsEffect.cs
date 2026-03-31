using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 抽牌效果的具體實作
/// 繼承自Effect，當卡牌效果觸發時產生抽牌行動
/// </summary>
public class DrawCardsEffect : Effect
{
    [SerializeField] private int drawAmount;//要抽幾張牌，可以在Inspector中設定
    public override GameAction GetGameAction(List<CombatantView> targets,CombatantView caster)
    {
        DrawCardsGA drawCardsGA=new(drawAmount);//建立抽牌行動
        return drawCardsGA;//交給ActionSystem執行
    }
}

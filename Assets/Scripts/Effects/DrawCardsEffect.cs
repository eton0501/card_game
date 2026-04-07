using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 把抽牌效果轉成 DrawCardsGA，讓卡牌可以觸發抽牌行為。
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

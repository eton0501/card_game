using System.Collections.Generic;
/// <summary>
/// 所有效果類型的抽象基底，規範效果必須能轉換成可執行的 GameAction。
/// </summary>

[System.Serializable]//讓子類別可以在Unspector中被編輯

public abstract class Effect 
{
    public abstract GameAction GetGameAction(
    List<CombatantView> targets//這個效果要作用的目標清單
    ,CombatantView caster);//發動這個效果的戰鬥者
}

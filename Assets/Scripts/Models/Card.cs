using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 卡牌的執行期實例資料，保存CardData與本場戰鬥可能被調整過的費用。
/// </summary>


public class Card
{
    private readonly CardData data;

    public CardData Data => data;
    public string Title => data.name;
    public string Description => data.Description;
    public Sprite Image => data.Image;
    public Effect ManualTargetEffect => data.ManualTargetEffect;
    public List<AutoTargetEffect> OtherEffects => data.OtherEffects;
    public int Mana { get; private set; }

    public Card(CardData cardData)
        : this(cardData, cardData != null ? cardData.Mana : 0)
    {
    }

    public Card(CardData cardData, int mana)
    {
        data = cardData;
        Mana = Mathf.Max(0, mana);
    }
}

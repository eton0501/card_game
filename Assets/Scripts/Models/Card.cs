using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
/// <summary>
/// 卡牌的執行時期的物件，封裝CardData的資料
/// </summary>
public class Card
{
    private readonly CardData data;//這張牌對應的資料檔案
    public string Title=>data.name;//利用=>代表每次有人存取就會去讀一次值
    public string Description=>data.Description;
    public Sprite Image=>data.Image;
    public Effect ManualTargetEffect=>data.ManualTargetEffect;
    public List<AutoTargetEffect> OtherEffects=>data.OtherEffects;//自動目標效果清單
    public int Mana{get;private set;}
    public Card(CardData cardData)
    {
        data=cardData;
        Mana=cardData.Mana;
    }
}

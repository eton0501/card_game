using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
/// <summary>
/// 負責初始化一場戰鬥所需的所有系統
/// </summary>
public class MatchSetupSystem : MonoBehaviour
{
   [SerializeField] private HeroData heroData;//英雄的資料
   [SerializeField] private PerkData perkData;//這場戰鬥攜帶的天賦資料
   [SerializeField] private List<EnemyData> enemyDatas;//這場戰鬥出現的所有敵人資料
    private void Start()
    {
        HeroSystem.Instance.Setup(heroData);//初始化英雄系統
        EnemySystem.Instance.Setup(enemyDatas);//初始化敵人系統
        CardSystem.Instance.Setup(heroData.Deck);//初始化卡牌系統
        PerkSystem.Instance.AddPerk(new Perk(perkData));//將這場戰鬥的天賦加入天賦系統
        DrawCardsGA drawCardsGA=new(5);//建立摸五張牌的行動
        ActionSystem.Instance.Perform(drawCardsGA);//交給ActionSystem執行
    }
}

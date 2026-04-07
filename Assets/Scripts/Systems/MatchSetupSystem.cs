using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建立一場新遊戲與每次遭遇開局，負責初始化英雄、牌組、敵人與起手狀態。
/// </summary>


public class MatchSetupSystem : MonoBehaviour
{
    [SerializeField] private HeroData heroData;
    [SerializeField] private PerkData perkData;
    [SerializeField] private List<EnemyData> enemyDatas;
    [SerializeField] private bool autoStart = false;
    [Header("Encounter Difficulty Multipliers")]
    [SerializeField] private float battleHealthMultiplier = 1f;
    [SerializeField] private float battleAttackMultiplier = 1f;
    [SerializeField] private float eliteHealthMultiplier = 1.35f;
    [SerializeField] private float eliteAttackMultiplier = 1.25f;
    [SerializeField] private float bossHealthMultiplier = 1.8f;
    [SerializeField] private float bossAttackMultiplier = 1.45f;

    private bool hasSetupCore = false;

    private void Start()
    {
        if (autoStart)
        {
            SetupMatch();
        }
    }

    public void SetupMatch()
    {
        SetupEncounter(enemyDatas, MapNodeType.Battle);
    }

    public void SetupEncounter(List<EnemyData> encounterEnemies)
    {
        SetupEncounter(encounterEnemies, MapNodeType.Battle);
    }

    public void SetupEncounter(List<EnemyData> encounterEnemies, MapNodeType nodeType)
    {
        bool justSetupCore = EnsureCoreSetup();
        if (!justSetupCore)
        {
            CardSystem.Instance.StartEncounterDeck();
        }

        GetMultipliersByNodeType(nodeType, out float healthMultiplier, out float attackMultiplier);
        EnemySystem.Instance.Setup(encounterEnemies, healthMultiplier, attackMultiplier);

        RefillManaGA refillManaGA = new();
        ActionSystem.Instance.Perform(refillManaGA, () =>
        {
            DrawCardsGA drawCardsGA = new(5);
            ActionSystem.Instance.Perform(drawCardsGA);
        });
    }

    private void GetMultipliersByNodeType(
        MapNodeType nodeType,
        out float healthMultiplier,
        out float attackMultiplier
    )
    {
        switch (nodeType)
        {
            case MapNodeType.Elite:
                healthMultiplier = eliteHealthMultiplier;
                attackMultiplier = eliteAttackMultiplier;
                return;
            case MapNodeType.Boss:
                healthMultiplier = bossHealthMultiplier;
                attackMultiplier = bossAttackMultiplier;
                return;
            default:
                healthMultiplier = battleHealthMultiplier;
                attackMultiplier = battleAttackMultiplier;
                return;
        }
    }

    private bool EnsureCoreSetup()
    {
        if (hasSetupCore) return false;
        hasSetupCore = true;

        HeroSystem.Instance.Setup(heroData);
        CardSystem.Instance.Setup(heroData.Deck);
        PerkSystem.Instance.AddPerk(new Perk(perkData));
        return true;
    }
}

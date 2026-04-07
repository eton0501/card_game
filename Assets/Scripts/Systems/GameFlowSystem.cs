using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 掌管整局流程狀態：開始遊戲、地圖節點切換、戰鬥結束、獎勵/商店/休息與勝敗畫面。
/// </summary>


public class GameFlowSystem : Singleton<GameFlowSystem>
{
    [SerializeField] private GameObject gameplayRoot;
    [SerializeField] private MatchSetupSystem matchSetupSystem;
    [SerializeField] private CardRewardSystem cardRewardSystem;
    [SerializeField] private ShopSystem shopSystem;
    [SerializeField] private RestSystem restSystem;
    [SerializeField] private RunMapSystem runMapSystem;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private float restHealPercent = 0.3f;
    [Header("Combat Gold Rewards")]
    [SerializeField] private int battleGoldReward = 20;
    [SerializeField] private int eliteGoldReward = 35;
    [SerializeField] private int bossGoldReward = 60;

    private MapNodeType? activeCombatNodeType;

    public bool IsGameStarted { get; private set; } = false;
    public bool IsGameOver { get; private set; } = false;
    public bool IsGameplayActive => IsGameStarted && !IsGameOver;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        if (startPanel != null) startPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (cardRewardSystem != null) cardRewardSystem.HideReward();
        if (shopSystem != null) shopSystem.HideShop();
        if (restSystem != null) restSystem.HideRest();
        if (runMapSystem != null) runMapSystem.HideMap();
        if (gameplayRoot != null) gameplayRoot.SetActive(false);
    }

    private void Update()
    {
        if (!IsGameplayActive) return;

        if (
            HeroSystem.Instance != null &&
            HeroSystem.Instance.HeroView != null &&
            HeroSystem.Instance.HeroView.CurrentHealth <= 0
        )
        {
            EndGame(false);
            return;
        }

        if (
            EnemySystem.Instance != null &&
            EnemySystem.Instance.Enemies != null &&
            EnemySystem.Instance.Enemies.Count == 0
        )
        {
            EndGame(true);
        }
    }

    public void OnClickStartGame()
    {
        if (IsGameStarted) return;
        if (matchSetupSystem == null) return;

        IsGameStarted = true;

        if (startPanel != null) startPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (cardRewardSystem != null) cardRewardSystem.HideReward();
        if (shopSystem != null) shopSystem.HideShop();
        if (restSystem != null) restSystem.HideRest();
        if (gameplayRoot != null) gameplayRoot.SetActive(true);
        if (GoldSystem.Instance != null) GoldSystem.Instance.ResetForNewRun();

        if (runMapSystem != null)
        {
            IsGameOver = true;
            runMapSystem.ResetRun();
            runMapSystem.BeginSelection(OnMapNodeSelected, () => ShowGameOver(true));
        }
        else
        {
            IsGameOver = false;
            matchSetupSystem.SetupMatch();
        }
    }

    public void OnClickRetry()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    private void EndGame(bool playerWon)
    {
        if (IsGameOver) return;
        IsGameOver = true;

        if (!playerWon)
        {
            ShowGameOver(false);
            return;
        }

        bool shouldShowReward = activeCombatNodeType == MapNodeType.Battle ||
                                activeCombatNodeType == MapNodeType.Elite ||
                                activeCombatNodeType == MapNodeType.Boss;
        GrantCombatGold();

        if (shouldShowReward && cardRewardSystem != null)
        {
            cardRewardSystem.ShowReward(AdvanceMapAfterNodeResolved);
        }
        else
        {
            AdvanceMapAfterNodeResolved();
        }
    }

    private void OnMapNodeSelected(MapNodeData nodeData)
    {
        if (nodeData == null)
        {
            ShowGameOver(true);
            return;
        }

        switch (nodeData.NodeType)
        {
            case MapNodeType.Battle:
            case MapNodeType.Elite:
            case MapNodeType.Boss:
            {
                if (nodeData.EnemyDatas == null)
                {
                    ShowGameOver(true);
                    return;
                }

                var validEnemies = nodeData.EnemyDatas.Where(enemy => enemy != null).ToList();
                if (validEnemies.Count == 0)
                {
                    ShowGameOver(true);
                    return;
                }

                activeCombatNodeType = nodeData.NodeType;
                matchSetupSystem.SetupEncounter(validEnemies, nodeData.NodeType);
                IsGameOver = false;
                break;
            }
            case MapNodeType.Shop:
            {
                activeCombatNodeType = null;
                IsGameOver = true;
                if (shopSystem != null)
                {
                    shopSystem.ShowShop(AdvanceMapAfterNodeResolved);
                }
                else
                {
                    AdvanceMapAfterNodeResolved();
                }
                break;
            }
            case MapNodeType.Rest:
            {
                activeCombatNodeType = null;
                IsGameOver = true;
                if (restSystem != null)
                {
                    restSystem.ShowRest(
                        AdvanceMapAfterNodeResolved,
                        ResolveRestHeal,
                        ResolveRestUpgrade
                    );
                }
                else
                {
                    ResolveRestHeal();
                    AdvanceMapAfterNodeResolved();
                }
                break;
            }
        }
    }

    private void AdvanceMapAfterNodeResolved()
    {
        if (runMapSystem == null)
        {
            ShowGameOver(true);
            return;
        }

        activeCombatNodeType = null;
        IsGameOver = true;
        runMapSystem.AdvanceAfterNodeResolved();
    }

    private void ShowGameOver(bool playerWon)
    {
        IsGameOver = true;
        if (runMapSystem != null) runMapSystem.HideMap();
        if (shopSystem != null) shopSystem.HideShop();
        if (restSystem != null) restSystem.HideRest();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverText != null)
        {
            gameOverText.text = playerWon ? "Victory!" : "Defeat!";
        }
    }

    private void HealHeroByPercent(float percent)
    {
        if (HeroSystem.Instance == null || HeroSystem.Instance.HeroView == null) return;
        HeroSystem.Instance.HeroView.HealByPercent(percent);
    }

    private void ResolveRestHeal()
    {
        HealHeroByPercent(restHealPercent);
    }

    private void ResolveRestUpgrade()
    {
        if (CardSystem.Instance == null)
        {
            ResolveRestHeal();
            return;
        }

        bool upgraded = CardSystem.Instance.TryUpgradeRandomCardMana(out CardData upgradedCardData);
        if (!upgraded)
        {
            // Fallback so rest node never feels wasted.
            HealHeroByPercent(restHealPercent * 0.5f);
            return;
        }

        if (upgradedCardData != null)
        {
        }
    }

    private void GrantCombatGold()
    {
        if (GoldSystem.Instance == null) return;
        if (activeCombatNodeType == null) return;

        int reward = activeCombatNodeType switch
        {
            MapNodeType.Battle => battleGoldReward,
            MapNodeType.Elite => eliteGoldReward,
            MapNodeType.Boss => bossGoldReward,
            _ => 0
        };

        if (reward > 0)
        {
            GoldSystem.Instance.AddGold(reward);
        }
    }
}

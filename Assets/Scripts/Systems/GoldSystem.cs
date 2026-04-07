using UnityEngine;

/// <summary>
/// 管理初始金幣，並在金幣變動時通知 UI 更新。
/// </summary>


public class GoldSystem : Singleton<GoldSystem>
{
    [SerializeField] private GoldUI goldUI;
    [SerializeField] private int startingGold = 99;

    public int CurrentGold { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        CurrentGold = Mathf.Max(0, startingGold);
        RefreshUI();
    }

    public void ResetForNewRun()
    {
        CurrentGold = Mathf.Max(0, startingGold);
        RefreshUI();
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        CurrentGold += amount;
        RefreshUI();
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (CurrentGold < amount) return false;

        CurrentGold -= amount;
        RefreshUI();
        return true;
    }

    private void RefreshUI()
    {
        if (goldUI != null)
        {
            goldUI.SetGold(CurrentGold);
        }
    }
}

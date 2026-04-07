using UnityEngine;

/// <summary>
/// 處理結束回合按鈕點擊，通知ActionSystem進入敵人回合。
/// </summary>


public class EndTurnButtonUI : MonoBehaviour
{
    public void OnClick()
    {
        if (GameFlowSystem.Instance != null && !GameFlowSystem.Instance.IsGameplayActive) return;

        EnemyTurnGA enemyTurnGA = new();
        ActionSystem.Instance.Perform(enemyTurnGA);
    }
}

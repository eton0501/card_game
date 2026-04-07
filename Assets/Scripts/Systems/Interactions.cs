/// <summary>
/// 集中管理目前玩家是否可操作(可互動/可懸停/是否拖曳中)等互動狀態。
/// </summary>


public class Interactions : Singleton<Interactions>
{
    public bool PlayerIsDragging { get; set; } = false;

    public bool PlayerCanInteract()
    {
        if (GameFlowSystem.Instance != null && !GameFlowSystem.Instance.IsGameplayActive) return false;
        if (ActionSystem.Instance.IsPerforming) return false;
        return true;
    }

    public bool PlayerCanHover()
    {
        if (GameFlowSystem.Instance != null && !GameFlowSystem.Instance.IsGameplayActive) return false;
        if (PlayerIsDragging) return false;
        return true;
    }
}

using System.Collections;
using UnityEngine;
/// <summary>
/// 管理玩家的法力系統
/// </summary>
public class ManaSystem : Singleton<ManaSystem>
{
    [SerializeField] private ManaUI manaUI;//顯示法力的UI元件
    private const int MAX_MANA=3;//法力上限
    private int currentMana=MAX_MANA;//初始為滿法力
    void OnEnable()//啟用時向ActionSystem註冊法力相關行動的執行者
    {
        ActionSystem.AttachPerformer<SpendManaGA>(SpendManaPerformer);//消耗法力
        ActionSystem.AttachPerformer<RefillManaGA>(RefillManaPerformer);//回滿法力
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction,ReactionTiming.POST);//訂閱EnemyTurnGA的POST反映
    }
    void OnDisable()//停用時取消註冊
    {
        ActionSystem.DetachPerformer<SpendManaGA>();
        ActionSystem.DetachPerformer<RefillManaGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction,ReactionTiming.POST);
    }
    public bool HasEnoughMana(int mana)//判斷目前法力是否足夠
    {
        return currentMana>=mana;//足夠回傳True，不夠回傳False
    }
    private IEnumerator SpendManaPerformer(SpendManaGA spendManaGA)//消耗法力的執行邏輯
    {
        currentMana-=spendManaGA.Amount;//扣除法力
        manaUI.UpdataManaText(currentMana);//更新UI
        yield return null;
    }
    private IEnumerator RefillManaPerformer(RefillManaGA refillManaGA)//回滿法力的執行邏輯
    {
        currentMana=MAX_MANA;//將法力恢復
        manaUI.UpdataManaText(currentMana);//更新UI
        yield return null;
    }
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)//敵人回合結束後觸發的反應
    {
        RefillManaGA refillManaGA=new();//建立回滿法力的行動
        ActionSystem.Instance.AddReaction(refillManaGA);//加入序列等待執行
    }
}

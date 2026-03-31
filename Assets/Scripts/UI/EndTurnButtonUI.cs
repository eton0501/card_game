using UnityEngine;
/// <summary>
/// 負責處理結束回合按鈕的事件
/// </summary>
public class EndTurnButtonUI : MonoBehaviour
{
    public void OnClick()//當玩家點擊結束回合
    {
        EnemyTurnGA enemyTurnGA=new();//建立執行敵人回合的物件
        ActionSystem.Instance.Perform(enemyTurnGA);//將行動交給ActionSystem執行
    }
}

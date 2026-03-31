using UnityEngine;
/// <summary>
/// 負責管理玩家目前的互動狀態
/// </summary>
public class Interactions : Singleton<Interactions>
{
    public bool PlayerIsDragging{get;set;}=false;//玩家是否正在拖移卡牌
    public bool PlayerCanInteract()//判斷玩家目前是否可以執行互動
    {
        if(!ActionSystem.Instance.IsPerforming)return true;//沒有行動正在執行就可以
        else return false;//有行動正在執行中就不行
    }
    public bool PlayerCanHover()//判斷玩家目前是否可以懸停預覽卡牌
    {
        if(PlayerIsDragging) return false;//如果正在拖移中就不行
        return true;//沒有在拖移就可以
    }
}   


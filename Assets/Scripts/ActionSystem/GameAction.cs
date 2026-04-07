using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定義所有遊戲行動的基底資料，提供 Pre/Perform/Post 三段反應佇列讓 ActionSystem 串接。
/// </summary>

public abstract class GameAction
{
    public List<GameAction> PreReactions{get;private set;}=new();
    public List<GameAction> PerformReactions{get;private set;}=new();
    public List<GameAction> PostReactions{get;private set;}=new();
}    

using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ActionSystem : Singleton<ActionSystem>//宣告類別，繼承自Singleton
{
    private List<GameAction> reactions=null;//定義一個只能放GameAction類別的List，變數名稱為reactions
    public bool IsPerforming{get;private set;}=false;//標記系統目前是否正在執行動作
    private static Dictionary<Type,List<Action<GameAction>>>preSubs=new();//建立一個儲存所有訂閱動作執行前要做的方式
    private static Dictionary<Type,List<Action<GameAction>>>postSubs=new();//建立一個儲存所有訂閱動作執行後要做的方式
    private static Dictionary<Type,Func<GameAction,IEnumerator>>performers=new();//儲存執行特定動作的協程函數
    public void Perform(GameAction action,System.Action OnPerformFinished = null)//外部可以呼叫這個方法開始執行一個遊戲動作，並且可傳入執行完畢後的回乎函數
    {
        if(IsPerforming)return;//如果系統在執行其他動作，則直接取消
        IsPerforming=true;//將狀態設為正在執行中
        StartCoroutine(Flow(action, () =>//啟動協程函數，並傳入匿名函數處理結束後的狀態
        {
            IsPerforming=false;//將狀態改回非執行
            OnPerformFinished?.Invoke();//如果有傳入回呼函數，就執行
        }));
    }
    public void AddReaction(GameAction gameAction)//允許在動作執行時將新的連鎖反應加入目前的排程中
    {
        reactions?.Add(gameAction);//如果目前的reactions串列存在就把新動作加進去
    }
    private IEnumerator Flow(GameAction action,Action OnFlowFinished = null)//控制動作的完整生命週期
    {
        reactions=action.PreReactions;//把當前的關注的串列切換為這個動作的執行前反應
        PerformSubscribers(action,preSubs);//觸發所有訂閱這個動作PRE階段的方式
        yield return PerformReactions();//等待所有PRE階段反應執行完畢
        
        reactions=action.PerformReactions;//把當前的關注的串列切換為這個動作的執行中反應
        yield return PerformPerformer(action);//執行這個動作的邏輯
        yield return PerformReactions();//等待這個動作執行完畢

        reactions=action.PostReactions;//把當前的關注的串列切換為這個動作的執行後反應
        PerformSubscribers(action,postSubs);//觸發所有訂閱這個動作POST階段的方式
        yield return PerformReactions();//等待所有POST階段反應執行完畢

        OnFlowFinished?.Invoke();//觸發結束的回呼函數
    }
    private IEnumerator PerformPerformer(GameAction action)//負責找出並執行此動作的核心邏輯
    {
        Type type=action.GetType();//取得傳入動作的具體型別
        if (performers.ContainsKey(type))//檢查字典裡有沒有註冊過這種型別的Performer
        {
            yield return performers[type](action);//如果有就執行
        }
    }
    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type=action.GetType();
        if (subs.ContainsKey(type))
        {
            foreach(var sub in subs[type])
            {
                sub(action);
            }
        }   
    }
    private IEnumerator PerformReactions()
    {
        foreach(var reaction in reactions)
        {
            yield return Flow(reaction);
        }
    }
    public static void AttachPerformer<T>(Func<T,IEnumerator>performer)where T : GameAction
    {
        Type type=typeof(T);
        IEnumerator wrappedPerformer(GameAction action)=>performer((T)action);
        if(performers.ContainsKey(type))performers[type]=wrappedPerformer;
        else performers.Add(type,wrappedPerformer);
    }
    public static void DetachPerformer<T>() where T: GameAction
    {
        Type type=typeof(T);
        if(performers.ContainsKey(type)) performers.Remove(type);
    }
    public static void SubscribeReaction<T>(Action<T> reaction,ReactionTiming timing)where T : GameAction
    {
        Dictionary<Type,List<Action<GameAction>>>subs=timing==ReactionTiming.PRE? preSubs:postSubs;
        void wrappedReaction(GameAction action)=>reaction((T)action);
        if (subs.ContainsKey(typeof(T)))
        {
            subs[typeof(T)].Add(wrappedReaction);
        }
        else
        {
            subs.Add(typeof(T),new());
            subs[typeof(T)].Add(wrappedReaction);
        }
    }
    public static void UnsubscribeReaction<T>(Action<T> reaction,ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type,List<Action<GameAction>>>subs=timing==ReactionTiming.PRE?preSubs:postSubs;
        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction(GameAction action)=>reaction((T)action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
    }
}








    
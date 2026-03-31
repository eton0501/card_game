using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// List的擴充方法
/// </summary>
public static class ListExtensions
{
   public static T Draw<T>(this List<T> list)//模擬真實抽牌
    {
        if(list.Count==0)return default;//如果list為空時回傳預設值
        int r=Random.Range(0,list.Count);//隨機產生一個索引值
        T t=list[r];//取得該索引對應的元素
        list.Remove(t);//將此元素從List移除
        return t;//回傳被移除的元素
    }
}

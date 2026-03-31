using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 英雄的資料容器，包含圖片、血量和初始牌組
/// </summary>
[CreateAssetMenu(menuName ="Data/Hero")]

public class HeroData : ScriptableObject
{
    [field:SerializeField] public Sprite Image{get;private set;}//英雄的圖片
    [field:SerializeField] public int Health{get; private set;}//初始血量
    [field:SerializeField] public List<CardData> Deck{get;private set;}//初始牌組
}

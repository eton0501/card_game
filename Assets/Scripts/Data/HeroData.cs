using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 儲存英雄的基礎資料，包含初始血量、圖片與起始牌組。
/// </summary>
[CreateAssetMenu(menuName ="Data/Hero")]
public class HeroData : ScriptableObject
{
    [field:SerializeField] public Sprite Image{get;private set;}//英雄的圖片
    [field:SerializeField] public int Health{get; private set;}//初始血量
    [field:SerializeField] public List<CardData> Deck{get;private set;}//初始牌組
}

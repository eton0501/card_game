using UnityEngine;
/// <summary>
/// 英雄的視覺元件
/// </summary>
public class HeroView : CombatantView
{
    public void Setup(HeroData heroData)//初始化英雄的基本資料
    {
        SetupBase(heroData.Health,heroData.Image);//將英雄血量和圖片傳給父類別的初始化
    }
}

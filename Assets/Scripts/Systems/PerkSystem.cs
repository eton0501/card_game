using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 管理玩家目前擁有的天賦清單，並同步處理UI顯示與事件訂閱
/// </summary>
public class PerkSystem : Singleton<PerkSystem>
{
    [SerializeField] private PerksUI perksUI;//天賦UI
    private readonly List<Perk> perks=new();//儲存目前已啟用的天賦實例清單
    public void AddPerk(Perk perk)//新增一個天賦到系統
    {
        perks.Add(perk);//加入清單
        perksUI.AddPerkUI(perk);//在UI上顯示
        perk.OnAdd();//啟用天賦
    }
    public void RemovePerk(Perk perk)
    {
        perks.Remove(perk);
        perksUI.RemovePerkUI(perk);
        perk.OnRemove();
    }
}

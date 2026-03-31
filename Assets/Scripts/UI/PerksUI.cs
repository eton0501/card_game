using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 管理天賦列表UI，新增天賦時建立對應的PerkUI
/// </summary>
public class PerksUI : MonoBehaviour
{
    [SerializeField] private PerkUI perkUIPrefb;//PerkUI預製體
    private readonly List<PerkUI> perkUIs=new();//儲存目前場上所有已建立的PerkUI
    public void AddPerkUI(Perk perk)//新增一個天賦UI
    {
        PerkUI perkUI=Instantiate(perkUIPrefb,transform);//在此物件底下實例化PerkUI
        perkUI.Setup(perk);//綁定該UI對應的Perk資料
        perkUIs.Add(perkUI);//加入內部清單
    }
    public void RemovePerkUI(Perk perk)//移除指定Perk對應的UI
    {
        PerkUI perkUI=perkUIs.Where(pui=>pui.Perk==perk).FirstOrDefault();//從清單找出匹配該Perk的UI
        if (perkUI != null)//如果找到
        {
            perkUIs.Remove(perkUI);//從清單中刪除
            Destroy(perkUI.gameObject);//銷毀該UI元件
        }
    }
}

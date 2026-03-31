
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 管理多個狀態效果的UI清單
/// </summary>
public class StatusEffectsUI : MonoBehaviour
{
    [SerializeField] private StatusEffectUI statusEffectUIPrefab;//單一狀態效果UI的預製體
    [SerializeField] private Sprite armorSprite,burnSprite;//各狀態對應要顯示的圖示
    private Dictionary<StatusEffectType,StatusEffectUI>statusEffectUIs=new();
    public void UpdateStatusEffectUI(StatusEffectType statusEffectType,int stackCount)//更新指定狀態的UI
    {
        if (stackCount == 0)//若層數歸零
        {
            if (statusEffectUIs.ContainsKey(statusEffectType))//確認字典裡有這個狀態UI
            {
                StatusEffectUI statusEffectUI=statusEffectUIs[statusEffectType];//取出對應UI實例
                statusEffectUIs.Remove(statusEffectType);//從字典裡移除
                Destroy(statusEffectUI.gameObject);//摧毀UI
            }
        }
        else//若層數大於0
        {
            if (!statusEffectUIs.ContainsKey(statusEffectType))//若目前還沒有此狀態的UI
            {
                StatusEffectUI statusEffectUI=Instantiate(statusEffectUIPrefab,transform);//建立新的狀態UI
                statusEffectUIs.Add(statusEffectType,statusEffectUI);//註冊到字典
            }
            Sprite sprite=GetSpriteByType(statusEffectType);//依狀態類型取得對應圖示
            statusEffectUIs[statusEffectType].Set(sprite,stackCount);//更新該狀態UI
        }
    }
    private Sprite GetSpriteByType(StatusEffectType statusEffectType)//將狀態類型轉成對應圖示
    {
        return statusEffectType switch
        {
            StatusEffectType.ARMOR=>armorSprite,
            StatusEffectType.BURN=>burnSprite,
            _=>null,
        };
    }
}

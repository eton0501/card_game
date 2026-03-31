using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
/// <summary>
/// 所有生物的基底類別
/// 提供血量管理、受傷計算、狀態效果等所有戰鬥者共用的功能
/// </summary>
public class CombatantView : MonoBehaviour
{
   [SerializeField] private TMP_Text healthText;//顯示目前血量的文字元件
   [SerializeField] private SpriteRenderer spriteRenderer;//顯示這個生物圖片的元件
   [SerializeField] private StatusEffectsUI statusEffectsUI;//顯示狀態效果的UI元件
   public int MaxHealth{get;private set;}//血量上限
   public int CurrentHealth{get;private set;}//目前血量
   private Dictionary<StatusEffectType,int>statusEffects=new();//用字典儲存目前身上所有狀態效果和對應的層數
   protected void SetupBase(int health,Sprite image)//初始化生物
    {
        MaxHealth=CurrentHealth=health;//同時設定血量上限和目前血量
        spriteRenderer.sprite=image;//設定角色圖片
        UpdataHealthText();//更新血量顯示
    }
    private void UpdataHealthText()//更新血量文字
    {
        healthText.text="HP: "+CurrentHealth;
    }
    public void Damage(int damageAmount)//對這個生物造成傷害
    {
        int remainingDamage=damageAmount;//紀錄還沒被抵銷的傷害
        int currentArmor=GetStatusEffectStacks(StatusEffectType.ARMOR);//取得目前的護甲層數
        if (currentArmor > 0)
        {
            if (currentArmor >= damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR,remainingDamage);
                remainingDamage=0;
            }
            else if (currentArmor < damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR,currentArmor);
                remainingDamage-=currentArmor;
            }
        }
        if (remainingDamage > 0)
        {
            CurrentHealth-=remainingDamage;
            if (CurrentHealth < 0)//確保血量不低於0
            {
                CurrentHealth=0;
            }
        }
        
        transform.DOShakePosition(0.2f,0.5f);//受傷時的震動動畫
        UpdataHealthText();//更新血量顯示
    }
    public void AddStatusEffect(StatusEffectType type,int stackCount)//增加指定狀態效果的層數
    {
        if (statusEffects.ContainsKey(type))
        {
            statusEffects[type]+=stackCount;//已有此效果，直接增加層數
        }
        else
        {
            statusEffects.Add(type,stackCount);//還沒有此效果就新增
        }
        statusEffectsUI.UpdateStatusEffectUI(type,GetStatusEffectStacks(type));//更新此狀態效果的顯示
    }
    public void RemoveStatusEffect(StatusEffectType type,int stackCount)//移除指定狀態效果的層數
    {
        if (statusEffects.ContainsKey(type))
        {
            statusEffects[type]-=stackCount;//扣除層數
            if (statusEffects[type] <= 0)
            {
                statusEffects.Remove(type);//層數歸零，從字典移除
            }
        }
        statusEffectsUI.UpdateStatusEffectUI(type,GetStatusEffectStacks(type));//更新UI
    }
    public int GetStatusEffectStacks(StatusEffectType type)//取得指定狀態效果目前的層數
    {
        if(statusEffects.ContainsKey(type))return statusEffects[type];
        else return 0;
    }
}

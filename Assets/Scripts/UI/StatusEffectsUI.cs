using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理角色身上的狀態圖示列表，依層數新增、更新或移除狀態UI。
/// </summary>


public class StatusEffectsUI : MonoBehaviour
{
    [SerializeField] private StatusEffectUI statusEffectUIPrefab;
    [SerializeField] private Sprite armorSprite;
    [SerializeField] private Sprite burnSprite;
    [SerializeField] private Sprite weakSprite;
    [SerializeField] private Sprite vulnerableSprite;

    private readonly Dictionary<StatusEffectType, StatusEffectUI> statusEffectUIs = new();

    public void UpdateStatusEffectUI(StatusEffectType statusEffectType, int stackCount)
    {
        if (stackCount <= 0)
        {
            if (statusEffectUIs.TryGetValue(statusEffectType, out StatusEffectUI statusEffectUI))
            {
                statusEffectUIs.Remove(statusEffectType);
                Destroy(statusEffectUI.gameObject);
            }
            return;
        }

        if (!statusEffectUIs.ContainsKey(statusEffectType))
        {
            StatusEffectUI statusEffectUI = Instantiate(statusEffectUIPrefab, transform);
            statusEffectUIs.Add(statusEffectType, statusEffectUI);
        }

        Sprite sprite = GetSpriteByType(statusEffectType);
        statusEffectUIs[statusEffectType].Set(sprite, stackCount);
    }

    private Sprite GetSpriteByType(StatusEffectType statusEffectType)
    {
        return statusEffectType switch
        {
            StatusEffectType.ARMOR => armorSprite,
            StatusEffectType.BURN => burnSprite,
            StatusEffectType.WEAK => weakSprite,
            StatusEffectType.VULNERABLE => vulnerableSprite,
            _ => null,
        };
    }
}

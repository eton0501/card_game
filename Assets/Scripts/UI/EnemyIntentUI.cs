using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 顯示敵人的下一步意圖圖示與數值。
/// </summary>


public class EnemyIntentUI : MonoBehaviour
{
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite defendSprite;
    [SerializeField] private Sprite burnSprite;

    public void SetIntent(EnemyIntentType intentType, int value)
    {
        if (valueText != null)
        {
            valueText.text = value.ToString();
        }

        if (iconImage == null) return;

        iconImage.sprite = intentType switch
        {
            EnemyIntentType.Attack => attackSprite,
            EnemyIntentType.Defend => defendSprite,
            EnemyIntentType.ApplyBurn => burnSprite,
            _ => null,
        };
    }
}

using UnityEngine.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// 顯示單一狀態圖示與層數文字。
/// </summary>


public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text stackCountText;
    public void Set(Sprite sprite,int stackCount)
    {
        image.sprite=sprite;
        stackCountText.text=stackCount.ToString();
    }
}

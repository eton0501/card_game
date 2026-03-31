using UnityEngine.UI;
using TMPro;
using UnityEngine;
/// <summary>
/// 控制單一狀態效果的UI顯示
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

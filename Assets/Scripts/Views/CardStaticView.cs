using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 在非手牌情境顯示卡面資訊，不處理拖曳互動。
/// </summary>


public class CardStaticView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text mana;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private Image imageUI;

    public void Setup(CardData data)
    {
        if (data == null)
        {
            if (title != null) title.text = "-";
            if (description != null) description.text = string.Empty;
            if (mana != null) mana.text = "-";
            if (imageSR != null) imageSR.sprite = null;
            if (imageUI != null) imageUI.sprite = null;
            return;
        }

        if (title != null) title.text = data.name;
        if (description != null) description.text = data.Description;
        if (mana != null) mana.text = data.Mana.ToString();
        if (imageSR != null) imageSR.sprite = data.Image;
        if (imageUI != null) imageUI.sprite = data.Image;
    }
}

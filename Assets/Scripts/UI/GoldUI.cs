using TMPro;
using UnityEngine;

/// <summary>
/// 把GoldSystem的目前金幣顯示在介面上。
/// </summary>


public class GoldUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private string displayFormat = "{0}";

    public void SetGold(int amount)
    {
        if (goldText == null) return;
        goldText.text = string.Format(displayFormat, amount);
    }
}

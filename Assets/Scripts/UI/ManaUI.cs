using TMPro;
using UnityEngine;

/// <summary>
/// 顯示當前法力與最大法力。
/// </summary>


public class ManaUI : MonoBehaviour
{
    [SerializeField] private TMP_Text mana;//顯示法力的文字元件
    public void UpdataManaText(int currentMana)//更新畫面上顯示的法力
    {
        mana.text=currentMana.ToString();//將整數轉為字串並更新UI
    }
}

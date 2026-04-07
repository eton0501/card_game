
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 顯示單一Perk的圖示與綁定資料。
/// </summary>


public class PerkUI : MonoBehaviour
{
    [SerializeField] private Image image;//天賦圖示元件
    public Perk Perk{get;private set;}//對外提供目前綁定的Perk
    public void Setup(Perk perk)//初始化UI
    {
        Perk=perk;
        image.sprite=perk.Image;//更新圖片
    }
}

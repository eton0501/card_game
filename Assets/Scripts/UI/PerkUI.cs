
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 控制單一天賦圖示的UI顯示，負責把Perk資料綁到畫面元件上
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

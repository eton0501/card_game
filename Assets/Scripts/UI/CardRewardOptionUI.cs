using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 顯示獎勵卡選項，負責綁定卡面資料與點擊選取觸發。
/// </summary>


public class CardRewardOptionUI : MonoBehaviour
    , IPointerClickHandler
{
    [SerializeField] private Button button;
    [SerializeField] private CardStaticView cardStaticView;

    private CardData cardData;
    private Action<CardData> onSelected;
    private bool hasSelected = false;

    public void Setup(CardData data, Action<CardData> onSelectedCallback)
    {
        cardData = data;
        onSelected = onSelectedCallback;
        hasSelected = false;

        if (cardStaticView != null)
        {
            cardStaticView.Setup(data);
        }

        if (button == null)
        {
            return;
        }
        button.onClick.RemoveAllListeners();
        button.interactable = data != null;
        if (data != null)
        {
            button.onClick.AddListener(TrySelect);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        TrySelect();
    }

    private void TrySelect()
    {
        if (hasSelected) return;
        if (cardData == null) return;
        hasSelected = true;
        onSelected?.Invoke(cardData);
    }
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制商店節點流程，隨機產生卡牌、判斷金幣是否足夠。
/// </summary>


public class ShopSystem : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private Button leaveButton;
    [SerializeField] private List<CardData> shopPool = new();
    [SerializeField] private List<ShopOptionUI> optionUIs = new();
    [SerializeField] private int defaultShopCost = 45;

    private Action onShopClosed;

    private void Awake()
    {
        if (leaveButton != null)
        {
            leaveButton.onClick.RemoveAllListeners();
            leaveButton.onClick.AddListener(CloseShop);
        }

        HideShop();
    }

    public void ShowShop(Action onClosedCallback)
    {
        onShopClosed = onClosedCallback;

        if (shopPanel == null)
        {
            onShopClosed?.Invoke();
            return;
        }

        if (headerText != null)
        {
            headerText.text = "Shop";
        }

        SetHintText("Choose cards to buy.");
        shopPanel.SetActive(true);

        if (shopPool == null || shopPool.Count == 0)
        {
            SetHintText("Shop pool is empty.");
        }

        List<CardData> offers = GetRandomOffers(optionUIs.Count);
        for (int i = 0; i < optionUIs.Count; i++)
        {
            ShopOptionUI optionUI = optionUIs[i];
            if (optionUI == null) continue;

            CardData cardData = i < offers.Count ? offers[i] : null;
            int price = cardData == null ? 0 : GetPrice(cardData);
            optionUI.Setup(cardData, price, OnClickBuyOption);
            optionUI.gameObject.SetActive(cardData != null);
        }
    }

    public void HideShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    private void OnClickBuyOption(ShopOptionUI optionUI)
    {
        if (optionUI == null || optionUI.CardData == null || optionUI.IsSold) return;

        if (CardSystem.Instance == null)
        {
            SetHintText("CardSystem is missing.");
            return;
        }

        if (GoldSystem.Instance == null)
        {
            SetHintText("GoldSystem is missing.");
            return;
        }

        bool hasBought = GoldSystem.Instance.TrySpend(optionUI.Price);
        if (!hasBought)
        {
            SetHintText("Not enough gold.");
            return;
        }

        CardSystem.Instance.AddCardToDeck(optionUI.CardData);
        optionUI.MarkSold();
        SetHintText($"Bought {optionUI.CardData.name}.");
    }

    private void CloseShop()
    {
        HideShop();
        onShopClosed?.Invoke();
    }

    private void SetHintText(string text)
    {
        if (hintText != null)
        {
            hintText.text = text;
        }
    }

    private int GetPrice(CardData cardData)
    {
        if (cardData == null) return 0;
        if (cardData.ShopCost > 0) return cardData.ShopCost;
        return defaultShopCost;
    }

    private List<CardData> GetRandomOffers(int count)
    {
        List<CardData> pool = new();
        foreach (CardData cardData in shopPool)
        {
            if (cardData == null) continue;
            pool.Add(cardData);
        }

        List<CardData> result = new();
        int offerCount = Mathf.Min(count, pool.Count);
        for (int i = 0; i < offerCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, pool.Count);
            result.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }
        return result;
    }
}

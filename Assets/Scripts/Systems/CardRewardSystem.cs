using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 控制三選一獎勵流程，隨機抽選候選卡並把選中的卡加入牌組。
/// </summary>


public class CardRewardSystem : MonoBehaviour
{
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private List<CardData> rewardPool = new();
    [SerializeField] private List<CardRewardOptionUI> optionUIs = new();

    private Action onRewardChosen;

    private void Awake()
    {
        HideReward();
    }

    public void ShowReward(Action onRewardChosenCallback)
    {
        onRewardChosen = onRewardChosenCallback;

        if (rewardPanel == null)
        {
            onRewardChosen?.Invoke();
            return;
        }

        if (rewardPool == null || rewardPool.Count == 0 || optionUIs.Count == 0)
        {
            rewardPanel.SetActive(false);
            onRewardChosen?.Invoke();
            return;
        }

        if (headerText != null)
        {
            headerText.text = "Choose a Card";
        }

        rewardPanel.SetActive(true);
        List<CardData> rewards = GetRandomRewards(3);

        for (int i = 0; i < optionUIs.Count; i++)
        {
            CardData option = i < rewards.Count ? rewards[i] : null;
            optionUIs[i].Setup(option, OnSelectRewardCard);
        }
    }

    public void HideReward()
    {
        if (rewardPanel != null) rewardPanel.SetActive(false);
    }

    private void OnSelectRewardCard(CardData cardData)
    {
        CardSystem.Instance.AddCardToDeck(cardData);
        HideReward();
        onRewardChosen?.Invoke();
    }

    private List<CardData> GetRandomRewards(int count)
    {
        List<CardData> pool = new(rewardPool);
        List<CardData> result = new();

        int rewardCount = Mathf.Min(count, pool.Count);
        for (int i = 0; i < rewardCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, pool.Count);
            result.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        return result;
    }
}

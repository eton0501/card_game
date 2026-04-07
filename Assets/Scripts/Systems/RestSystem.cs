using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制休息節點面板。
/// </summary>


public class RestSystem : MonoBehaviour
{
    [SerializeField] private GameObject restPanel;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private Button healButton;
    [SerializeField] private Button upgradeButton;

    private Action onClosed;
    private Action onChooseHeal;
    private Action onChooseUpgrade;
    private bool hasResolved;

    private void Awake()
    {
        HideRest();
    }

    public void ShowRest(Action onClosedCallback, Action onHealCallback, Action onUpgradeCallback)
    {
        onClosed = onClosedCallback;
        onChooseHeal = onHealCallback;
        onChooseUpgrade = onUpgradeCallback;
        hasResolved = false;

        if (restPanel == null)
        {
            onClosed?.Invoke();
            return;
        }

        if (headerText != null) headerText.text = "Rest Site";
        if (hintText != null) hintText.text = "Choose one option.";

        if (healButton != null)
        {
            healButton.onClick.RemoveAllListeners();
            healButton.onClick.AddListener(ChooseHeal);
            healButton.interactable = true;
        }

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(ChooseUpgrade);
            upgradeButton.interactable = true;
        }

        restPanel.SetActive(true);
    }

    public void HideRest()
    {
        if (restPanel != null)
        {
            restPanel.SetActive(false);
        }
    }

    public void SetHint(string text)
    {
        if (hintText != null)
        {
            hintText.text = text;
        }
    }

    private void ChooseHeal()
    {
        if (hasResolved) return;
        hasResolved = true;
        onChooseHeal?.Invoke();
        CloseRest();
    }

    private void ChooseUpgrade()
    {
        if (hasResolved) return;
        hasResolved = true;
        onChooseUpgrade?.Invoke();
        CloseRest();
    }

    private void CloseRest()
    {
        HideRest();
        onClosed?.Invoke();
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制地圖節點按鈕呈現。
/// </summary>


public class MapOptionButtonUI : MonoBehaviour
{
    [SerializeField] private string nodeId;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private Sprite battleSprite;
    [SerializeField] private Sprite eliteSprite;
    [SerializeField] private Sprite shopSprite;
    [SerializeField] private Sprite restSprite;
    [SerializeField] private Sprite bossSprite;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField, Range(0f, 1f)] private float unlockedAlpha = 1f;
    [SerializeField, Range(0f, 1f)] private float lockedAlpha = 0.35f;
    [SerializeField, Range(0f, 1f)] private float completedAlpha = 0.6f;

    public string NodeId => nodeId;
    public RectTransform RectTransform => transform as RectTransform;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    public void Setup(
        MapNodeData node,
        bool isUnlocked,
        bool isCompleted,
        Action<MapNodeData> onSelected
    )
    {
        bool exists = node != null;
        gameObject.SetActive(exists);
        if (!exists || button == null) return;

        if (titleText != null)
        {
            titleText.text = string.IsNullOrWhiteSpace(node.Title) ? node.NodeType.ToString() : node.Title;
        }

        if (subtitleText != null)
        {
            subtitleText.text = node.NodeType.ToString();
        }

        if (iconImage != null)
        {
            iconImage.sprite = GetSpriteByType(node.NodeType);
        }

        if (lockOverlay != null)
        {
            lockOverlay.SetActive(!isUnlocked && !isCompleted);
        }

        if (canvasGroup != null)
        {
            if (isUnlocked)
            {
                canvasGroup.alpha = unlockedAlpha;
            }
            else if (isCompleted)
            {
                canvasGroup.alpha = completedAlpha;
            }
            else
            {
                canvasGroup.alpha = lockedAlpha;
            }
        }

        SetTextAlpha(titleText, isUnlocked ? 1f : 0.75f);
        SetTextAlpha(subtitleText, isUnlocked ? 1f : 0.75f);

        button.onClick.RemoveAllListeners();
        button.interactable = isUnlocked && !isCompleted;
        if (button.interactable)
        {
            button.onClick.AddListener(() => onSelected?.Invoke(node));
        }
    }

    private void SetTextAlpha(TMP_Text text, float alpha)
    {
        if (text == null) return;
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }

    private Sprite GetSpriteByType(MapNodeType nodeType)
    {
        return nodeType switch
        {
            MapNodeType.Battle => battleSprite,
            MapNodeType.Elite => eliteSprite,
            MapNodeType.Shop => shopSprite,
            MapNodeType.Rest => restSprite,
            MapNodeType.Boss => bossSprite,
            _ => battleSprite,
        };
    }
}

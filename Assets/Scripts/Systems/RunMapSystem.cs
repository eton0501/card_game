using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理路線地圖顯示、節點解鎖、連線繪製。
/// </summary>


public class RunMapSystem : MonoBehaviour
{
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private TMP_Text floorTitleText;
    [SerializeField] private string mapTitle = "Map";
    [SerializeField] private List<MapOptionButtonUI> nodeButtons = new();
    [SerializeField] private List<MapNodeData> nodes = new();
    [Header("Connections")]
    [SerializeField] private RectTransform connectionsRoot;
    [SerializeField] private Image connectionLinePrefab;
    [SerializeField] private bool forceSolidRuntimeLineSprite = true;
    [SerializeField] private bool useHighContrastConnectionColors = true;
    [SerializeField] private float connectionThickness = 5f;
    [SerializeField] private float fromNodePadding = 18f;
    [SerializeField] private float toNodePadding = 22f;
    [SerializeField] private float arrowInset = 8f;
    [SerializeField] private float arrowSize = 18f;
    [SerializeField] private string arrowChildName = "ArrowHead";
    [SerializeField] private Color lockedConnectionColor = new(1f, 1f, 1f, 0.15f);
    [SerializeField] private Color previewConnectionColor = new(1f, 1f, 1f, 0.35f);
    [SerializeField] private Color activeConnectionColor = new(1f, 1f, 1f, 0.9f);
    [SerializeField] private Color completedConnectionColor = new(1f, 1f, 1f, 0.5f);

    private readonly HashSet<string> completedNodeIds = new();
    private readonly HashSet<string> unlockedNodeIds = new();
    private readonly Dictionary<string, MapNodeData> nodesById = new();
    private readonly Dictionary<string, MapOptionButtonUI> buttonsById = new();
    private readonly Dictionary<string, ConnectionViewData> connectionViews = new();

    private Action<MapNodeData> onNodeSelected;
    private Action onRunCompleted;
    private static Sprite runtimeSolidLineSprite;

    private class ConnectionViewData
    {
        public string FromNodeId;
        public string ToNodeId;
        public Image LineImage;
    }

    private void Awake()
    {
        RebuildNodeLookup();
        RebuildButtonLookup();
        BuildConnectionViews();
    }

    public void ResetRun()
    {
        RebuildNodeLookup();
        RebuildButtonLookup();
        BuildConnectionViews();
        completedNodeIds.Clear();
        unlockedNodeIds.Clear();

        List<MapNodeData> startNodes = nodes.Where(node => node != null && node.IsStartNode).ToList();
        if (startNodes.Count > 0)
        {
            foreach (MapNodeData node in startNodes)
            {
                unlockedNodeIds.Add(node.Id);
            }
        }
        else if (nodes.Count > 0 && nodes[0] != null)
        {
            unlockedNodeIds.Add(nodes[0].Id);
        }

        HideMap();
    }

    public void BeginSelection(Action<MapNodeData> onSelected, Action onCompleted)
    {
        onNodeSelected = onSelected;
        onRunCompleted = onCompleted;
        ShowSelection();
    }

    public void HideMap()
    {
        if (mapPanel != null)
        {
            mapPanel.SetActive(false);
        }
    }

    public void AdvanceAfterNodeResolved()
    {
        if (unlockedNodeIds.Count == 0)
        {
            HideMap();
            onRunCompleted?.Invoke();
            return;
        }

        ShowSelection();
    }

    private void ShowSelection()
    {
        RebuildButtonLookup();

        if (floorTitleText != null)
        {
            floorTitleText.text = mapTitle;
        }

        if (mapPanel != null)
        {
            mapPanel.SetActive(true);
        }

        
        Canvas.ForceUpdateCanvases();
        if (connectionsRoot != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(connectionsRoot);
        }

        BuildConnectionViews();

        foreach (MapOptionButtonUI buttonView in nodeButtons)
        {
            if (buttonView == null) continue;

            nodesById.TryGetValue(buttonView.NodeId, out MapNodeData nodeData);

            bool isUnlocked = nodeData != null && unlockedNodeIds.Contains(nodeData.Id);
            bool isCompleted = nodeData != null && completedNodeIds.Contains(nodeData.Id);
            buttonView.Setup(nodeData, isUnlocked, isCompleted, OnNodeClicked);
        }

        UpdateConnectionColors();
        RefreshConnectionPositions();
    }

    private void OnNodeClicked(MapNodeData node)
    {
        if (node == null) return;
        if (!unlockedNodeIds.Contains(node.Id)) return;

        HideMap();
        completedNodeIds.Add(node.Id);

        unlockedNodeIds.Clear();
        if (node.NextNodeIds != null)
        {
            foreach (string nextNodeId in node.NextNodeIds)
            {
                if (string.IsNullOrWhiteSpace(nextNodeId)) continue;
                if (!nodesById.ContainsKey(nextNodeId)) continue;
                if (completedNodeIds.Contains(nextNodeId)) continue;
                unlockedNodeIds.Add(nextNodeId);
            }
        }

        onNodeSelected?.Invoke(node);
    }

    private void RebuildNodeLookup()
    {
        nodesById.Clear();
        foreach (MapNodeData node in nodes)
        {
            if (node == null) continue;
            if (string.IsNullOrWhiteSpace(node.Id)) continue;
            if (nodesById.ContainsKey(node.Id)) continue;
            nodesById.Add(node.Id, node);
        }
    }

    private void RebuildButtonLookup()
    {
        buttonsById.Clear();

        foreach (MapOptionButtonUI button in nodeButtons)
        {
            if (button == null) continue;
            if (string.IsNullOrWhiteSpace(button.NodeId)) continue;
            if (buttonsById.ContainsKey(button.NodeId)) continue;

            buttonsById.Add(button.NodeId, button);
        }
    }

    private void BuildConnectionViews()
    {
        ClearConnectionViews();

        if (connectionsRoot == null || connectionLinePrefab == null) return;

        foreach (MapNodeData fromNode in nodes)
        {
            if (fromNode == null) continue;
            if (string.IsNullOrWhiteSpace(fromNode.Id)) continue;
            if (!buttonsById.TryGetValue(fromNode.Id, out MapOptionButtonUI fromButton)) continue;
            if (fromNode.NextNodeIds == null) continue;

            foreach (string toNodeId in fromNode.NextNodeIds)
            {
                if (string.IsNullOrWhiteSpace(toNodeId)) continue;
                if (!buttonsById.TryGetValue(toNodeId, out MapOptionButtonUI toButton)) continue;

                string key = GetConnectionKey(fromNode.Id, toNodeId);
                if (connectionViews.ContainsKey(key)) continue;

                Image lineImage = Instantiate(connectionLinePrefab, connectionsRoot);
                lineImage.gameObject.name = $"Conn_{fromNode.Id}_to_{toNodeId}";
                lineImage.raycastTarget = false;
                lineImage.enabled = true;

                if (forceSolidRuntimeLineSprite)
                {
                    lineImage.sprite = GetOrCreateRuntimeSolidLineSprite();
                    lineImage.type = Image.Type.Simple;
                    lineImage.preserveAspect = false;
                }

                lineImage.transform.SetAsFirstSibling();

                PositionConnectionLine(lineImage.rectTransform, fromButton.RectTransform, toButton.RectTransform);

                connectionViews.Add(key, new ConnectionViewData
                {
                    FromNodeId = fromNode.Id,
                    ToNodeId = toNodeId,
                    LineImage = lineImage
                });
            }
        }
    }

    private void ClearConnectionViews()
    {
        foreach (ConnectionViewData connection in connectionViews.Values)
        {
            if (connection?.LineImage == null) continue;
            Destroy(connection.LineImage.gameObject);
        }
        connectionViews.Clear();
    }

    private void UpdateConnectionColors()
    {
        Color lockedColor = lockedConnectionColor;
        Color previewColor = previewConnectionColor;
        Color activeColor = activeConnectionColor;
        Color completedColor = completedConnectionColor;

        if (useHighContrastConnectionColors)
        {
            // Softer parchment-friendly colors.
            lockedColor = new Color(0.18f, 0.12f, 0.06f, 0.25f);
            previewColor = new Color(0.26f, 0.18f, 0.10f, 0.45f);
            activeColor = new Color(0.38f, 0.26f, 0.13f, 0.75f);
            completedColor = new Color(0.22f, 0.15f, 0.08f, 0.4f);
        }

        foreach (ConnectionViewData connection in connectionViews.Values)
        {
            if (connection?.LineImage == null) continue;

            bool fromCompleted = completedNodeIds.Contains(connection.FromNodeId);
            bool toCompleted = completedNodeIds.Contains(connection.ToNodeId);
            bool fromUnlocked = unlockedNodeIds.Contains(connection.FromNodeId);
            bool toUnlocked = unlockedNodeIds.Contains(connection.ToNodeId);

            Color colorToUse = lockedColor;
            if (fromCompleted && toUnlocked)
            {
                colorToUse = activeColor;
            }
            else if (fromCompleted && toCompleted)
            {
                colorToUse = completedColor;
            }
            else if (fromUnlocked && !fromCompleted)
            {
                colorToUse = previewColor;
            }

            connection.LineImage.color = colorToUse;
        }
    }

    private void RefreshConnectionPositions()
    {
        foreach (ConnectionViewData connection in connectionViews.Values)
        {
            if (connection?.LineImage == null) continue;
            if (!buttonsById.TryGetValue(connection.FromNodeId, out MapOptionButtonUI fromButton)) continue;
            if (!buttonsById.TryGetValue(connection.ToNodeId, out MapOptionButtonUI toButton)) continue;

            PositionConnectionLine(
                connection.LineImage.rectTransform,
                fromButton.RectTransform,
                toButton.RectTransform
            );
        }
    }

    private void PositionConnectionLine(RectTransform lineRect, RectTransform fromRect, RectTransform toRect)
    {
        if (lineRect == null || fromRect == null || toRect == null || connectionsRoot == null) return;

        Vector2 from = GetCenterPointInConnectionsRoot(fromRect);
        Vector2 to = GetCenterPointInConnectionsRoot(toRect);
        Vector2 delta = to - from;
        float distance = delta.magnitude;
        if (distance < 0.001f) return;

        Vector2 direction = delta / distance;
        float fromRadius = GetApproxRectRadius(fromRect);
        float toRadius = GetApproxRectRadius(toRect);

        Vector2 start = from + direction * (fromRadius + fromNodePadding);
        Vector2 end = to - direction * (toRadius + toNodePadding);
        Vector2 clippedDelta = end - start;
        float length = clippedDelta.magnitude;
        if (length < 4f) return;

        float thickness = Mathf.Max(3f, connectionThickness);

        lineRect.anchorMin = new Vector2(0.5f, 0.5f);
        lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        lineRect.pivot = new Vector2(0.5f, 0.5f);
        lineRect.localScale = Vector3.one;
        lineRect.anchoredPosition = (start + end) * 0.5f;
        lineRect.sizeDelta = new Vector2(length, thickness);
        lineRect.localRotation = Quaternion.Euler(
            0f,
            0f,
            Mathf.Atan2(clippedDelta.y, clippedDelta.x) * Mathf.Rad2Deg
        );

        SetupArrowVisual(lineRect, length);
    }

    private Vector2 GetCenterPointInConnectionsRoot(RectTransform target)
    {
        Vector3 worldCenter = target.TransformPoint(target.rect.center);
        return connectionsRoot.InverseTransformPoint(worldCenter);
    }

    private string GetConnectionKey(string fromNodeId, string toNodeId)
    {
        return $"{fromNodeId}->{toNodeId}";
    }

    private float GetApproxRectRadius(RectTransform rect)
    {
        if (rect == null) return 0f;
        float halfW = rect.rect.width * 0.5f;
        float halfH = rect.rect.height * 0.5f;
        return Mathf.Max(halfW, halfH);
    }

    private void SetupArrowVisual(RectTransform lineRect, float lineLength)
    {
        if (lineRect == null || string.IsNullOrWhiteSpace(arrowChildName)) return;

        Transform arrowTransform = lineRect.Find(arrowChildName);
        if (arrowTransform == null) return;
        if (arrowTransform is not RectTransform arrowRect) return;

        float safeArrowSize = Mathf.Max(8f, arrowSize);
        arrowRect.anchorMin = new Vector2(1f, 0.5f);
        arrowRect.anchorMax = new Vector2(1f, 0.5f);
        arrowRect.pivot = new Vector2(0.5f, 0.5f);
        arrowRect.sizeDelta = new Vector2(safeArrowSize, safeArrowSize);
        arrowRect.anchoredPosition = new Vector2(-Mathf.Max(2f, arrowInset), 0f);
        arrowRect.localRotation = Quaternion.identity;

        arrowRect.gameObject.SetActive(lineLength > safeArrowSize + 8f);
    }

    private Sprite GetOrCreateRuntimeSolidLineSprite()
    {
        if (runtimeSolidLineSprite != null)
        {
            return runtimeSolidLineSprite;
        }

        Texture2D whiteTexture = Texture2D.whiteTexture;
        runtimeSolidLineSprite = Sprite.Create(
            whiteTexture,
            new Rect(0f, 0f, whiteTexture.width, whiteTexture.height),
            new Vector2(0.5f, 0.5f),
            100f
        );
        return runtimeSolidLineSprite;
    }
}

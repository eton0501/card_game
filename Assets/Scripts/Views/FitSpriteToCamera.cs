using UnityEngine;

/// <summary>
/// 自動把背景Sprite縮放到攝影機可視範圍。
/// </summary>

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]

public class FitSpriteToCamera : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool coverScreen = true;
    [SerializeField] private bool refreshEveryFrame = true;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        RefreshScale();
    }

    private void LateUpdate()
    {
        if (!refreshEveryFrame) return;
        RefreshScale();
    }

    private void OnValidate()
    {
        if (!isActiveAndEnabled) return;
        RefreshScale();
    }

    public void RefreshScale()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null) return;
        if (!cam.orthographic) return;

        float worldHeight = cam.orthographicSize * 2f;
        float worldWidth = worldHeight * cam.aspect;

        Vector2 spriteWorldSize = spriteRenderer.sprite.bounds.size;
        if (spriteWorldSize.x <= 0f || spriteWorldSize.y <= 0f) return;

        float scaleX = worldWidth / spriteWorldSize.x;
        float scaleY = worldHeight / spriteWorldSize.y;
        float finalScale = coverScreen ? Mathf.Max(scaleX, scaleY) : Mathf.Min(scaleX, scaleY);

        transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}

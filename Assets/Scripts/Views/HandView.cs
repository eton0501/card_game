using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// 控制手牌沿曲線排版、滑鼠懸停放大。
/// </summary>


public class HandView : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float baseCardSpacing = 0.1f;
    [SerializeField] private float minCardSpacing = 0.055f;
    [SerializeField] private float depthStep = 0.01f;
    [SerializeField] private Vector3 handWorldOffset = Vector3.zero;
    [SerializeField, Range(0f, 0.45f)] private float minSplinePosition = 0.18f;
    [SerializeField, Range(0.55f, 1f)] private float maxSplinePosition = 0.82f;
    [SerializeField] private int spacingTightenStartCount = 5;
    [SerializeField] private float spacingReducePerCard = 0.005f;
    [SerializeField] private int crowdLiftStartCount = 6;
    [SerializeField] private float crowdLiftPerCard = 0.045f;
    [SerializeField] private float maxCrowdLift = 0.22f;
    [SerializeField] private int flattenRotationStartCount = 7;
    [SerializeField, Range(0f, 1f)] private float maxRotationFlatten = 0.35f;

    [Header("Hover Feel")]
    [SerializeField] private float hoverLift = 0.85f;
    [SerializeField] private float hoverScale = 1.18f;
    [SerializeField] private float hoverDepthBoost = 0.45f;
    [SerializeField] private float maxHoverWorldY = -0.15f;
    [SerializeField] private float neighborPushOnSpline = 0.03f;
    [SerializeField] private float hoverLayoutDuration = 0.12f;

    [Header("Default Timing")]
    [SerializeField] private float defaultLayoutDuration = 0.15f;

    private readonly List<CardView> cards = new();
    private CardView hoveredCard;
    private CardView draggingCard;

    public IEnumerator AddCard(CardView cardView)
    {
        if (cardView == null) yield break;

        cardView.SetOwnerHand(this);
        cards.Add(cardView);
        yield return UpdateCardPositions(defaultLayoutDuration);
    }

    public CardView RemoveCard(Card card)
    {
        CardView cardView = GetCardView(card);
        if (cardView == null) return null;

        if (hoveredCard == cardView) hoveredCard = null;
        if (draggingCard == cardView) draggingCard = null;

        cards.Remove(cardView);
        StartCoroutine(UpdateCardPositions(defaultLayoutDuration));
        return cardView;
    }

    public void OnCardHoverEnter(CardView cardView)
    {
        if (cardView == null) return;
        if (!cards.Contains(cardView)) return;
        if (draggingCard != null) return;
        if (hoveredCard == cardView) return;

        hoveredCard = cardView;
        StartCoroutine(UpdateCardPositions(hoverLayoutDuration));
    }

    public void OnCardHoverExit(CardView cardView)
    {
        if (hoveredCard != cardView) return;

        hoveredCard = null;
        StartCoroutine(UpdateCardPositions(hoverLayoutDuration));
    }

    public void ClearHover()
    {
        if (hoveredCard == null) return;
        hoveredCard = null;
        StartCoroutine(UpdateCardPositions(hoverLayoutDuration));
    }

    public void OnCardDragStart(CardView cardView)
    {
        draggingCard = cardView;
        if (hoveredCard == cardView)
        {
            hoveredCard = null;
        }

        StartCoroutine(UpdateCardPositions(hoverLayoutDuration));
    }

    public void OnCardDragEnd(CardView cardView)
    {
        if (draggingCard != cardView) return;

        draggingCard = null;
        StartCoroutine(UpdateCardPositions(hoverLayoutDuration));
    }

    private CardView GetCardView(Card card)
    {
        return cards.Where(view => view.Card == card).FirstOrDefault();
    }

    private IEnumerator UpdateCardPositions(float duration)
    {
        if (cards.Count == 0) yield break;
        if (splineContainer == null) yield break;

        int cardCount = cards.Count;
        float availableSplineSpan = Mathf.Max(0f, maxSplinePosition - minSplinePosition);
        float wantedSpacing = Mathf.Clamp(
            baseCardSpacing - Mathf.Max(0, cardCount - spacingTightenStartCount) * spacingReducePerCard,
            minCardSpacing,
            baseCardSpacing
        );

        float maxSpacingAllowedByBounds = cardCount <= 1 ? wantedSpacing : availableSplineSpan / (cardCount - 1);
        float dynamicSpacing = Mathf.Min(wantedSpacing, maxSpacingAllowedByBounds);

        float totalSpan = (cardCount - 1) * dynamicSpacing;
        float firstCardPosition = 0.5f - totalSpan / 2f;
        firstCardPosition = Mathf.Clamp(firstCardPosition, minSplinePosition, maxSplinePosition - totalSpan);

        int crowdedCards = Mathf.Max(0, cardCount - crowdLiftStartCount);
        float crowdLift = Mathf.Min(maxCrowdLift, crowdedCards * crowdLiftPerCard);
        float flattenFactor = Mathf.Clamp01((cardCount - flattenRotationStartCount) / 5f) * maxRotationFlatten;

        Spline spline = splineContainer.Spline;

        int hoveredIndex = hoveredCard != null ? cards.IndexOf(hoveredCard) : -1;

        for (int i = 0; i < cardCount; i++)
        {
            CardView cardView = cards[i];
            if (cardView == null) continue;

            float p = firstCardPosition + i * dynamicSpacing;
            if (hoveredIndex >= 0)
            {
                if (i < hoveredIndex) p -= neighborPushOnSpline;
                else if (i > hoveredIndex) p += neighborPushOnSpline;
            }
            p = Mathf.Clamp(p, minSplinePosition, maxSplinePosition);

            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(
                -up,
                Vector3.Cross(-up, forward).normalized
            );

            Vector3 targetPosition =
                splinePosition +
                transform.position +
                handWorldOffset +
                depthStep * i * Vector3.forward;
            targetPosition.y += crowdLift;
            Quaternion targetRotation = rotation;
            float targetScale = 1f;
            int targetSortingOrder = i;

            if (cardView == draggingCard)
            {
                targetSortingOrder = cards.Count + 200;
                cardView.SetSortingOrder(targetSortingOrder);
                continue;
            }

            if (i == hoveredIndex)
            {
                float liftedY = targetPosition.y + hoverLift;
                targetPosition.y = Mathf.Min(liftedY, maxHoverWorldY);
                targetPosition += Vector3.forward * hoverDepthBoost;
                targetRotation = Quaternion.identity;
                targetScale = hoverScale;
                targetSortingOrder = cards.Count + 100;
            }
            else if (flattenFactor > 0f)
            {
                targetRotation = Quaternion.Slerp(targetRotation, Quaternion.identity, flattenFactor);
            }

            cardView.transform.DOKill();
            cardView.transform.DOMove(targetPosition, duration).SetEase(Ease.OutQuad);
            cardView.transform.DORotate(targetRotation.eulerAngles, duration).SetEase(Ease.OutQuad);
            cardView.transform.DOScale(Vector3.one * targetScale, duration).SetEase(Ease.OutQuad);
            cardView.SetSortingOrder(targetSortingOrder);
        }

        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
        }
    }
}

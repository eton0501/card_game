using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 管理抽牌堆、手牌、棄牌堆與出牌流程，並處理抽牌、棄牌、打牌與牌庫重洗。
/// </summary>


public class CardSystem : Singleton<CardSystem>
{
    [SerializeField] private HandView handView;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;

    private readonly List<CardData> masterDeckData = new();
    private readonly List<Card> drawPile = new();
    private readonly List<Card> discardPile = new();
    private readonly List<Card> hand = new();
    private readonly Dictionary<CardData, int> manaAdjustments = new();

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
    }

    public void Setup(List<CardData> deckData)
    {
        masterDeckData.Clear();
        manaAdjustments.Clear();
        if (deckData != null)
        {
            masterDeckData.AddRange(deckData);
        }

        StartEncounterDeck();
    }

    public void StartEncounterDeck()
    {
        ClearHandViewsImmediate();

        drawPile.Clear();
        discardPile.Clear();
        hand.Clear();

        foreach (CardData data in masterDeckData)
        {
            if (data == null) continue;
            drawPile.Add(BuildCard(data));
        }
    }

    public void AddCardToDeck(CardData cardData)
    {
        if (cardData == null) return;

        masterDeckData.Add(cardData);
        discardPile.Add(BuildCard(cardData));
    }

    public bool TryUpgradeRandomCardMana(out CardData upgradedCardData)
    {
        upgradedCardData = null;
        if (masterDeckData.Count == 0) return false;

        List<CardData> candidates = masterDeckData
            .Where(data => data != null && ResolveMana(data) > 0)
            .ToList();

        if (candidates.Count == 0) return false;

        CardData picked = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        if (!manaAdjustments.ContainsKey(picked))
        {
            manaAdjustments.Add(picked, -1);
        }
        else
        {
            manaAdjustments[picked] -= 1;
        }

        upgradedCardData = picked;
        RefreshExistingPileCardsMana();
        return true;
    }

    private IEnumerator DrawCardsPerformer(DrawCardsGA drawCardsGA)
    {
        int actualAmount = Mathf.Min(drawCardsGA.Amount, drawPile.Count);
        int notDrawnAmount = drawCardsGA.Amount - actualAmount;

        for (int i = 0; i < actualAmount; i++)
        {
            yield return DrawCard();
        }

        if (notDrawnAmount > 0)
        {
            RefillDeck();
            for (int i = 0; i < notDrawnAmount; i++)
            {
                yield return DrawCard();
            }
        }
    }

    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardAllCardsGA)
    {
        List<Card> currentHand = hand.ToList();
        foreach (Card card in currentHand)
        {
            CardView cardView = handView.RemoveCard(card);
            if (cardView != null)
            {
                yield return DiscardCard(cardView);
            }
        }

        hand.Clear();
    }

    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        hand.Remove(playCardGA.Card);
        CardView cardView = handView.RemoveCard(playCardGA.Card);
        if (cardView != null)
        {
            yield return DiscardCard(cardView);
        }

        SpendManaGA spendManaGA = new(playCardGA.Card.Mana);
        ActionSystem.Instance.AddReaction(spendManaGA);

        if (playCardGA.Card.ManualTargetEffect != null)
        {
            PerformEffectGA performEffectGA = new(
                playCardGA.Card.ManualTargetEffect,
                new() { playCardGA.ManualTarget }
            );
            ActionSystem.Instance.AddReaction(performEffectGA);
        }

        foreach (AutoTargetEffect effectWrapper in playCardGA.Card.OtherEffects)
        {
            List<CombatantView> targets = effectWrapper.TargetMode.GetTargets();
            PerformEffectGA performEffectGA = new(effectWrapper.Effect, targets);
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
    }

    private IEnumerator DrawCard()
    {
        Card card = drawPile.Draw();
        if (card == null) yield break;

        hand.Add(card);

        CardView cardView = CardViewCreator.Instance.CreateCardView(
            card,
            drawPilePoint.position,
            drawPilePoint.rotation
        );
        yield return handView.AddCard(cardView);
    }

    private void RefillDeck()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
    }

    private Card BuildCard(CardData cardData)
    {
        return new Card(cardData, ResolveMana(cardData));
    }

    private int ResolveMana(CardData cardData)
    {
        if (cardData == null) return 0;
        int adjustment = 0;
        if (manaAdjustments.TryGetValue(cardData, out int value))
        {
            adjustment = value;
        }

        return Mathf.Max(0, cardData.Mana + adjustment);
    }

    private void RefreshExistingPileCardsMana()
    {
        for (int i = 0; i < drawPile.Count; i++)
        {
            Card card = drawPile[i];
            if (card == null || card.Data == null) continue;
            drawPile[i] = BuildCard(card.Data);
        }

        for (int i = 0; i < discardPile.Count; i++)
        {
            Card card = discardPile[i];
            if (card == null || card.Data == null) continue;
            discardPile[i] = BuildCard(card.Data);
        }

        for (int i = 0; i < hand.Count; i++)
        {
            Card card = hand[i];
            if (card == null || card.Data == null) continue;
            hand[i] = BuildCard(card.Data);
        }
    }

    private IEnumerator DiscardCard(CardView cardView)
    {
        discardPile.Add(cardView.Card);
        cardView.transform.DOScale(Vector3.zero, 0.15f);
        Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.15f);
        yield return tween.WaitForCompletion();
        Destroy(cardView.gameObject);
    }

    private void ClearHandViewsImmediate()
    {
        List<Card> currentHand = hand.ToList();
        foreach (Card card in currentHand)
        {
            CardView cardView = handView.RemoveCard(card);
            if (cardView != null)
            {
                Destroy(cardView.gameObject);
            }
        }

        hand.Clear();
    }
}

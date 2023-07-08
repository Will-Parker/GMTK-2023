using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI : MonoBehaviour
{
    [SerializeField] private CardCollection handCards;
    [SerializeField] public CardCollection tableCards;
    [SerializeField] public CoinCollection bettingPool;

    [SerializeField] private AIType aiType; 
    public AIType GetAIType() { return aiType; }

    public AIActions? PrevAction { get; private set; }

    public int CurrentBet { get; private set; }

    private Hand hand;

    public int id;

    public enum AIType
    {
        Normal,
        Cheater,
        Dealer
    }

    public enum AIActions
    {
        Raise,
        Check,
        Fold
    }

    public struct Hand
    {
        private CardCollection.HandQuality quality;
        public float WinWeight { get; private set; }
        public Dictionary<AIActions, float> ActionWeights { get; private set; }

        public Hand(CardCollection.HandQuality quality)
        {
            this.quality = quality;
            ActionWeights = new Dictionary<AIActions, float> { { AIActions.Raise, 0 }, { AIActions.Check, 0 }, { AIActions.Fold, 0 } };
            switch (quality)
            {
                case CardCollection.HandQuality.Good:
                    WinWeight = 2;
                    ActionWeights[AIActions.Raise] = 120;
                    ActionWeights[AIActions.Check] = 30;
                    ActionWeights[AIActions.Fold] = 1;
                    break;
                case CardCollection.HandQuality.Bad:
                    WinWeight = 1;
                    ActionWeights[AIActions.Raise] = 80;
                    ActionWeights[AIActions.Check] = 30;
                    ActionWeights[AIActions.Fold] = 5;
                    break;
                case CardCollection.HandQuality.Cheated:
                    WinWeight = 3;
                    ActionWeights[AIActions.Raise] = 500;
                    ActionWeights[AIActions.Check] = 50;
                    ActionWeights[AIActions.Fold] = 0;
                    break;
                default:
                    WinWeight = 0;
                    break;
            }
        }

        public void UpdateHandQuality(CardCollection.HandQuality quality)
        {
            this.quality = quality;
            ActionWeights = new Dictionary<AIActions, float> { { AIActions.Raise, 0 }, { AIActions.Check, 0 }, { AIActions.Fold, 0 } };
            switch (quality)
            {
                case CardCollection.HandQuality.Good:
                    WinWeight = 2;
                    ActionWeights[AIActions.Raise] = 120;
                    ActionWeights[AIActions.Check] = 30;
                    ActionWeights[AIActions.Fold] = 1;
                    break;
                case CardCollection.HandQuality.Bad:
                    WinWeight = 1;
                    ActionWeights[AIActions.Raise] = 80;
                    ActionWeights[AIActions.Check] = 30;
                    ActionWeights[AIActions.Fold] = 5;
                    break;
                case CardCollection.HandQuality.Cheated:
                    WinWeight = 3;
                    ActionWeights[AIActions.Raise] = 500;
                    ActionWeights[AIActions.Check] = 50;
                    ActionWeights[AIActions.Fold] = 0;
                    break;
                default:
                    WinWeight = 0;
                    break;
            }
        }
    }

    public enum AIState
    {
        Idle,
        ActiveTurn,
    }

    void Start()
    {
        CurrentBet = 0;
    }

    public IEnumerator PerformTurn()
    {
        
        switch (aiType)
        {
            case AIType.Normal:
                yield return new WaitForSeconds(Random.Range(3f, 7f));
                float newRaiseWeight = hand.ActionWeights[AIActions.Raise] / (1 + (GameManager.instance.GetGameParticipants().Max(p => p.CurrentBet) / 20));
                float newCheckWeight = hand.ActionWeights[AIActions.Check];
                float newFoldWeight = hand.ActionWeights[AIActions.Fold] * (1 + (GameManager.instance.GetGameParticipants().Max(p => p.CurrentBet) / 20));
                float sum = newRaiseWeight + newCheckWeight + newFoldWeight;
                float raiseChance = newRaiseWeight / sum;
                float checkChance = newCheckWeight / sum;
                float foldChance = newFoldWeight / sum;
                float rand = Random.Range(0f, 1f);
                Debug.Log("AI " + id + " probabilities: Raise = " + raiseChance + ", Check = " + checkChance + ", Fold = " + foldChance);
                if (IsValInRange(rand, 0, raiseChance))
                    Raise();
                else if (IsValInRange(rand, raiseChance, raiseChance + checkChance))
                    Check();
                else
                    Fold();
                break;
            case AIType.Cheater:
                break;
            case AIType.Dealer:
                yield return new WaitForSeconds(1f);
                CurrentBet = 10;
                Debug.Log("AI " + id + " set starting bet to " + CurrentBet);
                bettingPool.AddCoin();
                PrevAction = AIActions.Raise;
                break;
        }
        GameManager.instance.EndTurn();
    }

    private void Raise()
    {
        int prevBet = CurrentBet;
        CurrentBet = GameManager.instance.GetGameParticipants().Max(p => p.CurrentBet) + (10 * Random.Range(1, 6));
        Debug.Log("AI " + id + " Raised bet to " + CurrentBet);
        bettingPool.AddCoins((CurrentBet - prevBet) / 10);
        PrevAction = AIActions.Raise;
    }

    private void Check()
    {
        int prevBet = CurrentBet;
        CurrentBet = GameManager.instance.GetGameParticipants().Max(p => p.CurrentBet);
        Debug.Log("AI " + id + " Called bet to " + CurrentBet);
        bettingPool.AddCoins((CurrentBet - prevBet) / 10);
        PrevAction = AIActions.Check;
    }

    private void Fold()
    {
        PutDownHand();
        Debug.Log("AI " + id + " folded");
        PrevAction = AIActions.Fold;
    }

    public void PickUpHand()
    {
        handCards.AddCards(tableCards.RemoveAllCards());
        CardCollection.HandQuality handQuality = CardCollection.HandQuality.Bad;
        if (Random.Range(0f, 1f) > 0.5)
        {
            handQuality = CardCollection.HandQuality.Good;
        }
        Debug.Log("AI " + id + " drew a hand of " + handQuality + " quality");
        hand = new Hand(handQuality);
    }

    public void PutDownHand()
    {
        tableCards.AddCards(handCards.RemoveAllCards());
    }

    public void Reveal()
    {
        tableCards.FlipAllCards();
    }

    private bool IsValInRange(float val, float minInclusive, float maxInclusive)
    {
        return val >= minInclusive && val <= maxInclusive;
    }
}

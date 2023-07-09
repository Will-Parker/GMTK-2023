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

    public int CurrentBet { get; set; }

    public Hand hand { get; private set; }

    public int id;

    [SerializeField] private AIColor aiColor;
    public AIColor GetAIColor() { return aiColor; }

    public Animator Anim { get; private set; }

    public enum AIType
    {
        Normal,
        Cheater,
        Dealer
    }

    public enum AIColor
    {
        Blue,
        Purple,
        Pink,
        Green,
        Yellow,
        Red,
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

    private void Awake()
    {
        if (aiType != AIType.Dealer)
            Anim = GetComponent<Animator>();
    }

    private void Start()
    {
        CurrentBet = 0;
    }

    private void Update()
    {
        if (aiType == AIType.Dealer)
            return;
        if (aiType == AIType.Normal)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                Anim.SetTrigger("BadHand");
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                Anim.SetTrigger("GoodHand");
            }
        }
        if (aiType == AIType.Cheater)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Anim.SetTrigger("Cheating1");
            }
            if (aiColor == AIColor.Pink || aiColor == AIColor.Red)
            {
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Anim.SetTrigger("Cheating2");
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Anim.SetTrigger("Lose");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Anim.SetTrigger("Win");
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Anim.SetTrigger("No Reaction");
        }
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
                yield return new WaitForSeconds(Random.Range(3f, 7f));
                float newRaiseWeight2 = hand.ActionWeights[AIActions.Raise] / (1 + (GameManager.instance.GetGameParticipants().Max(p => p.CurrentBet) / 20));
                float newCheckWeight2 = hand.ActionWeights[AIActions.Check];
                float newFoldWeight2 = hand.ActionWeights[AIActions.Fold] * (1 + (GameManager.instance.GetGameParticipants().Max(p => p.CurrentBet) / 20));
                float sum2 = newRaiseWeight2 + newCheckWeight2 + newFoldWeight2;
                float raiseChance2 = newRaiseWeight2 / sum2;
                float checkChance2 = newCheckWeight2 / sum2;
                float foldChance2 = newFoldWeight2 / sum2;
                float rand2 = Random.Range(0f, 1f);
                Debug.Log("AI " + id + " probabilities: Raise = " + raiseChance2 + ", Check = " + checkChance2 + ", Fold = " + foldChance2);
                if (IsValInRange(rand2, 0, raiseChance2))
                    Raise();
                else if (IsValInRange(rand2, raiseChance2, raiseChance2 + checkChance2))
                    Check();
                else
                    Fold();
                break;
            case AIType.Dealer:
                yield return new WaitForSeconds(1f);
                CurrentBet = 10;
                Debug.Log("AI " + id + " set starting bet to " + CurrentBet);
                bettingPool.AddCoin();
                PrevAction = AIActions.Raise;
                break;
        }
        Debug.Log("End Turn");
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
        Card[] cards = tableCards.RemoveAllCards();
        foreach (Card card in cards)
        {
            card.GetComponentsInChildren<MeshRenderer>().All(mr => mr.enabled = false);
        }
        handCards.AddCards(cards);
        if (aiType != AIType.Dealer)
            Anim.SetBool("isHoldingCards", true);

        CardCollection.HandQuality handQuality = CardCollection.HandQuality.Bad;
        if (Random.Range(0f, 1f) > 0.5)
        {
            handQuality = CardCollection.HandQuality.Good;
        }
        Debug.Log("AI " + id + " drew a hand of " + handQuality + " quality");
        if (aiType == AIType.Normal)
        {
            if (handQuality == CardCollection.HandQuality.Bad)
            {
                Anim.SetTrigger("BadHand");
            }
            else if (handQuality == CardCollection.HandQuality.Good)
            {
                Anim.SetTrigger("GoodHand");
            }
        }
        hand = new Hand(handQuality);
    }

    public void PutDownHand()
    {
        if (aiType != AIType.Dealer)
            Anim.SetBool("isHoldingCards", false);
        Card[] cards = handCards.RemoveAllCards();
        tableCards.AddCards(cards);
        foreach (Card card in cards)
        {
            card.GetComponentsInChildren<MeshRenderer>().All(mr => mr.enabled = true);
        }
    }

    public void Reveal()
    {
        tableCards.FlipAllCards();
    }

    public void CleanArea()
    {
        handCards.RemoveAllCards();
        tableCards.RemoveAllCards();
        bettingPool.RemoveAllCoins();
        CurrentBet = 0;
        PrevAction = null;
    }

    private bool IsValInRange(float val, float minInclusive, float maxInclusive)
    {
        return val >= minInclusive && val <= maxInclusive;
    }
}

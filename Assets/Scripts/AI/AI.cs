using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

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
    public NavMeshAgent Agent { get; private set; }

    public AIState AiState;

    private Coroutine purpleCheatCoroutine;
    private Coroutine checkIfCanMoveCoroutine;
    private Coroutine pinkRedCheatCoroutine;
    public Coroutine performTurnCoroutine;

    public GameManager gm;

    private float switchTableOdds;

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
        public CardCollection.HandQuality Quality { get; private set; }
        public float WinWeight { get; private set; }
        public Dictionary<AIActions, float> ActionWeights { get; private set; }

        public Hand(CardCollection.HandQuality quality)
        {
            this.Quality = quality;
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
        Moving,
        Detained
    }

    private void Awake()
    {
        if (aiType != AIType.Dealer)
        {
            Anim = GetComponent<Animator>();
            Agent = GetComponent<NavMeshAgent>();
        }
            
    }

    private void Start()
    {
        CurrentBet = 0;
        if (aiType != AIType.Dealer || (aiColor != AIColor.Pink && aiColor != AIColor.Red))
        {
            checkIfCanMoveCoroutine = StartCoroutine(CheckIfCanMove());
        }
    }

    private void Update()
    {
        if (AiState == AIState.Moving)
        {
            if (Agent.remainingDistance <= 0.1)
            {
                if (gm.TryAddParticipant(this))
                {
                    if (aiType != AIType.Dealer || (aiColor != AIColor.Pink && aiColor != AIColor.Red))
                    {
                        checkIfCanMoveCoroutine = StartCoroutine(CheckIfCanMove());
                    }
                    AiState = AIState.Idle;
                }
            }
        }
        if (AiState == AIState.Detained)
        {
            if (aiType == AIType.Cheater)
            {
                AudioManager.instance.Play("Correct");
                if (performTurnCoroutine != null)
                {
                    StopCoroutine(performTurnCoroutine);
                    AiState = AIState.Idle;
                    performTurnCoroutine = null;
                    gm.EndTurn();
                }
                gm.gameParticipants.Remove(this);
                gm.ResetGameParticipants();
                Destroy(this.gameObject);
            }
            else
            {
                AudioManager.instance.Play("Wrong");
                FindObjectOfType<WinLoseTracker>().remainingGameLength -= 180 * 1000;
                AiState = AIState.Idle;
            }
        }
    }

    public IEnumerator PerformTurn()
    {
        AiState = AIState.ActiveTurn;
        switch (aiType)
        {
            case AIType.Normal:
                yield return new WaitForSeconds(Random.Range(3f, 7f));
                float newRaiseWeight = hand.ActionWeights[AIActions.Raise] / (1 + (gm.GetGameParticipants().Max(p => p.CurrentBet) / 20));
                float newCheckWeight = hand.ActionWeights[AIActions.Check];
                float newFoldWeight = hand.ActionWeights[AIActions.Fold] * (1 + (gm.GetGameParticipants().Max(p => p.CurrentBet) / 20));
                float sum = newRaiseWeight + newCheckWeight + newFoldWeight;
                float raiseChance = newRaiseWeight / sum;
                float checkChance = newCheckWeight / sum;
                float foldChance = newFoldWeight / sum;
                float rand = Random.Range(0f, 1f);
                //Debug.Log("AI " + id + " probabilities: Raise = " + raiseChance + ", Check = " + checkChance + ", Fold = " + foldChance);
                if (IsValInRange(rand, 0, raiseChance))
                    Raise();
                else if (IsValInRange(rand, raiseChance, raiseChance + checkChance))
                    Check();
                else
                    Fold();
                break;
            case AIType.Cheater:
                yield return new WaitForSeconds(Random.Range(3f, 7f));
                float newRaiseWeight2 = hand.ActionWeights[AIActions.Raise] / (1 + (gm.GetGameParticipants().Max(p => p.CurrentBet) / 20));
                float newCheckWeight2 = hand.ActionWeights[AIActions.Check];
                float newFoldWeight2 = hand.ActionWeights[AIActions.Fold] * (1 + (gm.GetGameParticipants().Max(p => p.CurrentBet) / 10));
                float sum2 = newRaiseWeight2 + newCheckWeight2 + newFoldWeight2;
                float raiseChance2 = newRaiseWeight2 / sum2;
                float checkChance2 = newCheckWeight2 / sum2;
                float foldChance2 = newFoldWeight2 / sum2;
                float rand2 = Random.Range(0f, 1f);
                //Debug.Log("AI " + id + " probabilities: Raise = " + raiseChance2 + ", Check = " + checkChance2 + ", Fold = " + foldChance2);
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
                //Debug.Log("AI " + id + " set starting bet to " + CurrentBet);
                bettingPool.AddCoin();
                PrevAction = AIActions.Raise;
                break;
        }
        AiState = AIState.Idle;
        performTurnCoroutine = null;
        gm.EndTurn();
    }

    private void Raise()
    {
        int prevBet = CurrentBet;
        CurrentBet = gm.GetGameParticipants().Max(p => p.CurrentBet) + (10 * Random.Range(1, 6));
        //Debug.Log("AI " + id + " Raised bet to " + CurrentBet);
        bettingPool.AddCoins((CurrentBet - prevBet) / 10);
        PrevAction = AIActions.Raise;
        if (FindObjectOfType<Screen>().CurrentScreen == gm.gameID)
            AudioManager.instance.Play("Chips");
    }

    private void Check()
    {
        int prevBet = CurrentBet;
        CurrentBet = gm.GetGameParticipants().Max(p => p.CurrentBet);
        //Debug.Log("AI " + id + " Called bet to " + CurrentBet);
        bettingPool.AddCoins((CurrentBet - prevBet) / 10);
        PrevAction = AIActions.Check;
        if (FindObjectOfType<Screen>().CurrentScreen == gm.gameID)
            AudioManager.instance.Play("Chips");
    }

    private void Fold()
    {
        PutDownHand();
        //Debug.Log("AI " + id + " folded");
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
        //Debug.Log("AI " + id + " drew a hand of " + handQuality + " quality");
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
        if (aiColor == AIColor.Purple)
        {
            purpleCheatCoroutine = StartCoroutine(PurpleCheat());
        }
        if (aiColor == AIColor.Pink || aiColor == AIColor.Red)
        {
            pinkRedCheatCoroutine = StartCoroutine(PinkRedCheat());
        }
        if (FindObjectOfType<Screen>().CurrentScreen == gm.gameID)
            AudioManager.instance.Play("Card");
    }

    public void PutDownHand()
    {
        if (aiType != AIType.Dealer)
            Anim.SetBool("isHoldingCards", false);
        if (aiColor == AIColor.Purple)
            StopCoroutine(purpleCheatCoroutine);
        Card[] cards = handCards.RemoveAllCards();
        tableCards.AddCards(cards);
        foreach (Card card in cards)
        {
            card.GetComponentsInChildren<MeshRenderer>().All(mr => mr.enabled = true);
        }
        if (FindObjectOfType<Screen>().CurrentScreen == gm.gameID)
            AudioManager.instance.Play("Card");
    }

    public void Reveal()
    {
        tableCards.FlipAllCards();
        if (FindObjectOfType<Screen>().CurrentScreen == gm.gameID)
            AudioManager.instance.Play("Card");
    }

    public void CleanArea()
    {
        
        foreach (Card card in handCards.RemoveAllCards())
        {
            Destroy(card.gameObject);
        }
        foreach (Card card in tableCards.RemoveAllCards())
        {
            Destroy(card.gameObject);
        }
        bettingPool.RemoveAllCoins();
        CurrentBet = 0;
        PrevAction = null;
        if (FindObjectOfType<Screen>().CurrentScreen == gm.gameID)
        {
            AudioManager.instance.Play("Card");
            AudioManager.instance.Play("Chips");
        }
    }

    public void MoveTo(Seat seat)
    {
        Agent.SetDestination(seat.transform.position);
        AiState = AIState.Moving;
    }

    private IEnumerator CheckIfCanMove()
    {
        float odds = 1f;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            if (AiState == AIState.Idle)
            {
                if (Random.Range(0f, 1f) < odds)
                {
                    if (gm.TrySwitchTables(this))
                    {
                        Debug.Log("Switch Tables");
                        StopCoroutine(checkIfCanMoveCoroutine);
                        break;
                    }
                }
            }
        }
    }

    private IEnumerator PurpleCheat()
    {
        float purpleCheatOdds = hand.Quality == CardCollection.HandQuality.Good ? 0.02f : 0.1f;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            if (AiState != AIState.ActiveTurn)
            {
                if (Random.Range(0f, 1f) < purpleCheatOdds)
                {
                    Debug.Log("Purple Cheated");
                    Anim.SetTrigger("Cheating1");
                    hand = new Hand(CardCollection.HandQuality.Cheated);
                    StopCoroutine(purpleCheatCoroutine);
                    break;
                }
            }
        }
    }

    private IEnumerator PinkRedCheat()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 7f));
            if (AiState != AIState.Moving)
            {
                Debug.Log("Pink/red cheat");
                AI other;
                if (aiColor == AIColor.Pink)
                {
                    other = new List<AI>(FindObjectsOfType<AI>()).FindLast(ai => ai.aiColor == AIColor.Red);
                }
                else
                {
                    other = new List<AI>(FindObjectsOfType<AI>()).FindLast(ai => ai.aiColor == AIColor.Pink);
                }
                if (gm.GetGameParticipants().Max(p => p.CurrentBet) > 150)
                {
                    if (Random.Range(0f, 1f) < 0.2f)
                        Anim.SetTrigger("Cheating1");
                    gm.TrySwitchTables(other);
                }
                else
                {
                    if (Random.Range(0f, 1f) < 0.2f)
                        Anim.SetTrigger("Cheating2");
                    gm.TrySwitchTables(other);
                }
            }
        }
    }

    private bool IsValInRange(float val, float minInclusive, float maxInclusive)
    {
        return val >= minInclusive && val <= maxInclusive;
    }
}

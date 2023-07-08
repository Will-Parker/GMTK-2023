using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private List<AI> gameParticipants;
    private Queue<AI> gameParticipantsQueue;
    public List<AI> GetGameParticipants() { return gameParticipants; }
    private GamePhase gamePhase;

    public enum GamePhase
    {
        NoActiveGame,
        Dealing,
        Game,
        GameEnd,
        CollectBid,
        Cleanup
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TransitionToNextPhase();
        }
    }

    public void TransitionToNextPhase()
    {
        switch (gamePhase)
        {
            case GamePhase.NoActiveGame:
                gamePhase = GamePhase.Dealing;
                gameParticipantsQueue = new Queue<AI>(gameParticipants);
                Dealer dealer;
                if (gameParticipantsQueue.Peek().TryGetComponent(out dealer))
                    StartCoroutine(dealer.DealCards());
                else
                    Debug.LogError("Dealer not first in Queue");
                break;
            case GamePhase.Dealing:
                foreach(AI particpant in gameParticipants)
                {
                    particpant.PickUpHand();
                }
                if (gameParticipantsQueue.Peek().TryGetComponent<Dealer>(out _))
                {
                    gamePhase = GamePhase.Game;
                    StartCoroutine(gameParticipantsQueue.Peek().PerformTurn());
                }
                else
                    Debug.LogError("Dealer not first in Queue");
                break;
            case GamePhase.Game:
                Debug.Log("Game End");
                gamePhase = GamePhase.GameEnd;
                foreach (AI participant in gameParticipants)
                {
                    if (participant.PrevAction != AI.AIActions.Fold)
                    {
                        participant.PutDownHand();
                        participant.Reveal();
                    }
                }
                CalcWinner();
                break;
            case GamePhase.GameEnd:
                gamePhase = GamePhase.NoActiveGame;
                StartCoroutine(WaitToStartNextGame());
                break;
        }
    }

    public void EndTurn()
    {
        if (gamePhase != GamePhase.Game)
        {
            Debug.LogError("Not in Game Phase. Unable to end turn");
            return;
        }
        AI prevActiveParticipant = gameParticipantsQueue.Dequeue();
        if (prevActiveParticipant.PrevAction != AI.AIActions.Fold)
            gameParticipantsQueue.Enqueue(prevActiveParticipant);

        List<AI> temp = new(gameParticipantsQueue);
        if (gameParticipantsQueue.Count == 2 || temp.TrueForAll(a => a.PrevAction == AI.AIActions.Check || a.GetAIType() == AI.AIType.Dealer))
            TransitionToNextPhase();
        else
        {
            while (gameParticipantsQueue.Peek() != prevActiveParticipant)
            {
                AI nextPotentialActiveParticipant = gameParticipantsQueue.Peek();
                if (nextPotentialActiveParticipant.GetAIType() != AI.AIType.Dealer)
                {
                    StartCoroutine(nextPotentialActiveParticipant.PerformTurn());
                    return;
                }
                gameParticipantsQueue.Enqueue(gameParticipantsQueue.Dequeue());
            }
        }
    }

    private void CalcWinner()
    {
        float[] winChance = new float[4] { 0, 0, 0, 0 };
        float sum = 0;
        foreach (AI participant in gameParticipants)
        {
            if (participant.PrevAction != AI.AIActions.Fold)
            {
                if (participant.GetAIType() != AI.AIType.Dealer)
                {
                    sum += participant.hand.WinWeight;
                }
            }
        }
        foreach (AI participant in gameParticipants)
        {
            if (participant.PrevAction != AI.AIActions.Fold && participant.GetAIType() != AI.AIType.Dealer)
            {
                winChance[participant.id] = participant.hand.WinWeight / sum;
            }
        }
        float rand = Random.Range(0f, 1f);
        Debug.Log(rand);
        Debug.Log(winChance[0]);
        Debug.Log(winChance[1]);
        Debug.Log(winChance[2]);
        Debug.Log(winChance[3]);
        if (IsValInRange(rand, 0, winChance[0]))
        {
            StartCoroutine(HandleWin(gameParticipants.FindLast(p => p.id == 0)));
        }
        else if (IsValInRange(rand, winChance[0], winChance[0] + winChance[1]))
        {
            StartCoroutine(HandleWin(gameParticipants.FindLast(p => p.id == 1)));
        }
        else if (IsValInRange(rand, winChance[0] + winChance[1], winChance[0] + winChance[1] + winChance[2]))
        {
            StartCoroutine(HandleWin(gameParticipants.FindLast(p => p.id == 2)));
        }
        else
        {
            StartCoroutine(HandleWin(gameParticipants.FindLast(p => p.id == 3)));
        }
    }

    private IEnumerator HandleWin(AI winner)
    {
        yield return new WaitForSeconds(5f);
        int earnings = 0;
        foreach (AI participant in gameParticipants)
        {
            if (participant == winner)
            {
                // TODO: update anim
                participant.CurrentBet = 0;
            }
            else
            {
                // TODO: update anim
                earnings += participant.CurrentBet;
                participant.CurrentBet = 0;
                participant.bettingPool.RemoveAllCoins();
            }
        }
        winner.bettingPool.AddCoins(earnings / 10);
        StartCoroutine(CleanTable());
    }

    private IEnumerator CleanTable()
    {
        yield return new WaitForSeconds(5f);
        foreach (AI participant in gameParticipants)
        {
            participant.CleanArea();
        }
        TransitionToNextPhase();
    }

    private IEnumerator WaitToStartNextGame()
    {
        yield return new WaitForSeconds(Random.Range(3f, 7f));
        TransitionToNextPhase();
    }

    private bool IsValInRange(float val, float minInclusive, float maxInclusive)
    {
        return minInclusive != maxInclusive && val >= minInclusive && val <= maxInclusive;
    }
}

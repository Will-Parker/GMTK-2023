using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] public List<AI> gameParticipants;
    public Queue<AI> gameParticipantsQueue;
    public List<AI> GetGameParticipants() { return gameParticipants; }
    public List<Seat> seats;
    private GamePhase gamePhase;
    public GameManager otherGame;

    public enum GamePhase
    {
        NoActiveGame,
        Dealing,
        Game,
        GameEnd,
        CollectBid,
        Cleanup
    }

    private void Start()
    {
        //Time.timeScale = 20f;
        foreach (AI participant in FindObjectsOfType<AI>())
        {
            TryAddParticipant(participant);
        }
        TransitionToNextPhase();
    }

    public void TransitionToNextPhase()
    {
        switch (gamePhase)
        {
            case GamePhase.NoActiveGame:
                gamePhase = GamePhase.Dealing;
                gameParticipantsQueue = new Queue<AI>(gameParticipants);
                ResetGameParticipants();
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
                //Debug.Log("Game End");
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

    public bool TrySwitchTables(AI participant)
    {
        if (participant.PrevAction == AI.AIActions.Fold || gamePhase == GamePhase.NoActiveGame)
        {
            if (otherGame.gamePhase == GamePhase.NoActiveGame)
            {
                foreach (Seat newSeat in otherGame.seats)
                {
                    if (newSeat.participant == null)
                    {
                        Seat currSeat = seats.Find(x => x.id == participant.id);
                        
                        int a = participant.CurrentBet;
                        participant.bettingPool.RemoveAllCoins();
                        gameParticipants[0].bettingPool.AddCoins(a / 10);
                        gameParticipants[0].CurrentBet = a;
                        participant.CleanArea();
                        currSeat.RemoveParticipant(participant);
                        participant.MoveTo(newSeat);
                        newSeat.participant = participant;
                        participant.gm = otherGame;
                        gameParticipants.Remove(participant);
                        return true;
                    }
                    else if (newSeat.participant.PrevAction == AI.AIActions.Fold)
                    {
                        AI otherParticipant = newSeat.participant;
                        Seat currSeat = seats.Find(x => x.id == participant.id);

                        int a = participant.CurrentBet;
                        participant.bettingPool.RemoveAllCoins();
                        gameParticipants[0].bettingPool.AddCoins(a / 10);
                        gameParticipants[0].CurrentBet = a;
                        participant.CleanArea();

                        int b = otherParticipant.CurrentBet;
                        otherParticipant.bettingPool.RemoveAllCoins();
                        otherGame.gameParticipants[0].bettingPool.AddCoins(a / 10);
                        otherGame.gameParticipants[0].CurrentBet = a;
                        otherParticipant.CleanArea();

                        currSeat.RemoveParticipant(participant);
                        newSeat.RemoveParticipant(otherParticipant);
                        participant.MoveTo(newSeat);
                        otherParticipant.MoveTo(currSeat);
                        newSeat.participant = participant;
                        currSeat.participant = otherParticipant;
                        participant.gm = otherGame;
                        otherParticipant.gm = this;
                        gameParticipants.Remove(participant);
                        otherGame.gameParticipants.Remove(otherParticipant);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool TryAddParticipant(AI participant)
    {
        foreach (Seat seat in seats)
        {
            if (Vector3.Distance(participant.transform.position, seat.transform.position) <= 0.1)
            {
                seat.AddParticipant(participant);
                gameParticipants.Add(participant);
                participant.gm = this;
                if (gamePhase == GamePhase.Game)
                {
                    if (gameParticipants[0].TryGetComponent(out Dealer dealer))
                        StartCoroutine(dealer.DealCards(participant));
                }
                return true;
            }
        }
        return false;
    }

    public void ResetGameParticipants()
    {
        AI currPlayer = gameParticipantsQueue.Peek();
        List<AI> arrCopy = new List<AI>(gameParticipants);
        List<AI> temp = new List<AI>();
        while (arrCopy.Count > 0)
        {
            AI minEl = arrCopy.Aggregate((curMin, x) => (curMin == null || x.id < curMin.id ? x : curMin));
            temp.Add(minEl);
            arrCopy.Remove(minEl);
        }
        gameParticipants = temp;
        //Debug.Log(gameParticipants.Count);
        Queue<AI> q = new Queue<AI>();
        foreach (AI participant in gameParticipants)
        {
            if (participant.PrevAction != AI.AIActions.Fold)
            {
                q.Enqueue(participant);
            }
        }
        while (q.Peek() != currPlayer)
        {
            q.Enqueue(q.Dequeue());
        }
        gameParticipantsQueue = q;
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
                if (participant.GetAIType() != AI.AIType.Dealer)
                    participant.Anim.SetTrigger("Win");
                participant.CurrentBet = 0;
            }
            else
            {
                if (participant.GetAIType() != AI.AIType.Dealer)
                    participant.Anim.SetTrigger("Lose");
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
            if (participant.GetAIType() != AI.AIType.Dealer)
                participant.Anim.SetTrigger("No Reaction");
        }
        TransitionToNextPhase();
    }

    private IEnumerator WaitToStartNextGame()
    {
        yield return new WaitForSeconds(Random.Range(3f, 7f));
        //yield return new WaitForSeconds(20f);
        TransitionToNextPhase();
    }

    private bool IsValInRange(float val, float minInclusive, float maxInclusive)
    {
        return minInclusive != maxInclusive && val >= minInclusive && val <= maxInclusive;
    }
}

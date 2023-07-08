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
        Reveal,
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
                gameParticipantsQueue = new Queue<AI>(gameParticipants);
                Dealer dealer;
                if (gameParticipantsQueue.Peek().TryGetComponent(out dealer))
                    StartCoroutine(dealer.DealCards());
                else
                    Debug.LogError("Dealer not first in Queue");
                gamePhase = GamePhase.Dealing;
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
                gamePhase = GamePhase.Reveal;
                break;
            case GamePhase.Reveal:
                gamePhase = GamePhase.CollectBid;
                break;
            case GamePhase.CollectBid:
                gamePhase = GamePhase.Cleanup;
                break;
            case GamePhase.Cleanup:
                gamePhase = GamePhase.NoActiveGame;
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
}

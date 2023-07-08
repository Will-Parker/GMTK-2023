using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private List<AI> gameParticipants;
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
}

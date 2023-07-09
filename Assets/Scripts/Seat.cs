using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public int id;
    public CardCollection tableCards;
    public CoinCollection bettingPool;
    public AI participant;

    public void AddParticipant(AI participant)
    {
        participant.transform.parent = transform;
        participant.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        participant.tableCards = tableCards;
        participant.bettingPool = bettingPool;
        this.participant = participant;
        participant.id = id;
    }

    public void RemoveParticipant(AI participant)
    {
        participant.transform.parent = null;
        participant.tableCards = null;
        participant.bettingPool = null;
        this.participant = null;
        participant.id = 0;
    }
}

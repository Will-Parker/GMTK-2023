using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    [SerializeField] private Card[] cards;

    private const int NumCardsToDeal = 2;

    [SerializeField] private GameManager gm;

    public IEnumerator DealCards()
    {
        try
        {
            for (int i = 0; i < NumCardsToDeal; i++)
            {
                foreach (AI ai in gm.GetGameParticipants())
                {
                    ai.tableCards.AddCard(Instantiate(cards[Random.Range(0, 4)]));
                    if (FindObjectOfType<Screen>().CurrentScreen == gm.gameID)
                    {
                        AudioManager.instance.Play("Card");
                    }
                    yield return new WaitForSeconds(0.5f);
                }
            }
            yield return new WaitForSeconds(1f);
        }
        finally
        {
            gm.GetGameParticipants().ForEach(ai => ai.CleanArea());
            gm.GetGameParticipants().ForEach(ai => ai.tableCards.AddCards(new Card[] { Instantiate(cards[Random.Range(0, 4)]), Instantiate(cards[Random.Range(0, 4)]) }));
            gm.TransitionToNextPhase();
        }
    }

    public IEnumerator DealCards(AI participant)
    {
        for (int i = 0; i < NumCardsToDeal; i++)
        {
            participant.tableCards.AddCard(Instantiate(cards[Random.Range(0, 4)]));
            yield return new WaitForSeconds(0.5f);
        }
        gm.ResetGameParticipants();
        participant.PickUpHand();
    }
}

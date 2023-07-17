using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    [SerializeField] private Card card;

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
                    ai.tableCards.AddCard(Instantiate(card));
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
            gm.GetGameParticipants().ForEach(ai => ai.tableCards.AddCards(new Card[] { Instantiate(card), Instantiate(card) }));
            gm.TransitionToNextPhase();
        }
    }

    public IEnumerator DealCards(AI participant)
    {
        for (int i = 0; i < NumCardsToDeal; i++)
        {
            participant.tableCards.AddCard(Instantiate(card));
            yield return new WaitForSeconds(0.2f);
        }
        gm.ResetGameParticipants();
        participant.PickUpHand();
    }
}

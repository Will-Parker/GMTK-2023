using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    [SerializeField] private Card card;

    private const int NumCardsToDeal = 2;

    public IEnumerator DealCards()
    {
        for (int i = 0; i < NumCardsToDeal; i++)
        {
            foreach (AI ai in GameManager.instance.GetGameParticipants())
            {
                ai.tableCards.AddCard(Instantiate(card));
                yield return new WaitForSeconds(0.2f);
            }
        }
        GameManager.instance.TransitionToNextPhase();
    }
}

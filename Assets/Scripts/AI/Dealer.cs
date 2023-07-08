using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    [SerializeField] private AI[] ais;
    [SerializeField] private Card card;

    private const int NumCardsToDeal = 2;

    public IEnumerator DealCards()
    {
        for (int i = 0; i < NumCardsToDeal; i++)
        {
            foreach (AI ai in ais)
            {
                ai.tableCards.AddCard(Instantiate(card));
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(DealCards());
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField] private CardCollection handCards;
    [SerializeField] public CardCollection tableCards;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            handCards.AddCards(tableCards.RemoveAllCards());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) 
        {
            tableCards.AddCards(handCards.RemoveAllCards());
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            tableCards.FlipAllCards();
        }
    }


}

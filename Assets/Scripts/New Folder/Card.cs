using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardSide 
    { 
        FaceUp,
        FaceDown
    }

    public CardSide cardSide = CardSide.FaceDown;

    public void Flip()
    {
        if (cardSide == CardSide.FaceDown)
        {
            cardSide = CardSide.FaceUp;
            transform.localRotation = Quaternion.Euler(0, 180, 0);

        }
        else if (cardSide == CardSide.FaceUp)
        {
            cardSide = CardSide.FaceDown;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}

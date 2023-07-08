using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollection : MonoBehaviour
{
    private const float CardWidth = 0.2f;
    [SerializeField] private List<Card> cards;
    // Start is called before the first frame update
    void Start()
    {
        cards.AddRange(transform.GetComponentsInChildren<Card>(false));
        OrganizeCards();
    }

    private void OrganizeCards()
    {
        float collectionWidth = cards.Count * CardWidth;
        float startPosition = (collectionWidth / (-2)) + (CardWidth / 2);
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.localPosition = Vector3.right * (startPosition + (i * CardWidth));
            cards[i].transform.localRotation = cards[i].cardSide == Card.CardSide.FaceDown ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        }
    }

    public void AddCard(Card newCard)
    {
        if (!cards.Exists(card => card == newCard))
        {
            newCard.transform.parent = transform;
            cards.Add(newCard);
            OrganizeCards();
        }
        else
            Debug.LogWarning("NewCard has already been added to the card collection");
    }

    public void AddCards(Card[] newCards)
    {
        foreach (Card newCard in newCards)
        {
            AddCard(newCard);
        }
    }

    public Card RemoveCard(Card cardToRemove)
    {
        if (cards.Remove(cardToRemove))
        {
            cardToRemove.transform.parent = null;
            cardToRemove.transform.position = Vector3.down * 100;
            OrganizeCards();
            return cardToRemove;
        }
        else
            Debug.LogWarning("CardToRemove was not found in the card collection");
        return null;
    }

    public Card RemoveCard(int indexOfCardToRemove)
    {
        if (indexOfCardToRemove < cards.Count && indexOfCardToRemove >= 0)
        {
            Card cardToRemove = cards[indexOfCardToRemove];
            cardToRemove.transform.parent = null;
            cards.RemoveAt(indexOfCardToRemove);
            cardToRemove.transform.position = Vector3.down * 100;
            OrganizeCards();
            return cardToRemove;
        }
        else
            Debug.LogWarning("IndexOfCardToRemove was outside of the range of the card collection");
        return null;
    }

    public Card RemoveCard()
    {
        return RemoveCard(0);
    }

    public Card[] RemoveCards(Card[] cardsToRemove)
    {
        Card[] cardsRemoved = new Card[cardsToRemove.Length];
        for (int i = 0; i < cardsToRemove.Length; i++)
        {
            cardsRemoved[i] = RemoveCard(cardsToRemove[i]);
        }
        return cardsRemoved;
    }

    public Card[] RemoveAllCards()
    {
        Card[] cardsRemoved = cards.ToArray();
        cards.Clear();
        foreach(Card cardRemoved in cardsRemoved)
        {
            cardRemoved.transform.position = Vector3.down * 100;
        }
        OrganizeCards();

        return cardsRemoved;
    }

    public void FlipAllCards()
    {
        if (cards.Count > 0)
        {
            foreach (Card card in cards)
            {
                card.Flip();
            }
        }
        else
            Debug.LogWarning("Nothing to Flip. No cards in collection.");
    }
}

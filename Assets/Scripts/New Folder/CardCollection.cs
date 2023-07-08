using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollection : MonoBehaviour
{
    private const float card_width = 0.2f;
    [SerializeField] private List<Card> cards;
    // Start is called before the first frame update
    void Start()
    {
        cards.AddRange(transform.GetComponentsInChildren<Card>(false));
        OrganizeCards();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cards[0].Flip();
        }
    }

    private void OrganizeCards()
    {
        float collectionWidth = cards.Count * card_width;
        float startPosition = (collectionWidth / (-2)) + (card_width / 2);
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.localPosition = Vector3.right * (startPosition + (i * card_width));
        }
    }
}

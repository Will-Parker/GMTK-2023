using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollection : MonoBehaviour
{
    [SerializeField]
    private List<Card> cards;
    // Start is called before the first frame update
    void Start()
    {
        cards.AddRange(transform.GetComponentsInChildren<Card>(false));
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
        
    }
}

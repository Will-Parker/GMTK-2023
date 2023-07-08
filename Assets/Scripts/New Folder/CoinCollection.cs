using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollection : MonoBehaviour
{
    private const float CoinWidth = 0.1f;
    private const float CoinHeight = 0.02f;
    [SerializeField] private List<Coin> coins;
    [SerializeField] private Coin coin;

    void Start()
    {
        coins.AddRange(transform.GetComponentsInChildren<Coin>(false));
        OrganizeCoins();
    }

    private void OrganizeCoins()
    {
        float xPos = 0f;
        float yPos = 0f;
        float zPos = 0f;
        for (int i = 1; i <= coins.Count; i++)
        {
            coins[i-1].transform.localPosition = new Vector3(xPos, yPos, zPos);
            if (i % 5 == 0)
            {
                yPos = 0f;
                if (i % 3 == 0)
                {
                    zPos -= CoinWidth;
                    xPos = 0f;
                }
                else
                {
                    xPos += CoinWidth;
                }
            }
            else
            {
                yPos += CoinHeight;
            }
        }
    }

    public void AddCoin()
    {
        Coin c = Instantiate(coin);
        c.transform.parent = transform;
        coins.Add(c);
        OrganizeCoins();
    }

    public void AddCoins(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Coin c = Instantiate(coin);
            c.transform.parent = transform;
            coins.Add(c);
        }
        OrganizeCoins();
    }

    public void RemoveCoin()
    {
        Coin coin = coins[0];
        coins.RemoveAt(0);
        Destroy(coin.gameObject);
        OrganizeCoins();
    }

    public void RemoveCoins(int count)
    {
        if (count <= coins.Count)
        {
            List<Coin> coins = this.coins.GetRange(0, count);
            this.coins.RemoveRange(0, count);
            foreach (Coin coin in coins)
            {
                Destroy(coin.gameObject);
            }
            OrganizeCoins();
        }
        else
            RemoveAllCoins();
        
    }

    public void RemoveAllCoins()
    {
        foreach (Coin coin in coins)
        {
            Destroy(coin.gameObject);
        }
        coins.Clear();
    }
}

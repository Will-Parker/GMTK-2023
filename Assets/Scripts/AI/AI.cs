using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField] private CardCollection handCards;
    [SerializeField] public CardCollection tableCards;

    public bool IsCheater { get; private set; }

    public enum AIActions
    {
        Raise,
        Check,
        Fold
    }

    public struct Hand
    {
        private CardCollection.HandQuality quality;
        public float WinWeight { get; private set; }
        public Dictionary<AIActions, float> ActionWeights { get; private set; }

        public Hand(CardCollection.HandQuality quality)
        {
            this.quality = quality;
            ActionWeights = new Dictionary<AIActions, float> { { AIActions.Raise, 0 }, { AIActions.Check, 0 }, { AIActions.Fold, 0 } };
            switch (quality)
            {
                case CardCollection.HandQuality.Good:
                    WinWeight = 2;
                    ActionWeights[AIActions.Raise] = 2;
                    ActionWeights[AIActions.Check] = 3;
                    ActionWeights[AIActions.Fold] = 1;
                    break;
                case CardCollection.HandQuality.Bad:
                    WinWeight = 1;
                    ActionWeights[AIActions.Raise] = 1;
                    ActionWeights[AIActions.Check] = 2;
                    ActionWeights[AIActions.Fold] = 3;
                    break;
                case CardCollection.HandQuality.Cheated:
                    WinWeight = 3;
                    ActionWeights[AIActions.Raise] = 3;
                    ActionWeights[AIActions.Check] = 1;
                    ActionWeights[AIActions.Fold] = 0;
                    break;
                default:
                    WinWeight = 0;
                    break;
            }
        }

        public void UpdateHandQuality(CardCollection.HandQuality quality)
        {
            this.quality = quality;
            ActionWeights = new Dictionary<AIActions, float> { { AIActions.Raise, 0 }, { AIActions.Check, 0 }, { AIActions.Fold, 0 } };
            switch (quality)
            {
                case CardCollection.HandQuality.Good:
                    WinWeight = 2;
                    ActionWeights[AIActions.Raise] = 2;
                    ActionWeights[AIActions.Check] = 3;
                    ActionWeights[AIActions.Fold] = 1;
                    break;
                case CardCollection.HandQuality.Bad:
                    WinWeight = 1;
                    ActionWeights[AIActions.Raise] = 1;
                    ActionWeights[AIActions.Check] = 2;
                    ActionWeights[AIActions.Fold] = 3;
                    break;
                case CardCollection.HandQuality.Cheated:
                    WinWeight = 3;
                    ActionWeights[AIActions.Raise] = 3;
                    ActionWeights[AIActions.Check] = 1;
                    ActionWeights[AIActions.Fold] = 0;
                    break;
                default:
                    WinWeight = 0;
                    break;
            }
        }
    }

    public enum AIState
    {
        Idle,
        ActiveTurn,
    }

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

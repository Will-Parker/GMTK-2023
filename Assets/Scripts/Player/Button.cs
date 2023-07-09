using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    // AI Array
    //
    // Table1: Left Middle Right
    // Table2: Left Middle Right
    [SerializeField] public AI[] AiArray;
    [SerializeField] private Player player;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < AiArray.Length; i++)
        {
            AiArray[i].id = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        string ButtonName = "None";
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit Hit;
            if(Physics.Raycast(ray,out Hit))
            {
                ButtonName = Hit.transform.name;
            }
            switch(ButtonName)
            {
                case "LeftButton":
                    DetainPlayer(AiArray[player.CurrentScreen * 3]);
                    break;
                case "MiddleButton":
                    DetainPlayer(AiArray[(player.CurrentScreen * 3) + 1]);
                    break;
                case "RightButton":
                    DetainPlayer(AiArray[(player.CurrentScreen * 3) + 2]);
                    break;
            }
        }
    }

    void DetainPlayer(AI ai)
    {
        if(ai.AiState == AI.AIState.Detained)
        {
            Debug.Log("AI " + ai.id + " has already been detained!");
            return;
        }
        ai.AiState = AI.AIState.Detained;
        Debug.Log("AI " + ai.id + " has been " + ai.AiState);
    }
}

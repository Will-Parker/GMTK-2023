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
    //[SerializeField] public AI[] AiArray;
    [SerializeField] public GameManager[] gm;
    [SerializeField] private Player player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<AI> TableList = gm[player.CurrentScreen].GetGameParticipants();
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
                    if (TableList.Count > 1)
                    {
                        DetainPlayer(TableList[1]);
                    }
                    break;
                case "MiddleButton":
                    if (TableList.Count > 2)
                    {
                        DetainPlayer(TableList[2]);
                    }
                    break;
                case "RightButton":
                    if (TableList.Count > 3)
                    {
                        DetainPlayer(TableList[3]);
                    }
                    break;
            }
        } else if(Input.GetKeyDown(KeyCode.Z))
        {
            if (TableList.Count > 1)
            {
                DetainPlayer(TableList[1]);
            }
        } else if (Input.GetKeyDown(KeyCode.X))
        {
            if(TableList.Count > 2)
            {
                DetainPlayer(TableList[2]);
            }
            
        } else if (Input.GetKeyDown(KeyCode.C))
        {
            if (TableList.Count > 3)
            {
                DetainPlayer(TableList[3]);
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
        return;
    }
}

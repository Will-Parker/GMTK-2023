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
        List<Seat> TableList = gm[player.CurrentScreen].seats;
        /*string ButtonName = "None";
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
                    if (TableList[0].participant != null && TableList.Count > 1)
                    {
                        DetainPlayer(TableList[0].participant);
                    }
                    break;
                case "MiddleButton":
                    if (TableList[1].participant != null && TableList.Count > 2)
                    {
                        DetainPlayer(TableList[1].participant);
                    }
                    break;
                case "RightButton":
                    if (TableList[2].participant != null && TableList.Count > 3)
                    {
                        DetainPlayer(TableList[2].participant);
                    }
                    break;
            }
        } else */if(Input.GetKeyDown(KeyCode.Z))
        {
            if (TableList[0].participant != null && TableList.Count >= 1)
            {
                DetainPlayer(TableList[0].participant);
            }
        } else if (Input.GetKeyDown(KeyCode.X))
        {
            if(TableList[1].participant != null && TableList.Count >= 2)
            {
                DetainPlayer(TableList[1].participant);
            }
            
        } else if (Input.GetKeyDown(KeyCode.C) )
        {
            if (TableList[2].participant != null && TableList.Count >= 3)
            {
                DetainPlayer(TableList[2].participant);
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


    public void Detain1()
    {
        List<Seat> TableList = gm[player.CurrentScreen].seats;
        if (TableList[0].participant != null && TableList.Count >= 1)
        {
            DetainPlayer(TableList[0].participant);
        }
    }

    public void Detain2()
    {
        List<Seat> TableList = gm[player.CurrentScreen].seats;
        if (TableList[1].participant != null && TableList.Count >= 2)
        {
            DetainPlayer(TableList[1].participant);
        }
    }

    public void Detain3()
    {
        List<Seat> TableList = gm[player.CurrentScreen].seats;
        if (TableList[2].participant != null && TableList.Count >= 3)
        {
            DetainPlayer(TableList[2].participant);
        }
    }
}

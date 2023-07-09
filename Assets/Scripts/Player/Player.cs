using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Screen screen;
    public int CurrentScreen;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            CurrentScreen = screen.NextScreen();
        } else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CurrentScreen = screen.PrevScreen();
        } else if(Input.GetMouseButtonDown(0))
        {
            string ButtonName = "None";
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit Hit;
            if (Physics.Raycast(ray, out Hit))
            {
                ButtonName = Hit.transform.name;
            }
            switch (ButtonName)
            {
                case "NextButton":
                    CurrentScreen = screen.NextScreen();
                    break;
                case "PrevButton":
                    CurrentScreen = screen.PrevScreen();
                    break;
            }
        }

    }

}

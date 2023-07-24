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
        CurrentScreen = screen.CurrentScreen;
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            screen.NextScreen();
        } else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            screen.PrevScreen();
        } /*else if(Input.GetMouseButtonDown(0))
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
                    screen.NextScreen();
                    break;
                case "PrevButton":
                    screen.PrevScreen();
                    break;
            }
        }*/

    }

    public void Screen1()
    {
        screen.PrevScreen();
    }

    public void Screen2()
    {
        screen.NextScreen();
    }
}

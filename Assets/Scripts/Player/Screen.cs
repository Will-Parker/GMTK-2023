using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Screen : MonoBehaviour
{
    public int CurrentScreen = 0;
    [SerializeField] Material[] ScreenMats;
    Renderer ScreenRenderer;
    void Start()
    {
        ScreenRenderer = GetComponent<Renderer>();
        ScreenRenderer.sharedMaterial = ScreenMats[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int NextScreen()
    {
        if(CurrentScreen == 0)
        {
            CurrentScreen = 1;
            ScreenRenderer.sharedMaterial = ScreenMats[CurrentScreen];
        }

        return CurrentScreen;
    }

    public int PrevScreen()
    {
        if (CurrentScreen == 1)
        {
            CurrentScreen = 0;
            ScreenRenderer.sharedMaterial = ScreenMats[CurrentScreen];
        }
        return CurrentScreen;
    }


}

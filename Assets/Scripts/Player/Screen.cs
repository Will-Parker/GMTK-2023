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

    public void NextScreen()
    {
        if(CurrentScreen == 0)
        {
            AudioManager.instance.Play("Switch Camera");
            CurrentScreen = 1;
            ScreenRenderer.sharedMaterial = ScreenMats[CurrentScreen];
        }
    }

    public void PrevScreen()
    {
        if (CurrentScreen == 1)
        {
            AudioManager.instance.Play("Switch Camera");
            CurrentScreen = 0;
            ScreenRenderer.sharedMaterial = ScreenMats[CurrentScreen];
        }
    }


}

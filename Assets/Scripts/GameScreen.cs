using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScreen : MonoBehaviour
{
    public void LoadGame()
    {
        AudioManager.instance.Play("Button");
        SceneManager.LoadScene(1);
    }
    public void ExitGame()
    {
        AudioManager.instance.Play("Button");
        Application.Quit();
    }

    public void ToTitle()
    {
        AudioManager.instance.Play("Button");
        SceneManager.LoadScene(0);
    }
}

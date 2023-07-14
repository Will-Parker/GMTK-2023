using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    private void Start()
    {
        Sound s = Array.Find(AudioManager.instance.sounds, sound => sound.name == "Music");
        if (s == null)
        {
            Debug.LogWarning("Sound: Music not found!");
            return;
        }
        if (!s.isPlaying)
        {
            AudioManager.instance.Play("Music");
            s.isPlaying = true;
        }
    }
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
}

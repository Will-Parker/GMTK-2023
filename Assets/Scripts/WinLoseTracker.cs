using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLoseTracker : MonoBehaviour
{
    public float gameLength;
    public float remainingGameLength;
    public Slider slider;
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 20f;
        remainingGameLength = gameLength;
        slider.maxValue = gameLength;
        SetSliderValue(remainingGameLength);
    }

    // Update is called once per frame
    void Update()
    {
        remainingGameLength -= Time.deltaTime;
        SetSliderValue(remainingGameLength);
        if (remainingGameLength <= 0)
        {
            StopAllCoroutines();
            SceneManager.LoadScene("GameOver");
        }
        List<AI> ais = new List<AI>(FindObjectsOfType<AI>());
        var b = ais.RemoveAll(a => (a.GetAIType() == AI.AIType.Dealer || a.GetAIType() == AI.AIType.Normal));
        if (ais.Count == 0)
        {
            StopAllCoroutines();
            SceneManager.LoadScene("Win");
        }
    }

    public void SetSliderValue(float value)
    {
        slider.value = value;
    }
}

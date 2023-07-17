using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseTracker : MonoBehaviour
{
    public float gameLength;
    public float remainingGameLength;
    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 20f;
        remainingGameLength = gameLength;
    }

    // Update is called once per frame
    void Update()
    {
        remainingGameLength -= Time.deltaTime;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreUI;

    public int score = 0;
    public bool isScoring = false;

    public PauseOnStart pos;

    void Start()
    {
        if (!pos.hasStarted) return;

        StartCoroutine(ScoreChange());
    }

    void Update()
    {
        if (!pos.hasStarted) return;

        if (!isScoring) StartCoroutine(ScoreChange());
    }

    public IEnumerator ScoreChange()
    {
        isScoring = true;

        score++;

        PlayerPrefs.SetInt("Score", score);

        int hs = PlayerPrefs.GetInt("High Score");
        int s = PlayerPrefs.GetInt("Score");

        if (s >= hs)
        {
            PlayerPrefs.SetInt("High Score", s);
        }

        scoreUI.text = "Score: " + score.ToString();

        yield return new WaitForSeconds(1.4f);

        isScoring = false;
    }
}

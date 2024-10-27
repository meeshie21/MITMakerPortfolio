using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OPS.AntiCheat.Field;
using OPS.AntiCheat.Prefs;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject spawner;
    [SerializeField] private Stopwatch stopwatch;
    [SerializeField] private TMP_Text time;
    [SerializeField] private TMP_Text highScore;

    private int gameDifficulty, tier;
    private ProtectedFloat xpMultiplier;
    [SerializeField] private ProtectedFloat xpRate;
    [SerializeField] private PlayfabManager playfabManager;

    private ProtectedFloat seconds;
    private bool newHighScore;

    void Start()
    {
        /*ProtectedPlayerPrefs.SetInt("xp", 2500);
        ProtectedPlayerPrefs.SetInt("tier", 1);*/
        MovingPlatform.speed = 5f;
        ObstacleGroup.rotSpeed = 150f;
        gameDifficulty = ProtectedPlayerPrefs.GetInt("difficultyLevel");

        switch(gameDifficulty)
        {
            case -1:
                xpMultiplier = 0.5f;
                break;
            case 0:
                xpMultiplier = 1;
                break;
            case 1:
                xpMultiplier = 2;
                break;
            case 2:
                xpMultiplier = 4;
                break;
            case 3:
                xpMultiplier = 8;
                break;
        }

        newHighScore = false;
    }

    public void GameOver()
    {
        seconds = stopwatch.GetSeconds();

        ProtectedInt32 inGameXp = (int)(seconds * xpRate * xpMultiplier);
        ProtectedInt32 xpToLoad =  ProtectedPlayerPrefs.GetInt("xp") + inGameXp;

        ProtectedInt32 tierToLoad = ProtectedPlayerPrefs.GetInt("tier");
        if (xpToLoad > (int)(Mathf.Pow(tierToLoad, 2) * 500)) tierToLoad = tierToLoad + 1;

        ProtectedPlayerPrefs.SetInt("xp", xpToLoad);
        ProtectedPlayerPrefs.SetInt("tier", tierToLoad);

        Destroy(spawner.gameObject);
        MovingPlatform.speed = 0f;
        ObstacleGroup.rotSpeed = 0f;
        StartCoroutine(GameOverPanel());

        switch (gameDifficulty)
        {
            case -1:
                if (seconds > ProtectedPlayerPrefs.GetInt("starterHighScore"))
                {
                    ProtectedPlayerPrefs.SetInt("starterHighScore", (ProtectedInt32)seconds);
                    newHighScore = true;
                }
                break;
            case 0:
                if (seconds > ProtectedPlayerPrefs.GetInt("easyHighScore"))
                {
                    ProtectedPlayerPrefs.SetInt("easyHighScore", (ProtectedInt32)seconds);
                    newHighScore = true;
                }
                break;
            case 1:
                if(seconds > ProtectedPlayerPrefs.GetInt("normalHighScore"))
                {
                    ProtectedPlayerPrefs.SetInt("normalHighScore", (ProtectedInt32)seconds);
                    newHighScore = true;
                }
                break;
            case 2:
                if (seconds > ProtectedPlayerPrefs.GetInt("hardHighScore"))
                {
                    ProtectedPlayerPrefs.SetInt("hardHighScore", (ProtectedInt32)seconds);
                    newHighScore = true;
                }
                break;
            case 3:
                if (seconds > ProtectedPlayerPrefs.GetInt("deathHighScore"))
                {
                    ProtectedPlayerPrefs.SetInt("deathHighScore", (ProtectedInt32)seconds);
                    newHighScore = true;
                }
                break;
        }

        playfabManager.SendLeaderboard(ProtectedPlayerPrefs.GetInt("xp"));
    }

    private IEnumerator GameOverPanel()
    {
        yield return new WaitForSeconds(0.5f);
        gameOverPanel.SetActive(true);
        time.text = Stopwatch.TimeText(seconds);

        switch (gameDifficulty)
        {
            case -1:
                highScore.text = newHighScore ? "NEW HIGHSCORE" : "HIGHSCORE: " + Stopwatch.TimeText(ProtectedPlayerPrefs.GetInt("starterHighScore"));
                break;
            case 0:
                highScore.text = newHighScore ? "NEW HIGHSCORE" : "HIGHSCORE: " + Stopwatch.TimeText(ProtectedPlayerPrefs.GetInt("easyHighScore"));
                break;
            case 1:
                highScore.text = newHighScore ? "NEW HIGHSCORE" : "HIGHSCORE: " + Stopwatch.TimeText(ProtectedPlayerPrefs.GetInt("normalHighScore"));
                break;
            case 2:
                highScore.text = newHighScore ? "NEW HIGHSCORE" : "HIGHSCORE: " + Stopwatch.TimeText(ProtectedPlayerPrefs.GetInt("hardHighScore"));
                break;
            case 3:
                highScore.text = newHighScore ? "NEW HIGHSCORE" : "HIGHSCORE: " + Stopwatch.TimeText(ProtectedPlayerPrefs.GetInt("deathHighScore"));
                break;
        }

        StopCoroutine(GameOverPanel());
    }
}

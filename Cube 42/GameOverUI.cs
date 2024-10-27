using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public bool isRunning = false;

    public Text difficultyUI;
    public float difficulty;

    public Text coinsUI;
    public int coins;

    public Text scoreUI;
    public int score;

    public Text highScoreUI;
    public int highScore;

    public GameObject adUI;

    public PauseOnStart pos;

    void Start()
    {
        
    }

    void Update()
    {
        score = PlayerPrefs.GetInt("Score");
        scoreUI.text = "Score: " + score.ToString();


        highScore = PlayerPrefs.GetInt("High Score");
        highScoreUI.text = "High Score: " + highScore.ToString();

        coins = PlayerPrefs.GetInt("CoinGain");

        if(PlayerPrefs.GetInt("CoinGain") < 10)
        {
            coinsUI.text = "+ 000" + coins.ToString();
        }

        if (PlayerPrefs.GetInt("CoinGain") < 100 && PlayerPrefs.GetInt("CoinGain") > 10)
        {
            coinsUI.text = "+ 00" + coins.ToString();
        }

        if (PlayerPrefs.GetInt("CoinGain") < 1000 && PlayerPrefs.GetInt("CoinGain") > 100)
        {
            coinsUI.text = "+ 0" + coins.ToString();
        }

        if (PlayerPrefs.GetInt("CoinGain") < 10000 && PlayerPrefs.GetInt("CoinGain") > 1000)
        {
            coinsUI.text = "+ " + coins.ToString();
        }

        if (PlayerPrefs.GetInt("CoinGain") < 100000 && PlayerPrefs.GetInt("CoinGain") > 10000)
        {
            coinsUI.text = "0" + coins.ToString();
        }

        if(PlayerPrefs.GetInt("InterstitialNumber") == 2)
        {
            adUI.SetActive(true);
        } 
        
        else
        {
            adUI.SetActive(false);
        }


        difficulty = PlayerPrefs.GetFloat("Difficulty");
        difficultyUI.text = "Speed: " + difficulty.ToString() + "mph";
    }
}

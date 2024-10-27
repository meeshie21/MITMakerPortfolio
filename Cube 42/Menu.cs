using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Text coinsUI;
    public int coins;

    void Start()
    {
        Application.targetFrameRate = 60;

        Time.timeScale = 1f;

        if(Screen.dpi < 250)
        {
            Screen.SetResolution(1334, 750, true);
        }
    }

    void Update()
    {
        coins = PlayerPrefs.GetInt("CoinTotal");

        if (PlayerPrefs.GetInt("CoinTotal") < 10)
        {
            coinsUI.text = "00000" + coins.ToString();
        }

        if (PlayerPrefs.GetInt("CoinTotal") < 100 && PlayerPrefs.GetInt("CoinTotal") > 10)
        {
            coinsUI.text = "0000" + coins.ToString();
        }

        if (PlayerPrefs.GetInt("CoinTotal") < 1000 && PlayerPrefs.GetInt("CoinTotal") > 100)
        {
            coinsUI.text = "000" + coins.ToString();
        }

        if (PlayerPrefs.GetInt("CoinTotal") < 10000 && PlayerPrefs.GetInt("CoinTotal") > 1000)
        {
            coinsUI.text = "00" + coins.ToString();
        }

        if (PlayerPrefs.GetInt("CoinTotal") < 100000 && PlayerPrefs.GetInt("CoinTotal") > 10000)
        {
            coinsUI.text = "0" + coins.ToString();
        }
    }
}

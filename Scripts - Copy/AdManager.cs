using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    // Start is called before the first frame update
    bool firstPlay;

    void Awake()
    {
        if (Application.isEditor == false)
        {
            if (PlayerPrefs.GetInt("FirstPlay", 1) == 1)
            {
                firstPlay = true;
                PlayerPrefs.SetInt("FirstPlay", 0);
                PlayerPrefs.Save();
            }

            else
                firstPlay = false;
        }
    }

    void Start()
    {
        if (firstPlay == false) return;
        for (int i = 1; i < 50; i++)
        {
            PlayerPrefs.SetInt(i.ToString() + "IsBought", 0);
            PlayerPrefs.SetInt(i.ToString() + "IsEquipped", 0);
        }

        PlayerPrefs.SetInt(0.ToString() + "IsBought", 1);
        PlayerPrefs.SetInt(0.ToString() + "IsEquipped", 1);

        PlayerPrefs.SetInt("ColorValue", 1);
        PlayerPrefs.SetInt("AudioValue", 1);
        PlayerPrefs.SetInt("MeshNumber", 0);
        PlayerPrefs.SetInt("High Score", 0);
        PlayerPrefs.SetInt("CoinTotal", 0);
        PlayerPrefs.SetInt("InterstitialNumber", 1);
        PlayerPrefs.SetInt("MutedSFX", 1);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

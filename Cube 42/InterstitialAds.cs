using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAds : MonoBehaviour
{
    public string gameId = "3876256";
    public bool testMode = false;
    public bool adIsReady = false;

    void Start()
    {
        PlayerPrefs.SetInt("InterstitialNumber", PlayerPrefs.GetInt("InterstitialNumber") + 1);

        if(PlayerPrefs.GetInt("InterstitialNumber") == 4)
        {
            PlayerPrefs.SetInt("InterstitialNumber", 1);
            adIsReady = true;
        }

        else
        {
            adIsReady = false;
        }

        Advertisement.Initialize(gameId, testMode);

        if (!adIsReady) return;
        StartCoroutine(ShowInterstitialAdWhenInitialized());
    }

    IEnumerator ShowInterstitialAdWhenInitialized()
    {
        while(!Advertisement.IsReady())
        {
            yield return null;
        }

        Advertisement.Show();
    }
}
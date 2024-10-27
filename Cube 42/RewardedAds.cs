using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class RewardedAds : MonoBehaviour, IUnityAdsListener
{
    public string gameId = "3876256";
    public string placement = "rewardedVideo";
    public bool testMode = false;

    void Start()
    {

    }

    public void OnUserInitiatedAd()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
        StartCoroutine(ShowRewardedAdWhenInitialized());
    }

    IEnumerator ShowRewardedAdWhenInitialized()
    {
        while(!Advertisement.IsReady(placement))
        {
            yield return null;
        }

        Advertisement.Show(placement);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
       if(showResult == ShowResult.Finished)
       {
            PlayerPrefs.SetInt("CoinTotal", PlayerPrefs.GetInt("CoinTotal") + 100);
       }
    }

    public void OnUnityAdsDidStart(string placementId)
    {

    }

    public void OnUnityAdsReady(string placementId)
    {
       
    }

    public void OnUnityAdsDidError(string message)
    {

    }
}
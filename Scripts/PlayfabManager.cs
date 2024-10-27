using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using OPS.AntiCheat.Field;
using OPS.AntiCheat.Prefs;

public class PlayfabManager : MonoBehaviour
{
    /*private string MyPlayfabID;

    void GetAccountInfo()
    {
        GetAccountInfoRequest request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, OnAccountInfoGet, OnError);
    }

    void OnAccountInfoGet(GetAccountInfoResult result)
    {
        MyPlayfabID = result.AccountInfo.PlayFabId;
    }*/

    // Start is called before the first frame update
    void Start()
    {
        Login();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult result)
    {
        Debug.Log("Success With Login Request");
        GetLeaderboardAroundPlayer();
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error with PlayFab Request");
        Debug.Log(error.GenerateErrorReport());
    }

    public void SendLeaderboard(int xpLevel)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate 
                {
                    StatisticName = "RankingXP",
                    Value = xpLevel 
                }
                
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successful Leaderboard Send!");
    }

    public void GetLeaderboardAroundPlayer()
    {
        var request = new GetLeaderboardAroundPlayerRequest()
        {
            StatisticName = "RankingXP",
            MaxResultsCount = 1
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnLeaderboardGet, OnRankError);
    }

    void OnLeaderboardGet(GetLeaderboardAroundPlayerResult result)
    {
        foreach(var item in result.Leaderboard)
        {
           ProtectedPlayerPrefs.SetInt("rank", item.Position + 1);
        }
    }

    void OnRankError(PlayFabError error)
    {
        ProtectedPlayerPrefs.SetInt("rank", -1);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabTest : MonoBehaviour, IPunInstantiateMagicCallback
{
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _leaderboardButton;
    [SerializeField] private Button _leaderboardSetButton;
    public string playFabTitleId = string.Empty;

    private int _currentScore = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _loginButton.onClick.AddListener(DoLogin);
        _leaderboardButton.onClick.AddListener(GetLeaderboard);
        _leaderboardSetButton.onClick.AddListener(SetScore);
        
    }

    void DoLogin()
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest() { TitleId = this.playFabTitleId, CustomId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginCallback, ErrorCallback, null);
    }

    private void OnLoginCallback(LoginResult result)
    {
        Debug.Log(result.ToString());
    }

    private void ErrorCallback(PlayFabError error)
    {
        Debug.Log(error.ToString());
    }

    void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest();
        request.StatisticName = "Headshots";
        request.Version = 0;
        request.StartPosition = 0;
        PlayFabClientAPI.GetLeaderboard(request, ResultCallback_Leaderboard, ErrorCallback_Leaderboard);
    }

    private void SetScore()
    {
        try
        {
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                _currentScore++;
                var request = new UpdatePlayerStatisticsRequest();
                request.Statistics = new List<StatisticUpdate> {new StatisticUpdate {StatisticName = "Headshots", Version = 0, Value = _currentScore}};
                PlayFabClientAPI.UpdatePlayerStatistics(request, result =>
                {
                    Debug.Log("Succefull: " + result.Request.ToJson());
                }, error =>
                {
                    Debug.LogError(error.GenerateErrorReport());
                });
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
       
    }

    private void ErrorCallback_Leaderboard(PlayFabError error)
    {
        Debug.Log(error.ToString());
    }

    private void ResultCallback_Leaderboard(GetLeaderboardResult result)
    {
        Debug.Log(result.ToString());
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
    }
}

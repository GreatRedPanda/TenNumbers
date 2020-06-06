using UnityEngine;
using System.Collections.Generic;
using GooglePlayGames;

using System;
using GooglePlayGames.BasicApi;
using System.Text;

[System.Serializable]
public class LeaderBoardData
{
    public string ID;
}
public class GPlayServiceController : MonoBehaviour
{
    public event System.Action OnGameLoaded;

    public static GPlayServiceController Instance { get; private set; }

  
    private bool mAuthenticating = false;
    private string mAuthProgressMessage = "Signing in and loading your progress...";
   public string LeaderboardId;
  //  public List<LeaderBoardData> leaderBoards = new List<LeaderBoardData>();
    bool auth = false;
    bool recordsNorm = false;

    String AUTHpROGRESS = "";


    List<PlayGamesLeaderboard> playGamesLeaderboards = new List<PlayGamesLeaderboard>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
             .Build()
             ;
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            AUTHpROGRESS = "configInit";

            PlayGamesPlatform.Activate();
        
            AUTHpROGRESS = "activated";
        }
        else Destroy(gameObject);

        DontDestroyOnLoad(Instance);
    }





    public void Authenticate()
    {
        if (Authenticated || Authenticating)
        {
            if (Authenticated)
                AUTHpROGRESS = "Authenticated";
            else if (Authenticating)
                AUTHpROGRESS = "Authenticating";
            return;
        }


        mAuthenticating = true;


        Social.localUser.Authenticate((bool success) =>
        {

            OnAuthenticateReport(success);
        });
    }



    void OnAuthenticateReport(bool success)
    {


        auth = true;
        mAuthenticating = false;
        if (success)
            AUTHpROGRESS = "success";
        else
            AUTHpROGRESS = "failed";
        AddScoreToLeaderboard();
        AUTHpROGRESS = "records reported";


    }
    public void AddScoreToLeaderboard()
    {

        //foreach (var item in leaderBoards)
        {
            int highScore = GameData.HighScore;
            AUTHpROGRESS = "record 1 report";
            if (highScore > 0)
                PlayGamesPlatform.Instance.ReportScore((long)highScore, LeaderboardId, success => { });
        }

    }

    public void ShowLeaderboardsUI(Action<bool> callback)
    {


        if ((!Authenticated && !Authenticating))
        {
            mAuthenticating = true;
            PlayGamesPlatform.Instance.localUser.Authenticate((bool success) =>
            {

                OnAuthenticateReport(success);
                if (!success)
                    callback.Invoke(success);
                else
                    PlayGamesPlatform.Instance.ShowLeaderboardUI(null, uiStatus =>
                    {

                        callback.Invoke(uiStatus == UIStatus.Valid);
                    }
                    );
            });
        }
        else
        {
            AddScoreToLeaderboard();
            AUTHpROGRESS = "records reported";
            PlayGamesPlatform.Instance.ShowLeaderboardUI(null, uiStatus =>
            {

                callback.Invoke(uiStatus == UIStatus.Valid);
            }


            );

        }

    }






    public bool Authenticated
    {
        get
        {
            return PlayGamesPlatform.Instance.IsAuthenticated();
            //return Social.Active.localUser.authenticated;
        }
    }
    public bool Authenticating
    {
        get
        {
            return mAuthenticating;
        }
    }

    public void SignOut()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
    }


    private void OnGUI()
    {


    }
}




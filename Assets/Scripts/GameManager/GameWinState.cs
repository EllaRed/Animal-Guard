using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
//using UnityEngine.Analytics.Experimental;
#endif
public class GameWinState : AState {

 
/// <summary>
/// state pushed on top of the GameManager when the player wins.
/// </summary>
 public TrackManager trackManager;
    public Canvas canvas;
    public Text scoretext;
    public Text leveltext;
    public MissionUI missionPopup;
    public GameObject[] stars;
    public AudioClip gamewinTheme;
    public Levelcontroller levelcontroller;
    

    public GameObject addButton;

    protected bool m_CoinCredited = false;

    public override void Enter(AState from)
    {
        canvas.gameObject.SetActive(true);
        scoretext.text = trackManager.score.ToString();
        leveltext.text = (manager.currentlevel +1).ToString();
        stars[0].SetActive(false);
        stars[1].SetActive(false);
        stars[2].SetActive(false);


        if (PlayerData.instance.AnyMissionComplete())
            missionPopup.Open();
        else
            missionPopup.gameObject.SetActive(false);

        m_CoinCredited = false;

        CreditCoins();

        if (MusicPlayer.instance.GetStem(0) != gamewinTheme)
        {
            MusicPlayer.instance.SetStem(0, gamewinTheme);
            StartCoroutine(MusicPlayer.instance.RestartAllStems());
        }
        int j = 0;
        int min = manager.minscore;
        int f = min;
        int g = min * 2;

        for (int i = min; i < g; i++)
        {

            if (trackManager.score >= f)
            {
                stars[j].SetActive(true);
                

            }
            f += min / 2;
            i += min / 2;
            i -= 2;
            j++;
        }
    }

    public override void Exit(AState to)
    {
        canvas.gameObject.SetActive(false);
        manager.minscore = 0;
        FinishRun();
    }

    public override string GetName()
    {
        return "GameWin";
    }

    public override void Tick()
    {

    }

   

    public void GoToStore()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("testshop", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }


    public void GoToLoadout()
    {
        trackManager.isRerun = false;
        manager.SwitchState("Loadout");
    }

  

    protected void CreditCoins()
    {
        if (m_CoinCredited)
            return;

        // -- give coins gathered
        PlayerData.instance.coins += trackManager.characterController.coins;
        PlayerData.instance.premium += trackManager.characterController.premium;
        PlayerData.instance.Insertlvlscore(manager.currentlevel, trackManager.score);
        Levelreward();
        if (manager.currentlevel == PlayerData.instance.levelat)
        {
            PlayerData.instance.levelat += 1;
        }
        PlayerData.instance.Save();

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        var transactionId = System.Guid.NewGuid().ToString();
        var transactionContext = "gameplay";
        var level = PlayerData.instance.rank.ToString();
        var itemType = "consumable";
        
        if (trackManager.characterController.coins > 0)
        {
            AnalyticsEvent.ItemAcquired(
                AcquisitionType.Soft, // Currency type
                transactionContext,
                trackManager.characterController.coins,
                "fishbone",
                PlayerData.instance.coins,
                itemType,
                level,
                transactionId
            );
        }

        if (trackManager.characterController.premium > 0)
        {
            AnalyticsEvent.ItemAcquired(
                AcquisitionType.Premium, // Currency type
                transactionContext,
                trackManager.characterController.premium,
                "anchovies",
                PlayerData.instance.premium,
                itemType,
                level,
                transactionId
            );
        }
#endif 

        m_CoinCredited = true;
    }

    protected void Levelreward()
    {
        Levelsettings level = levelcontroller.levels[manager.currentlevel].gameObject.GetComponent(typeof(Levelsettings)) as Levelsettings;
        if (manager.currentlevel == PlayerData.instance.levelat)
        {
            if ( level.rewardtype == 1)
            {
                PlayerData.instance.coins += level.rewardmoney;
            }
            if (level.rewardtype == 2)
            {
                PlayerData.instance.premium += level.rewardgem;
            }
            if (level.rewardtype == 3)
            {
                PlayerData.instance.rewardmultiplier += level.rewardmultiplier;
            }
            if (level.rewardtype == 4)
            {
                PlayerData.instance.AddTheme(level.rewardlocation);
            }
        }
    }
    protected void FinishRun()
    {

        PlayerData.instance.InsertScore(trackManager.score, PlayerData.instance.previousName);
       
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Note: make sure to add 'using GooglePlayGames'
            PlayGamesPlatform.Instance.ReportScore(trackManager.score,
                GPGSIds.leaderboard_highest_of_the_high_scores,
                (bool success) =>
                {
                    Debug.Log("Leaderboard update success: " + success);
                });
            WriteUpdatedScore();
        }
        CharacterCollider.DeathEvent de = trackManager.characterController.characterCollider.deathData;
        //register data to analytics
#if UNITY_ANALYTICS
        AnalyticsEvent.GameOver(null, new Dictionary<string, object> {
            { "coins", de.coins },
            { "premium", de.premium },
            { "score", de.score },
            { "distance", de.worldDistance },
            { "obstacle",  de.obstacleType },
            { "theme", de.themeUsed },
            { "character", de.character },
        });
#endif

        PlayerData.instance.Save();

        trackManager.End();
    }

    public void WriteUpdatedScore()
    {
        // Local variable
        ISavedGameMetadata currentGame = null;

        // CALLBACK: Handle the result of a write
        Action<SavedGameRequestStatus, ISavedGameMetadata> writeCallback =
        (SavedGameRequestStatus status, ISavedGameMetadata game) => {
            Debug.Log("(Lollygagger) Saved Game Write: " + status.ToString());
        };

        // CALLBACK: Handle the result of a binary read
        Action<SavedGameRequestStatus, byte[]> readBinaryCallback =
        (SavedGameRequestStatus status, byte[] data) => {
            Debug.Log("(Lollygagger) Saved Game Binary Read: " + status.ToString());
            if (status == SavedGameRequestStatus.Success)
            {
                // Read score from the Saved Game
                int score = 0;
                try
                {
                    string scoreString = System.Text.Encoding.UTF8.GetString(data);
                    score = Convert.ToInt32(scoreString);
                }
                catch (Exception e)
                {
                    Debug.Log("(Lollygagger) Saved Game Write: convert exception");
                }

                // Increment score, convert to byte[]
                int newScore = score + trackManager.hitcount;
                string newScoreString = Convert.ToString(newScore);
                byte[] newData = System.Text.Encoding.UTF8.GetBytes(newScoreString);

                // Write new data
                //Debug.Log("(Lollygagger) Old Score: " + score.ToString());
                //Debug.Log("(Lollygagger) mHits: " + mHits.ToString());
                //Debug.Log("(Lollygagger) New Score: " + newScore.ToString());
                WriteSavedGame(currentGame, newData, writeCallback);

                // Extra credit for me..
                PlayGamesPlatform.Instance.ReportScore(newScore,
                    GPGSIds.leaderboard_doggies_and_rodents_blasted,
                    (bool success) =>
                    {
                        Debug.Log("(Lollygagger) Leaderboard update success: " + success);
                    });

            }
        };

        // CALLBACK: Handle the result of a read, which should return metadata
        Action<SavedGameRequestStatus, ISavedGameMetadata> readCallback =
        (SavedGameRequestStatus status, ISavedGameMetadata game) => {
            Debug.Log("(Lollygagger) Saved Game Read: " + status.ToString());
            if (status == SavedGameRequestStatus.Success)
            {
                // Read the binary game data
                currentGame = game;
                PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game,
                                                    readBinaryCallback);

            }
        };

        // Read the current data and kick off the callback chain
        Debug.Log("(Lollygagger) Saved Game: Reading");
        ReadSavedGame("file_total_hits", readCallback);
    }

    public void ReadSavedGame(string filename,
                           Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            filename,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            callback);
    }

    public void WriteSavedGame(ISavedGameMetadata game, byte[] savedData,
                               Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {

        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder()
            .WithUpdatedPlayedTime(TimeSpan.FromMinutes(game.TotalTimePlayed.Minutes + 1))
            .WithUpdatedDescription("Saved at: " + System.DateTime.Now);

        // You can add an image to saved game data (such as as screenshot)
        // byte[] pngData = <PNG AS BYTES>;
        // builder = builder.WithUpdatedPngCoverImage(pngData);

        SavedGameMetadataUpdate updatedMetadata = builder.Build();

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, callback);
    }
    //----------------
}

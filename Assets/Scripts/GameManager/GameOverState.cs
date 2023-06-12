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
using System.Collections.Generic;
 
/// <summary>
/// state pushed on top of the GameManager when the player dies.
/// </summary>
public class GameOverState : AState
{
    public TrackManager trackManager;
    public Canvas canvas;
    public MissionUI missionPopup;

	public AudioClip gameOverTheme;

	public Leaderboard miniLeaderboard;
	public Leaderboard fullLeaderboard;

    public GameObject addButton,ld;

	protected bool m_CoinCredited = false;

    public override void Enter(AState from)
    {
        canvas.gameObject.SetActive(true);
        if (manager.minscore!=0)
        {
            miniLeaderboard.gameObject.SetActive(false);
            
            if(ld!=null)
            ld.SetActive(false);
        }
        else
        {
            ld.SetActive(true);
            miniLeaderboard.playerEntry.inputName.text = PlayerData.instance.previousName;

            miniLeaderboard.playerEntry.score.text = trackManager.score.ToString();
            miniLeaderboard.Populate();
        }
        if (PlayerData.instance.AnyMissionComplete())
            missionPopup.Open();
        else
            missionPopup.gameObject.SetActive(false);

		m_CoinCredited = false;

		CreditCoins();

		if (MusicPlayer.instance.GetStem(0) != gameOverTheme)
		{
            MusicPlayer.instance.SetStem(0, gameOverTheme);
			StartCoroutine(MusicPlayer.instance.RestartAllStems());
        }
    }

	public override void Exit(AState to)
    {
        canvas.gameObject.SetActive(false);
        FinishRun();
    }

    public override string GetName()
    {
        return "GameOver";
    }

    public override void Tick()
    {
        
    }

	public void OpenLeaderboard()
	{
		fullLeaderboard.forcePlayerDisplay = false;
		fullLeaderboard.displayPlayer = true;
		fullLeaderboard.playerEntry.playerName.text = miniLeaderboard.playerEntry.inputName.text;
		fullLeaderboard.playerEntry.score.text = trackManager.score.ToString();

		fullLeaderboard.Open();
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

    public void RunAgain()
    {
        trackManager.isRerun = false;
        manager.SwitchState("Game");
    }

    protected void CreditCoins()
	{
		if (m_CoinCredited)
			return;

		// -- give coins gathered
		PlayerData.instance.coins += trackManager.characterController.coins;
		PlayerData.instance.premium += trackManager.characterController.premium;

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

	protected void FinishRun()
    {
		if(miniLeaderboard.playerEntry.inputName.text == "")
		{
            if (PlayGamesPlatform.Instance.localUser.authenticated)
            {
                miniLeaderboard.playerEntry.inputName.text = Social.localUser.userName;
            }
            else
            {
                miniLeaderboard.playerEntry.inputName.text = "Player..player?";
            }

		}
		else
		{
			PlayerData.instance.previousName = miniLeaderboard.playerEntry.inputName.text;
		}

        PlayerData.instance.InsertScore(trackManager.score, miniLeaderboard.playerEntry.inputName.text );
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


        manager.minscore = 0;
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

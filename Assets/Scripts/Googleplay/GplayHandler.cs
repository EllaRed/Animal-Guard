using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.UI;

public class GplayHandler : MonoBehaviour
{
    public Text signInButtonText;
    public Text authStatus;
    public Text cloudcoins;
    //public Text hardcoins;
    //public GameObject achButton;
    //public GameObject ldrButton;
    public GameObject connection_status_good, connection_status_bad;

    // Start is called before the first frame update
    void Start()
    {
        //PlayerData.Create();
        //PlayGamesClientConfiguration config = new
        //   PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();


        //// Enable debugging output (recommended)
        //PlayGamesPlatform.DebugLogEnabled = true;

        //// Initialize and activate the platform
        //PlayGamesPlatform.InitializeInstance(config);
        //PlayGamesPlatform.Activate();

        //// PASTE THESE LINES AT THE END OF Start()


        //// Try silent sign-in (second parameter is isSilent)
        //PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
    }

    // Update is called once per frame
    void Update()
    {

        //achButton.SetActive(Social.localUser.authenticated);
        //ldrButton.SetActive(Social.localUser.authenticated);
        if (Social.localUser.authenticated)
        {
            signInButtonText.text = "Sign out";

            // Show the user's name
            authStatus.text = "Signed in as: " + Social.localUser.userName;

            if (PlayerData.instance.hasloaded)
            {
                connection_status_good.SetActive(true);
                connection_status_bad.SetActive(false);
            }
            else
            {
                connection_status_good.SetActive(false);
                connection_status_bad.SetActive(true);
            }
        }
        else
        {
            connection_status_good.SetActive(false);
            connection_status_bad.SetActive(true);

        }
        
    }

    public void SignIn()
    {
        Debug.Log("signInButton clicked!");
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Sign in with Play Game Services, showing the consent dialog
            // by setting the second parameter to isSilent=false.
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);

        }
        else
        {
            // Sign out of play games
            PlayGamesPlatform.Instance.SignOut();
            PlayerData.instance.hasloaded = false;
            PlayerData.NewSave();

            // Reset UI
            signInButtonText.text = "Sign In";
            authStatus.text = "";
        }
    }
    public void SignInCallback(bool success)
    {
        if (success)
        {

            PlayerData.instance.Read();
            PlayerData.instance.isloading = true;
            // Change sign-in button text
            signInButtonText.text = "Sign out";

            // Show the user's name
            authStatus.text = "Signed in as: " + Social.localUser.userName;
        }
        else
        {


            // Show failure message
            signInButtonText.text = "Sign in";
            authStatus.text = "Sign-in failed";
        }
    }
    public void ShowAchievements()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }

    }

    public void ShowLeaderboards()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }

    }

    public void ADD()
    {
        PlayerData.instance.coins += 100;
        PlayerData.instance.Save();
        cloudcoins.text= PlayerData.instance.coins.ToString();
    }

    public void minus()
    {
        PlayerData.instance.coins -= 100;
        PlayerData.instance.Save();
        cloudcoins.text = PlayerData.instance.coins.ToString();
    }
}



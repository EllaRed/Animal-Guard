using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;


public class Levelcontroller : MonoBehaviour {
    public Button[] levels;
    public bool dev;
    public void Start()
    {
        int levelat = PlayerData.instance.levelat;
        if (!dev)
        {
            for (int i = 0; i < levels.Length; i++)
            {
                if (i > levelat)
                {
                    levels[i].interactable = false;
                }
            }
        }

        if (Social.localUser.authenticated)
        { // area unlock achievements
            if (levelat==6)
            {
                PlayGamesPlatform.Instance.ReportProgress(
                        GPGSIds.achievement_into_the_night,
                        100.0f, (bool success) =>
                        {
                            Debug.Log(" city night unlock: " +
                                  success);
                        });
            }
            if (levelat == 16)
            {
                PlayGamesPlatform.Instance.ReportProgress(
                        GPGSIds.achievement_everythings_better_in_black_and_white,
                        100.0f, (bool success) =>
                        {
                            Debug.Log(" city night all clear: " +
                                  success);
                        });

                PlayGamesPlatform.Instance.ReportProgress(
                        GPGSIds.achievement_for_the_north_pole,
                        100.0f, (bool success) =>
                        {
                           
                        });
            }
            if (levelat == 26)
            {
                PlayGamesPlatform.Instance.ReportProgress(
                        GPGSIds.achievement_and_penguin_kind,
                        100.0f, (bool success) =>
                        {
                           
                        });
            }
        }
        }
	
	public void SetLevels()
    {
        int levelat = PlayerData.instance.levelat;
       
        for (int i=0; i< levels.Length; i++)
        {
            if (i > levelat)
            {
                if (!dev)
                {
                    levels[i].interactable = false;
                }
            }
            else {
                levels[i].interactable = true;
                Levelsettings levelsettings = (Levelsettings)levels[i].gameObject.GetComponent(typeof(Levelsettings));
                levelsettings.Refresh();
                
            }
        }
    }
	
}

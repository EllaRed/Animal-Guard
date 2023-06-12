﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
using UnityEngine.Analytics.Experimental;
#endif
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

public class StartButton : MonoBehaviour
{
    public void StartGame()
    {
        if (PlayerData.instance.ftueLevel == 0)
        {
            PlayerData.instance.ftueLevel = 1;
            PlayerData.instance.Save();
#if UNITY_ANALYTICS
            UnityEngine.Analytics.AnalyticsEvent.FirstInteraction("start_button_pressed");
#endif
        }

#if UNITY_PURCHASING
        var module = StandardPurchasingModule.Instance();
#endif
        SceneManager.LoadScene("maincopy");
    }
}

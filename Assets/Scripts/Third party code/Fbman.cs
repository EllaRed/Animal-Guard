using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class Fbman : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (Application.platform==RuntimePlatform.IPhonePlayer)
        { Application.targetFrameRate = 60; }
        FB.Init(FBInitCallback);
    }

    private void FBInitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
    }


    public void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
        }
    }
}

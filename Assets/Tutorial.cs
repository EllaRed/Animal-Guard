using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {
    
    public GameObject[] tutstuff;
   
    public TrackManager trackManager;
    public int intid;
 
    public void Move1()
    {    
        gameObject.SetActive(true);

            for (int i = 0; i < tutstuff.Length; i++)
            {
                if (i == intid)
                {
                    tutstuff[i].SetActive(true);
                     trackManager.StopMove();

                }
                else tutstuff[i].SetActive(false);

            }
        

    }
    //public void Pause()
    //{
       
    //}

    public void Resume()
    {
        
        tutstuff[intid].SetActive(false);
        gameObject.SetActive(false);
        intid += 1;
        if (intid >= 4)
        {
            PlayerData.instance.tutorialcomplete = 1;
            PlayerData.instance.Save();
        }
        else { Move(); }
        trackManager.StartMove(false);
        
      
    }
    public void Move()
    {
        Invoke("Move1", 3);
    }
}

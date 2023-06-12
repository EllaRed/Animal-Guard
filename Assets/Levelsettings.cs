using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Levelsettings : MonoBehaviour {
    [Header("Settings")]
    public int levelid;
    public int leveltype;
    public int usedtheme;
    public int levelscore;
    public int minscore;
    public string lvlwinreq;
    public Modifier modifier;
    public LoadoutState Loadout;
    public int Enemymod;
    public float distmod;
    public int scoremod;
    public string rewardtext;
    public int rewardtype;
    public int rewardmoney;
    public string rewardlocation;
    public int rewardmultiplier;
    public int rewardgem;
    [Header("UI")]
   
    public Button Gobtn;
    public Text lvlscore, lvlnumber,lvlreward;
    public Text Lvlwinreqs;
    [Header("Stars")]
    public GameObject[] stars;
   

    public void Start()
    {
        
      //  Refresh();
    }

        public void Refresh()
    {
        Lvlwinreqs.text = lvlwinreq;
        lvlnumber.text = (levelid + 1).ToString();
        lvlreward.text = rewardtext;
        stars[ 3].SetActive(false);
        stars[4].SetActive(false);
        stars[5].SetActive(false);
        int j = 0;
            levelscore = 0;
        
        
        int f = minscore;
        int g = minscore * 2;
        if (levelid <= PlayerData.instance.levelat)
        {
            levelscore = PlayerData.instance.levelDatas[levelid].score;
            lvlscore.text = levelscore.ToString();

            for (int i = minscore; i < g; i++)
            {

                if (levelscore >= f)
                {
                    stars[j].SetActive(true);
                    stars[j + 3].SetActive(true);

                }
                f += minscore / 2;
                i += minscore / 2;
                i -= 2;
                j++;
            }
        }
        
        }
    
    public void Modify()
    {
        PlayerData.instance.usedTheme = usedtheme;

        if (leveltype == 0)
        {
            modifier = new LimitedLengthRun(distmod);
            
            Loadout.SetModifier(modifier,this);

        }
        if (leveltype == 1)
        {
            modifier = new ScoreRun(scoremod);

            Loadout.SetModifier(modifier, this);

        }
        if (leveltype == 2)
        {
            modifier = new SingleLifeRun(distmod);

            Loadout.SetModifier(modifier, this);

        }
        if (leveltype == 3)
        {
            modifier = new HitEnemiesRun(Enemymod);

            Loadout.SetModifier(modifier, this);

        }
        if (leveltype == 4)
        {
            modifier = new LimitedHitEnemiesRun(distmod, Enemymod);

            Loadout.SetModifier(modifier, this);

        }
        if (leveltype == 5)
        {
            modifier = new HighscoreRun();

            Loadout.SetModifier(modifier, this);

        }

        if (leveltype == 6)
        {
            modifier = new HitEnemiesOneLifeRun(distmod,Enemymod);

            Loadout.SetModifier(modifier, this);

        }

        if (leveltype == 7)
        {
            modifier = new LimitedMaxspeedRun(distmod);

            Loadout.SetModifier(modifier, this);

        }
    }


}

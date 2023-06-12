using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This class is used to modify the game state (e.g. limit length run, seed etc.)
/// Subclass it and override wanted messages to handle the state.
/// </summary>
public class Modifier 
{
    public bool win = false;
	public virtual void OnRunStart(GameState state)
	{
        
	}

	public virtual void OnRunTick(GameState state)
	{

	}

	//return true if the gameobver screen should be displayed, returning false will return directly to loadout (useful for challenge)
	public virtual bool OnRunEnd(GameState state)
	{
        
        return true;
	}
}

// The following classes are all the samples modifiers.

public class LimitedLengthRun : Modifier
{
	public float distance;

	public LimitedLengthRun(float dist)
	{
		distance = dist;
	}

	public override void OnRunTick(GameState state)
	{
        state.Distanceslider.value = state.trackManager.worldDistance;
        if (state.trackManager.worldDistance >= distance && win==false)
		{
            win = true;
            //state.trackManager.characterController.currentLife = 0;
            state.trackManager.characterController.Playwinsound();

            state.distanceText.color = Color.yellow;

        }
	}

	public override void OnRunStart(GameState state)
	{
        state.Distanceslider.gameObject.SetActive(true);
        state.Distanceslider.maxValue = distance;
    }

	public override bool OnRunEnd(GameState state)
    {
        if (win)
        {
            state.manager.SwitchState("GameWin");
        }else if (state.trackManager.isRerun)
            state.manager.SwitchState("GameOver");
        else
            state.OpenGameOverPopup();
        //state.manager.SwitchState("GameOver");
        return false;
	}
}

public class SeededRun : Modifier
{
	int m_Seed;

    protected const int k_DaysInAWeek = 7;

	public SeededRun()
	{
        m_Seed = System.DateTime.Now.DayOfYear / k_DaysInAWeek;
	}

	public override void OnRunStart(GameState state)
	{
		state.trackManager.trackSeed = m_Seed;
	}

	public override bool OnRunEnd(GameState state)
	{
		state.QuitToLoadout();
		return false;
	}
}

public class SingleLifeRun : Modifier
{
    public float distance;

    public SingleLifeRun(float dist)
    {
        distance = dist;
    }
    public override void OnRunTick(GameState state)
	{
        state.Distanceslider.value = state.trackManager.worldDistance;
        if (state.trackManager.characterController.currentLife > 1)
			state.trackManager.characterController.currentLife = 1;
        if (state.trackManager.worldDistance >= distance && win==false)
        {
            win = true;
            state.trackManager.characterController.Playwinsound();
      
            state.distanceText.color = Color.yellow;

        }
    }


	public override void OnRunStart(GameState state)
	{
        state.Distanceslider.gameObject.SetActive(true);
        state.Distanceslider.maxValue = distance;
    }

	public override bool OnRunEnd(GameState state)
	{
        if (win)
        {
            state.manager.SwitchState("GameWin");
        }
        else if (state.trackManager.isRerun)
            state.manager.SwitchState("GameOver");
        else
            state.OpenGameOverPopup();
        //state.manager.SwitchState("GameOver");
        return false;
	}
}

//hit a certain number of enemies to win
public class HitEnemiesRun : Modifier
{
    public int number;

    public HitEnemiesRun(int num)
    {
        number = num;
    }

    public override void OnRunTick(GameState state)
    {
        
        if (state.trackManager.hitcount == number && win==false)
        {
            win = true;
            //state.trackManager.characterController.currentLife = 0;
            state.trackManager.characterController.Playwinsound();

            state.hitsText.color = Color.yellow;

        }
    }

    public override void OnRunStart(GameState state)
    {
        
    }

    public override bool OnRunEnd(GameState state)
    {
        if (win)
        {
            state.manager.SwitchState("GameWin");
        }
        else if(state.trackManager.isRerun)
            state.manager.SwitchState("GameOver");
        else
            state.OpenGameOverPopup();
        //state.manager.SwitchState("GameOver");
        return false;
    }
}


//hit a certain number of enemies within a certain distance to win
public class LimitedHitEnemiesRun : Modifier
{
    public float distance;
    public int number;
    public LimitedHitEnemiesRun(float dist, int num)
    {
        distance = dist;
        number = num;
    }

    public override void OnRunTick(GameState state)
    {
        state.Distanceslider.value= state.trackManager.worldDistance;
        if (state.trackManager.hitcount == number &&win==false)
        {
            win = true;
            state.trackManager.characterController.Playwinsound();

            state.hitsText.color = Color.yellow;
        }
        if (state.trackManager.worldDistance >= distance)
        {  
            state.trackManager.characterController.currentLife = 0;
            
        }
    }

    public override void OnRunStart(GameState state)
    {
        state.Distanceslider.gameObject.SetActive(true);
        state.Distanceslider.maxValue = distance;
    }

    public override bool OnRunEnd(GameState state)
    {
        if (win)
        {
            state.manager.SwitchState("GameWin");
        }
        else 
            state.manager.SwitchState("GameOver");
        return false;
    }
}


//beat your own highscore
public class HighscoreRun : Modifier
{
    

    public override void OnRunTick(GameState state)
    {
        if ((state.trackManager.score >= PlayerData.instance.highscores[0].score) && win==false )
        {
            win = true;
            //state.trackManager.characterController.currentLife = 0;
            state.trackManager.characterController.Playwinsound();

            state.scoreText.color = Color.yellow;

        }
    }

    public override void OnRunStart(GameState state)
    {

    }

    public override bool OnRunEnd(GameState state)
    {
        if (win)
        {
            state.manager.SwitchState("GameWin");
        }
        else if (state.trackManager.isRerun)
            state.manager.SwitchState("GameOver");
        else
            state.OpenGameOverPopup();
        //state.manager.SwitchState("GameOver");
        return false;
    }
}
//get a particular score
public class ScoreRun : Modifier
{
    public int score;
    public ScoreRun( int num)
    {
        
        score = num;
    }

    public override void OnRunTick(GameState state)
    {
        if ((state.trackManager.score >= score) && win == false)
        {
            win = true;
            //state.trackManager.characterController.currentLife = 0;
            state.trackManager.characterController.Playwinsound();

            state.scoreText.color = Color.yellow;

        }
    }

    public override void OnRunStart(GameState state)
    {

    }

    public override bool OnRunEnd(GameState state)
    {
        if (win)
        {
            state.manager.SwitchState("GameWin");
        }
        else if(state.trackManager.isRerun)
            state.manager.SwitchState("GameOver");
        else
            state.OpenGameOverPopup();
        //state.manager.SwitchState("GameOver");
        return false;
    }
}

// hit enemies single life run

public class HitEnemiesOneLifeRun : Modifier
{
    public float distance;
    public int number;
    public HitEnemiesOneLifeRun(float dist, int num)
    {
        distance = dist;
        number = num;
    }

    public override void OnRunTick(GameState state)
    {
        state.Distanceslider.value = state.trackManager.worldDistance;
        if (state.trackManager.hitcount == number && win == false)
        {
            win = true;
            state.trackManager.characterController.Playwinsound();

            state.hitsText.color = Color.yellow;
        }
        if (state.trackManager.worldDistance >= distance)
        {
            state.trackManager.characterController.currentLife = 0;

        }
    }

    public override void OnRunStart(GameState state)
    {
        if (state.trackManager.characterController.currentLife > 1)
            state.trackManager.characterController.currentLife = 1;
        state.Distanceslider.gameObject.SetActive(true);
        state.Distanceslider.maxValue = distance;
    }

    public override bool OnRunEnd(GameState state)
    {
        if (win)
        {
            state.manager.SwitchState("GameWin");
        }
        else
            state.manager.SwitchState("GameOver");
        return false;
    }
}

//get to maxspeed
public class LimitedMaxspeedRun : Modifier
{
    public float distance;
    
    public LimitedMaxspeedRun(float dist)
    {
        distance = dist;
        
    }

    public override void OnRunTick(GameState state)
    {
        state.Distanceslider.value = state.trackManager.worldDistance;
        if (state.trackManager.m_Speed== state.trackManager.maxSpeed && win == false)
        {
            win = true;
            state.trackManager.characterController.Playwinsound();

            state.distanceText.color = Color.yellow;
        }
        if (state.trackManager.worldDistance >= distance)
        {
            state.trackManager.characterController.currentLife = 0;

        }
    }

    public override void OnRunStart(GameState state)
    {
        state.Distanceslider.gameObject.SetActive(true);
        state.Distanceslider.maxValue = distance;
    }

    public override bool OnRunEnd(GameState state)
    {
        if (win)
        {
            state.manager.SwitchState("GameWin");
        }
        else
            state.manager.SwitchState("GameOver");
        return false;
    }
}
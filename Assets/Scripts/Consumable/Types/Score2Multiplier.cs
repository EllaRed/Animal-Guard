﻿using UnityEngine;
using System;

public class Score2Multiplier : Consumable
{
    public override string GetConsumableName()
    {
        return "x2";
    }

    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.SCORE_MULTIPLAYER;
    }

    public override int GetPrice()
    {
        return 750;
    }

	public override int GetPremiumCost()
	{
		return 0;
	}

	public override void Started(CharacterInputController c)
    {
        base.Started(c);

        m_SinceStart = 0;

        c.trackManager.modifyMultiply += MultiplyModify;
    }

    public override void Ended(CharacterInputController c)
    {
        base.Ended(c);

        c.trackManager.modifyMultiply -= MultiplyModify;
    }

    protected int MultiplyModify(int multi)
    {
        int plier=2;
        if (PlayerData.instance.pudm[identifier] > 0)
        {
            plier += (int)PlayerData.instance.pudm[identifier]; 
        }
        return multi * plier;
    }
}

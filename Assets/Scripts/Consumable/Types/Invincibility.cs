﻿using UnityEngine;
using System;

public class Invincibility : Consumable
{
    public override string GetConsumableName()
    {
        return "Invincible";
    }

    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.INVINCIBILITY;
    }

    public override int GetPrice()
    {
        return 2000;
    }

	public override int GetPremiumCost()
	{
		return 0;
	}

	public override void Tick(CharacterInputController c)
    {
        base.Tick(c);

        c.characterCollider.SetInvincibleExplicit(true);
    }

    public override void Started(CharacterInputController c)
    {
        base.Started(c);
        c.characterCollider.SetInvincible(duration+PlayerData.instance.pudm[identifier]);
    }

    public override void Ended(CharacterInputController c)
    {
        base.Ended(c);
        c.characterCollider.SetInvincibleExplicit(false);
    }
}

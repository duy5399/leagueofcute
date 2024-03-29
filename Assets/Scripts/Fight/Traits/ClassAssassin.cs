using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClassAssassin : ChTraits
{
    public override void Launch(int breakpoint, List<GameObject> championsGetBuff)
    {
        base.Launch(breakpoint, championsGetBuff);

    }

    public override void OnLaunch(ChampionInfo1 chInfo, int breakpoint)
    {
        OnRemove(chInfo);
        if (chInfo.currentState != null && chInfo.skills != null)
        {
            Assassin assassin = (Assassin)_trait;
            chInfo.currentState._buffOnCriticalStrikeChance.Add(new StateBuff(assassin, StateBuff.TypeBuff.Mult, 1f + assassin.criticalChance[breakpoint]));
            chInfo.currentState._buffOnCriticalStrikeDamage.Add(new StateBuff(assassin, StateBuff.TypeBuff.Mult, 1f + assassin.criticalDamage[breakpoint]));
            chInfo.skills.specialAbility.details.canCrit = true;
        }
    }

    public override void OnRemove(ChampionInfo1 chInfo)
    {
        if (chInfo.currentState != null && chInfo.skills != null)
        {
            Assassin assassin = (Assassin)_trait;
            chInfo.currentState._buffOnCriticalStrikeChance.RemoveAll(x => x.classBuff == assassin);
            chInfo.currentState._buffOnCriticalStrikeDamage.RemoveAll(x => x.classBuff == assassin);
            chInfo.skills.specialAbility.details.canCrit = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassSorcerer : ChTraits
{
    public override void Launch(int breakpoint, List<GameObject> championsGetBuff)
    {
        base.Launch(breakpoint, championsGetBuff);

    }

    public override void OnLaunch(ChampionInfo1 chInfo, int breakpoint)
    {
        OnRemove(chInfo);
        if (chInfo.currentState != null)
        {
            Sorcerer sorcerer = (Sorcerer)_trait;
            chInfo.currentState._buffOnAbilityPower.Add(new StateBuff(sorcerer, StateBuff.TypeBuff.Add, sorcerer.abilityPower[breakpoint]));
        }    
    }

    public override void OnRemove(ChampionInfo1 chInfo)
    {
        if (chInfo.currentState != null)
        {
            Sorcerer sorcerer = (Sorcerer)_trait;
            chInfo.currentState._buffOnAbilityPower.RemoveAll(x => x.classBuff == sorcerer);
        }
    }
}

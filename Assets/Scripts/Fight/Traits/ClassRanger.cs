using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClassRanger : ChTraits
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
            Ranger ranger = (Ranger)_trait;
            if (chInfo.chStat.idClass == ClassBase.IdClass.Ranger)
            {
                chInfo.currentState._buffOnAttackSpeed.Add(new StateBuff(ranger, StateBuff.TypeBuff.Add, ranger.attackSpeedForRanger[breakpoint]));
            }
            else
            {
                chInfo.currentState._buffOnAttackSpeed.Add(new StateBuff(ranger, StateBuff.TypeBuff.Add, ranger.attackSpeedForAllies[breakpoint]));
            }
        }
    }

    public override void OnRemove(ChampionInfo1 chInfo)
    {
        if (chInfo.currentState != null)
        {
            Ranger ranger = (Ranger)_trait;
            chInfo.currentState._buffOnAttackSpeed.RemoveAll(x => x.classBuff == ranger);
        }
    }
}

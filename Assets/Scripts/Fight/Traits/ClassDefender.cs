using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassDefender : ChTraits
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
            Defender defender = (Defender)_trait;
            if (chInfo.chStat.idClass == ClassBase.IdClass.Defender)
            {
                chInfo.currentState._buffOnArmor.Add(new StateBuff(defender, StateBuff.TypeBuff.Add, defender.armorForDefender[breakpoint]));
            }
            else
            {
                chInfo.currentState._buffOnArmor.Add(new StateBuff(defender, StateBuff.TypeBuff.Add, defender.armorForAllies[breakpoint]));
            }
        }
    }

    public override void OnRemove(ChampionInfo1 chInfo)
    {
        if (chInfo.currentState != null)
        {
            Defender defender = (Defender)_trait;
            chInfo.currentState._buffOnArmor.RemoveAll(x => x.classBuff == defender);
        }
    }
}
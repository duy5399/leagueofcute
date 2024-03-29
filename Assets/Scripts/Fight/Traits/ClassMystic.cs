using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassMystic : ChTraits
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
            Mystic mystic = (Mystic)_trait;
            if (chInfo.chStat.idClass == ClassBase.IdClass.Mystic)
            {
                chInfo.currentState._buffOnMagicResistance.Add(new StateBuff(mystic, StateBuff.TypeBuff.Add, mystic.magicResistForMystic[breakpoint]));
            }
            else
            {
                chInfo.currentState._buffOnMagicResistance.Add(new StateBuff(mystic, StateBuff.TypeBuff.Add, mystic.magicResistForAllies[breakpoint]));
            }
        }
    }

    public override void OnRemove(ChampionInfo1 chInfo)
    {
        if (chInfo.currentState != null && chInfo.skills != null)
        {
            Mystic mystic = (Mystic)_trait;
            chInfo.currentState._buffOnMagicResistance.RemoveAll(x => x.classBuff == mystic);
        }
    }
}
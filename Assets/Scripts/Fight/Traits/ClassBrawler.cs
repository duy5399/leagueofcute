using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassBrawler : ChTraits
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
            Brawler brawler = (Brawler)_trait;
            if (chInfo.chStat.idClass == ClassBase.IdClass.Brawler)
            {
                chInfo.currentState._buffOnMaxHP.Add(new StateBuff(brawler, StateBuff.TypeBuff.Add, brawler.hpForBrawler[breakpoint]));
                chInfo.currentState.hp = chInfo.currentState.maxHP;
            }
            else
            {
                chInfo.currentState._buffOnMaxHP.Add(new StateBuff(brawler, StateBuff.TypeBuff.Add, brawler.hpForAllies[breakpoint]));
                chInfo.currentState.hp = chInfo.currentState.maxHP;
            }
        }
    }

    public override void OnRemove(ChampionInfo1 chInfo)
    {
        if (chInfo.currentState != null)
        {
            Brawler brawler = (Brawler)_trait;
            chInfo.currentState._buffOnMaxHP.RemoveAll(x => x.classBuff == brawler);
            chInfo.currentState.hp = chInfo.currentState.maxHP;
        }
    }
}
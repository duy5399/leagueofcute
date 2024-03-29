using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OriginDuelist : ChTraits
{
    public override void Launch(int breakpoint, List<GameObject> championsGetBuff)
    {
        base.Launch(breakpoint, championsGetBuff);

    }

    public override void OnLaunch(ChampionInfo1 chInfo, int breakpoint)
    {
        OnRemove(chInfo);
    }

    public override void OnRemove(ChampionInfo1 chInfo)
    {
        if (chInfo.currentState != null)
        {
            Duelist duelist = (Duelist)_trait;
            chInfo.currentState._buffOnAttackSpeed.RemoveAll(x => x.originBuff == duelist);
        }
    }

    protected override void FixedUpdate()
    {

    }

    public override void OnTriggerBuff()
    {
        Duelist duelist = (Duelist)_trait;
        foreach (var i in championsGetBuff)
        {
            ChampionInfo1 chInfo = i.GetComponent<ChampionInfo1>();
            if (chInfo != null && chInfo.stateCtrl.inCombat && !chInfo.currentState.dead)
            {
                chInfo.currentState._buffOnAttackSpeed.Add(new StateBuff(duelist, StateBuff.TypeBuff.Add, duelist.attackSpeed[breakpoint]));
            }
        }
    }
}
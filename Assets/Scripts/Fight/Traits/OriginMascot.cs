using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginMascot : ChTraits
{
    public override void Launch(int breakpoint, List<GameObject> championsGetBuff)
    {
        base.Launch(breakpoint, championsGetBuff);

    }

    public override void OnLaunch(ChampionInfo1 chInfo, int breakpoint)
    {

    }

    public override void OnRemove(ChampionInfo1 chInfo)
    {
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnTriggerBuff()
    {
        Mascot mascot = (Mascot)_trait;
        foreach (var i in championsGetBuff)
        {
            ChampionInfo1 chInfo = i.GetComponent<ChampionInfo1>();
            if (chInfo != null && chInfo.stateCtrl.inCombat && !chInfo.currentState.dead)
            {
                float heal = 0;
                if (chInfo.chStat.idOrigin == OriginBase.IdOrigin.Mascot)
                {
                    heal = chInfo.currentState.maxHP * mascot.hpRegenForMascot[breakpoint];
                }
                else
                {
                    heal = chInfo.currentState.maxHP * mascot.hpRegenForAllies[breakpoint];
                }
                chInfo.stateCtrl.TriggerHeal(heal);
            }
        }
    }
}

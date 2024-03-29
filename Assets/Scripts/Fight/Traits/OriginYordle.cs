using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginYordle : ChTraits
{
    public override void Launch(int breakpoint, List<GameObject> championsGetBuff)
    {
        base.Launch(breakpoint, championsGetBuff);

    }

    public override void OnLaunch(ChampionInfo1 chInfo, int breakpoint)
    {
        OnRemove(chInfo);
        if (chInfo.chStat.idOrigin== OriginBase.IdOrigin.Yordle)
        {
            Yordle yordle = (Yordle)_trait;
            chInfo.currentState._buffOnMaxMana.Add(new StateBuff(yordle, StateBuff.TypeBuff.Mult, 1 - yordle.decreaseManaRequired[breakpoint]));
        }
    }

    public override void OnRemove(ChampionInfo1 chInfo)
    {
        if (chInfo.currentState != null)
        {
            Yordle yordle = (Yordle)_trait;
            chInfo.currentState._buffOnMaxMana.RemoveAll(x => x.originBuff == yordle);
        }
    }

    protected override void FixedUpdate()
    {

    }

    public override void OnTriggerBuff()
    {
        Yordle yordle = (Yordle)_trait;
        foreach (var i in championsGetBuff)
        {
            ChampionInfo1 chInfo = i.GetComponent<ChampionInfo1>();
            if (chInfo != null && chInfo.stateCtrl.inCombat && chInfo.currentState.dead)
            {
                foreach (var j in championsGetBuff)
                {
                    ChampionInfo1 chInfo1 = j.GetComponent<ChampionInfo1>();
                    if (chInfo1 != null && !chInfo1.currentState.dead && chInfo1.chStat.idOrigin == OriginBase.IdOrigin.Yordle)
                    {
                        float mana = yordle.restoreMana[breakpoint];
                        chInfo1.stateCtrl.TriggerManaDelta(mana);
                    }
                }
                championsGetBuff.Remove(i);
            }
        }
    }
}
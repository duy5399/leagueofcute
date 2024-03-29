using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginHextech : ChTraits
{
    public override void Launch(int breakpoint, List<GameObject> championsGetBuff)
    {
        base.Launch(breakpoint, championsGetBuff);
        Debug.Log("OriginHextech Launch");
    }

    public override void OnLaunch(ChampionInfo1 chInfo, int breakpoint)
    {
        base.OnLaunch(chInfo, breakpoint);
    }

    public override void OnRemove(ChampionInfo1 chInfo)
    {
        base.OnRemove(chInfo);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Debug.Log("FixedUpdate OriginHextech");
    }

    public override void OnTriggerBuff()
    {
        Debug.Log("OnTriggerBuff OriginHextech");
        Hextech hextech = (Hextech)_trait;
        foreach (var i in championsGetBuff)
        {
            ChampionInfo1 chInfo = i.GetComponent<ChampionInfo1>();
            if (chInfo != null && chInfo.stateCtrl.inCombat && !chInfo.currentState.dead)
            {
                float mana = hextech.manaRegen[breakpoint];
                chInfo.stateCtrl.TriggerManaDelta(mana);
            }
        }
    }
}
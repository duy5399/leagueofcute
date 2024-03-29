using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SkillBase1;

public class OriginDawnbringer : ChTraits
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
            Dawnbringer dawnbringer = (Dawnbringer)_trait;
            chInfo.currentState._buffOnPhysicalVamp.RemoveAll(x => x.originBuff == dawnbringer);
            chInfo.currentState._buffOnSpellVamp.RemoveAll(x => x.originBuff == dawnbringer);
        }
    }

    protected override void FixedUpdate()
    {

    }

    public override void OnTriggerBuff()
    {
        Dawnbringer dawnbringer = (Dawnbringer)_trait;
        foreach (var i in championsGetBuff)
        {
            ChampionInfo1 chInfo = i.GetComponent<ChampionInfo1>();
            if (chInfo != null && chInfo.stateCtrl.inCombat && !chInfo.currentState.dead && (chInfo.currentState.hp / chInfo.currentState.maxHP) < 0.5f)
            {
                if (chInfo.currentState._buffOnPhysicalVamp.FirstOrDefault(x => x.originBuff == dawnbringer) != null && chInfo.currentState._buffOnSpellVamp.FirstOrDefault(x => x.originBuff == dawnbringer) != null)
                {
                    float healing = chInfo.currentState.maxHP * dawnbringer.healing[breakpoint];
                    chInfo.stateCtrl.TriggerHeal(healing);
                    chInfo.currentState._buffOnPhysicalVamp.Add(new StateBuff(dawnbringer, StateBuff.TypeBuff.Add, dawnbringer.physicalVamp[breakpoint]));
                    chInfo.currentState._buffOnSpellVamp.Add(new StateBuff(dawnbringer, StateBuff.TypeBuff.Add, dawnbringer.spellVamp[breakpoint]));
                }
            }
        }
    }
}
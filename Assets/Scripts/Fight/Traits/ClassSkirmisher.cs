using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassSkirmisher : ChTraits
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
            Skirmisher skirmisher = (Skirmisher)_trait;
            float shield = skirmisher.shieldAmount[breakpoint] * chInfo.currentState.maxHP;
            chInfo.currentState._buffOnShield.Add(new StateBuff(skirmisher, StateBuff.TypeBuff.Add, shield));
        }
    }

    public override void OnRemove(ChampionInfo1 chInfo)
    {
        if (chInfo.currentState != null)
        {
            Skirmisher skirmisher = (Skirmisher)_trait;
            chInfo.currentState._buffOnShield.RemoveAll(x => x.classBuff == skirmisher);
            chInfo.currentState._buffOnAttackDamage.RemoveAll(x => x.classBuff == skirmisher);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnTriggerBuff()
    {
        Skirmisher skirmisher = (Skirmisher)_trait;
        foreach (var i in championsGetBuff)
        {
            ChampionInfo1 chInfo = i.GetComponent<ChampionInfo1>();
            if (chInfo != null && chInfo.stateCtrl.inCombat && !chInfo.currentState.dead)
            {
                chInfo.currentState._buffOnAttackDamage.Add(new StateBuff(skirmisher, StateBuff.TypeBuff.Add, skirmisher.attackDamagePerSecond[breakpoint]));
            }
        }
    }
}

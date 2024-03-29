using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OriginNightbringer : ChTraits
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
            Nightbringer nightbringer = (Nightbringer)_trait;
            chInfo.currentState._buffOnShield.RemoveAll(x => x.originBuff == nightbringer);
            chInfo.currentState._buffOnArmorPenetrationPercentage.RemoveAll(x => x.originBuff == nightbringer);
            chInfo.currentState._buffOnMagicPenetrationPercentage.RemoveAll(x => x.originBuff == nightbringer);
        }
    }

    protected override void FixedUpdate()
    {
        if (!isActive)
        {
            return;
        }
        if (_trait.isActiveInCombat)
        {
            OnTriggerBuff();
            Debug.Log("OnTriggerBuff();");
        }
    }

    public override void OnTriggerBuff()
    {
        Nightbringer nightbringer = (Nightbringer)_trait;
        foreach (var i in championsGetBuff)
        {
            ChampionInfo1 chInfo = i.GetComponent<ChampionInfo1>();
            if (chInfo != null && chInfo.stateCtrl.inCombat && !chInfo.currentState.dead && (chInfo.currentState.hp / chInfo.currentState.maxHP) < 0.5f)
            {
                Debug.Log("OnTriggerBuff(): " + chInfo.name);
                if (chInfo.currentState._buffOnShield.FirstOrDefault(x => x.originBuff == nightbringer) == null && chInfo.currentState._buffOnArmorPenetrationPercentage.FirstOrDefault(x => x.originBuff == nightbringer) == null && chInfo.currentState._buffOnMagicPenetrationPercentage.FirstOrDefault(x => x.originBuff == nightbringer) == null)
                {
                    float shield = chInfo.currentState.maxHP * nightbringer.shieldAmout[breakpoint];
                    chInfo.currentState._buffOnShield.Add(new StateBuff(nightbringer, StateBuff.TypeBuff.Add, shield));
                    chInfo.currentState._buffOnArmorPenetrationPercentage.Add(new StateBuff(nightbringer, StateBuff.TypeBuff.Add, nightbringer.armorPen[breakpoint]));
                    chInfo.currentState._buffOnMagicPenetrationPercentage.Add(new StateBuff(nightbringer, StateBuff.TypeBuff.Add, nightbringer.magicResistPen[breakpoint]));
                    //WaitFor()
                    Debug.Log("OnTriggerBuff() success: " + chInfo.name);
                }
            }
        }
    }
}
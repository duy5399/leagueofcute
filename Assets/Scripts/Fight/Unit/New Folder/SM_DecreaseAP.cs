using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_DecreaseAP : SM_Buff
{
    [SerializeField] private float abilityPowerAdd;
    [SerializeField] private float abilityPowerMult;

    public override void OnLaunch()
    {
        base.currentState._buffOnAbilityPower.Add(new StateBuff(this, StateBuff.TypeBuff.Add, -abilityPowerAdd));
        base.currentState._buffOnAbilityPower.Add(new StateBuff(this, StateBuff.TypeBuff.Mult, 1f - abilityPowerMult));
    }

    public override void WhenNotActive()
    {
        if (base.info.currentState)
        {
            base.info.currentState._buffOnAbilityPower.RemoveAll(x => x.buff == this);
        }
    }
}
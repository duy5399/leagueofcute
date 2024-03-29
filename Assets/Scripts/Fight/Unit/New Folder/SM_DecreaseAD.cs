using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_DecreaseAD : SM_Buff
{
    [SerializeField] private float attackDamageAdd;
    [SerializeField] private float attackDamageMult;

    public override void OnLaunch()
    {
        base.currentState._buffOnAttackDamage.Add(new StateBuff(this, StateBuff.TypeBuff.Add, -attackDamageAdd));
        base.currentState._buffOnAttackDamage.Add(new StateBuff(this, StateBuff.TypeBuff.Mult, 1f - attackDamageMult));
    }

    public override void WhenNotActive()
    {
        if(base.info.currentState)
        {
            base.info.currentState._buffOnAttackDamage.RemoveAll(x => x.buff == this);
        }
    }
}

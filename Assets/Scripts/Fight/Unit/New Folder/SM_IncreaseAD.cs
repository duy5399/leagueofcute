using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_IncreaseAD : SM_Buff
{
    [SerializeField] private float attackDamageAdd;
    [SerializeField] private float attackDamageMult;

    public override void _Init()
    {
        buffID = skill.details.increaseAD.buffID;
        haveLifeTime = skill.details.increaseAD.haveLifeTime;
        lifeTime = (skill.details.increaseAD.lifeTimeCanChange == true) ? skill.details.increaseAD.lifeTime[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseAD.lifeTime[0];
        lifeTimeLeft = lifeTime;
        destroyOnLifeEnding = skill.details.increaseAD.destroyOnLifeEnding;
        addType = skill.details.increaseAD.addType;
        maxStackUp = skill.details.increaseAD.maxStackUp;
        attackDamageAdd = skill.details.increaseAD.attackDamageAddCanChange == true ? skill.details.increaseAD.attackDamageAdd[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseAD.attackDamageAdd[0];
        attackDamageMult = skill.details.increaseAD.attackDamageMultCanChange == true ? skill.details.increaseAD.attackDamageMult[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseAD.attackDamageMult[0];
    }

    public override void OnLaunch()
    {
        base.currentState._buffOnAttackDamage.Add(new StateBuff(this, StateBuff.TypeBuff.Add, attackDamageAdd));
        base.currentState._buffOnAttackDamage.Add(new StateBuff(this, StateBuff.TypeBuff.Mult, 1f + attackDamageMult));
    }

    public override void WhenNotActive()
    {
        if (base.info.currentState)
        {
            base.info.currentState._buffOnAttackDamage.RemoveAll(x => x.buff == this);
        }
    }
}


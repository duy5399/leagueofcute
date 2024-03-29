using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_DecreaseMR : SM_Buff
{

    [SerializeField] private float magicResistanceAdd;
    [SerializeField] private float magicResistanceMult;

    public override void _Init()
    {
        buffID = skill.details.decreaseMR.buffID;
        haveLifeTime = skill.details.decreaseMR.haveLifeTime;
        lifeTime = (skill.details.decreaseMR.lifeTimeCanChange == true) ? skill.details.decreaseMR.lifeTime[skill.info.chStat.currentLevel.star - 1] : skill.details.decreaseMR.lifeTime[0];
        lifeTimeLeft = lifeTime;
        destroyOnLifeEnding = skill.details.decreaseMR.destroyOnLifeEnding;
        addType = skill.details.decreaseMR.addType;
        maxStackUp = skill.details.decreaseMR.maxStackUp;
        magicResistanceAdd = skill.details.decreaseMR.magicResistanceAddCanChange == true ? skill.details.decreaseMR.magicResistanceAdd[skill.info.chStat.currentLevel.star - 1] : skill.details.decreaseMR.magicResistanceAdd[0];
        magicResistanceMult = skill.details.decreaseMR.magicResistanceMultCanChange == true ? skill.details.decreaseMR.magicResistanceMult[skill.info.chStat.currentLevel.star - 1] : skill.details.decreaseMR.magicResistanceMult[0];
    }

    public override void OnLaunch()
    {
        base.currentState._buffOnMagicResistance.Add(new StateBuff(this, StateBuff.TypeBuff.Add, -magicResistanceAdd));
        base.currentState._buffOnMagicResistance.Add(new StateBuff(this, StateBuff.TypeBuff.Mult, 1f - magicResistanceMult));
    }

    public override void WhenNotActive()
    {
        if (base.info.currentState)
        {
            base.info.currentState._buffOnMagicResistance.RemoveAll(x => x.buff == this);
        }
    }
}

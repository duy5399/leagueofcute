using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillSpawn1;

public class SM_IncreaseMR : SM_Buff
{

    [SerializeField] private float magicResistanceAdd;
    [SerializeField] private float magicResistanceMult;

    public override void _Init()
    {
        buffID = skill.details.increaseMR.buffID;
        haveLifeTime = skill.details.increaseMR.haveLifeTime;
        lifeTime = (skill.details.increaseMR.lifeTimeCanChange == true) ? skill.details.increaseMR.lifeTime[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseMR.lifeTime[0];
        lifeTimeLeft = lifeTime;
        destroyOnLifeEnding = skill.details.increaseMR.destroyOnLifeEnding;
        addType = skill.details.increaseMR.addType;
        maxStackUp = skill.details.increaseMR.maxStackUp;
        magicResistanceAdd = skill.details.increaseMR.magicResistanceAddCanChange == true ? skill.details.increaseMR.magicResistanceAdd[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseMR.magicResistanceAdd[0];
        magicResistanceMult = skill.details.increaseMR.magicResistanceMultCanChange == true ? skill.details.increaseMR.magicResistanceMult[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseMR.magicResistanceMult[0];
    }

    public override void OnLaunch()
    {
        base.currentState._buffOnMagicResistance.Add(new StateBuff(this, StateBuff.TypeBuff.Add, magicResistanceAdd));
        base.currentState._buffOnMagicResistance.Add(new StateBuff(this, StateBuff.TypeBuff.Mult, 1f + magicResistanceMult));
    }

    public override void WhenNotActive()
    {
        if (base.info.currentState)
        {
            base.info.currentState._buffOnMagicResistance.RemoveAll(x => x.buff == this);
        }
    }
}

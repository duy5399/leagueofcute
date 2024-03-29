using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_IncreaseAR : SM_Buff
{
    [SerializeField] private float armorAdd;
    [SerializeField] private float armorMult;

    public override void _Init()
    {
        buffID = skill.details.increaseAR.buffID;
        haveLifeTime = skill.details.increaseAR.haveLifeTime;
        lifeTime = (skill.details.increaseAR.lifeTimeCanChange == true) ? skill.details.increaseAR.lifeTime[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseAR.lifeTime[0];
        lifeTimeLeft = lifeTime;
        destroyOnLifeEnding = skill.details.increaseAR.destroyOnLifeEnding;
        addType = skill.details.increaseAR.addType;
        maxStackUp = skill.details.increaseAR.maxStackUp;
        armorAdd = skill.details.increaseAR.armorAddCanChange == true ? skill.details.increaseAR.armorAdd[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseAR.armorAdd[0];
        armorMult = skill.details.increaseAR.armorMultCanChange == true ? skill.details.increaseAR.armorMult[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseAR.armorMult[0];
    }

    public override void OnLaunch()
    {
        base.currentState._buffOnArmor.Add(new StateBuff(this, StateBuff.TypeBuff.Add, armorAdd));
        base.currentState._buffOnArmor.Add(new StateBuff(this, StateBuff.TypeBuff.Mult, 1f + armorMult));
    }

    public override void WhenNotActive()
    {
        if (base.info.currentState)
        {
            base.info.currentState._buffOnArmor.RemoveAll(x => x.buff == this);
        }
    }
}

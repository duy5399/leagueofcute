using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_IncreaseAS : SM_Buff
{
    [SerializeField] private float attackSpeedMult;

    public override void _Init()
    {
        buffID = skill.details.increaseAS.buffID;
        haveLifeTime = skill.details.increaseAS.haveLifeTime;
        lifeTime = (skill.details.increaseAS.lifeTimeCanChange == true) ? skill.details.increaseAS.lifeTime[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseAS.lifeTime[0];
        lifeTimeLeft = lifeTime;
        destroyOnLifeEnding = skill.details.increaseAS.destroyOnLifeEnding;
        addType = skill.details.increaseAS.addType;
        maxStackUp = skill.details.increaseAS.maxStackUp;
        attackSpeedMult = skill.details.increaseAS.attackSpeedMultCanChange == true ? skill.details.increaseAS.attackSpeedMult[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseAS.attackSpeedMult[0];
    }

    public override void OnLaunch()
    {
        base.currentState._buffOnAttackSpeed.Add(new StateBuff(this, StateBuff.TypeBuff.Mult, 1f + attackSpeedMult));
    }

    public override void WhenNotActive()
    {
        if (base.info.currentState)
        {
            base.info.currentState._buffOnAttackSpeed.RemoveAll(x => x.buff == this);
        }
    }
}
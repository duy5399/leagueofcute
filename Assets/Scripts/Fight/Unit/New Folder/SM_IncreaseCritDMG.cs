using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_IncreaseCritDMG : SM_Buff
{
    [SerializeField] private float criticalDamageAdd;

    public override void _Init()
    {
        buffID = skill.details.increaseCritDMG.buffID;
        haveLifeTime = skill.details.increaseCritDMG.haveLifeTime;
        lifeTime = (skill.details.increaseCritDMG.lifeTimeCanChange == true) ? skill.details.increaseCritDMG.lifeTime[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseCritDMG.lifeTime[0];
        lifeTimeLeft = lifeTime;
        destroyOnLifeEnding = skill.details.increaseCritDMG.destroyOnLifeEnding;
        addType = skill.details.increaseCritDMG.addType;
        maxStackUp = skill.details.increaseCritDMG.maxStackUp;
        criticalDamageAdd = skill.details.increaseCritDMG.criticalDamageAddCanChange == true ? skill.details.increaseCritDMG.criticalDamageAdd[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseCritDMG.criticalDamageAdd[0];
    }

    public override void OnLaunch()
    {
        base.currentState._buffOnCriticalStrikeDamage.Add(new StateBuff(this, StateBuff.TypeBuff.Add, criticalDamageAdd));
    }

    public override void WhenNotActive()
    {
        if (base.info.currentState)
        {
            base.info.currentState._buffOnCriticalStrikeDamage.RemoveAll(x => x.buff == this);
        }
    }
}


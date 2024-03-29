using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_IncreaseCritChance : SM_Buff
{
    [SerializeField] private float criticalChanceAdd;

    public override void _Init()
    {
        buffID = skill.details.increaseCritChance.buffID;
        haveLifeTime = skill.details.increaseCritChance.haveLifeTime;
        lifeTime = (skill.details.increaseCritChance.lifeTimeCanChange == true) ? skill.details.increaseCritChance.lifeTime[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseCritChance.lifeTime[0];
        lifeTimeLeft = lifeTime;
        destroyOnLifeEnding = skill.details.increaseCritChance.destroyOnLifeEnding;
        addType = skill.details.increaseCritChance.addType;
        maxStackUp = skill.details.increaseCritChance.maxStackUp;
        criticalChanceAdd = skill.details.increaseCritChance.criticalChanceAddCanChange == true ? skill.details.increaseCritChance.criticalChanceAdd[skill.info.chStat.currentLevel.star - 1] : skill.details.increaseCritChance.criticalChanceAdd[0];
    }

    public override void OnLaunch()
    {
        base.currentState._buffOnCriticalStrikeChance.Add(new StateBuff(this, StateBuff.TypeBuff.Add, criticalChanceAdd));
    }

    public override void WhenNotActive()
    {
        if (base.info.currentState)
        {
            base.info.currentState._buffOnCriticalStrikeChance.RemoveAll(x => x.buff == this);
        }
    }
}

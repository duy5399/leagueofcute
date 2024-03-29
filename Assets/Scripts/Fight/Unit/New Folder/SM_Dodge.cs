using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_Dodge : SM_Buff
{

    [SerializeField] private float dodgeLifetime;

    public override void _Init()
    {
        buffID = skill.details.dodge.buffID;
        haveLifeTime = skill.details.dodge.haveLifeTime;
        lifeTime = (skill.details.dodge.lifeTimeCanChange == true) ? skill.details.dodge.lifeTime[skill.info.chStat.currentLevel.star - 1] : skill.details.dodge.lifeTime[0];
        lifeTimeLeft = lifeTime;
        destroyOnLifeEnding = skill.details.dodge.destroyOnLifeEnding;
        addType = skill.details.dodge.addType;
        maxStackUp = skill.details.dodge.maxStackUp;
        dodgeLifetime = skill.details.dodge.dodgeLifetimeCanChange == true ? skill.details.dodge.dodgeLifetime[skill.info.chStat.currentLevel.star - 1] : skill.details.dodge.dodgeLifetime[0];
    }

    public override void OnLaunch()
    {
        base.stateCtrl.dodgeTimeLeft = dodgeLifetime;
    }
}

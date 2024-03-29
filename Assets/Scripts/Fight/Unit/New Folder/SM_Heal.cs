using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_Heal : SM_Buff
{
    [SerializeField] private float healing;
    [SerializeField] private float extraMaxHpPercentage;
    [SerializeField] private float extraCurrentHpPercentage;
    [SerializeField] private float extraMissingHpPercentage;

    public override void _Init()
    {
        buffID = skill.details.heal.buffID;
        haveLifeTime = skill.details.heal.haveLifeTime;
        lifeTime = (skill.details.heal.lifeTimeCanChange == true) ? skill.details.heal.lifeTime[skill.info.chStat.currentLevel.star - 1] : skill.details.heal.lifeTime[0];
        lifeTimeLeft = lifeTime;
        destroyOnLifeEnding = skill.details.heal.destroyOnLifeEnding;
        addType = skill.details.heal.addType;
        maxStackUp = skill.details.heal.maxStackUp;
        healing = skill.details.heal.healingCanChange == true ? skill.details.heal.healing[skill.info.chStat.currentLevel.star - 1] : skill.details.heal.healing[0];
        extraMaxHpPercentage = skill.details.heal.extraMaxHpPercentageCanChange == true ? skill.details.heal.extraMaxHpPercentage[skill.info.chStat.currentLevel.star - 1] : skill.details.heal.extraMaxHpPercentage[0];
        extraCurrentHpPercentage = skill.details.heal.extraCurrentHpPercentageCanChange == true ? skill.details.heal.extraCurrentHpPercentage[skill.info.chStat.currentLevel.star - 1] : skill.details.heal.extraCurrentHpPercentage[0];
        extraMissingHpPercentage = skill.details.heal.extraMissingHpPercentageCanChange == true ? skill.details.heal.extraMissingHpPercentage[skill.info.chStat.currentLevel.star - 1] : skill.details.heal.extraMissingHpPercentage[0];
    }

    public float healValue
    {
        get
        {
            return (healing + (extraMaxHpPercentage * base.currentState.maxHP) + (extraCurrentHpPercentage * base.currentState.hp) + extraMissingHpPercentage * (base.currentState.maxHP - base.currentState.hp)) * currentCasterStatus.abilityPower/100;
        }
    }

    public override void OnLaunch()
    {
        if (base.info.stateCtrl != null)
        {
            base.info.stateCtrl.TriggerHeal(healValue);
        }
    }
}
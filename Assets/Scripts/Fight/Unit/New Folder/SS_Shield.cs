using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_Shield : SS_Buff
{
    protected override void Init()
    {
        buffOn = skill.details.shield.buffOn;
        buffRange = skill.details.shield.buffRange;
        maxHitNum = skill.details.shield.maxHitCanChange == true ? skill.details.shield.maxHit[skill.info.chStat.currentLevel.star - 1] : skill.details.shield.maxHit[0];
    }
}

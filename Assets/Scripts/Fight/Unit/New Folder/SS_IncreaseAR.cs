using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_IncreaseAR : SS_Buff
{
    protected override void Init()
    {
        buffOn = skill.details.increaseAR.buffOn;
        buffRange = skill.details.increaseAR.buffRange;
        maxHitNum = skill.details.increaseAR.maxHitNum;
    }
}

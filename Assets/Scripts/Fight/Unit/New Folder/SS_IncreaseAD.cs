using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_IncreaseAD : SS_Buff
{
    protected override void Init()
    {
        buffOn = skill.details.increaseAD.buffOn;
        buffRange = skill.details.increaseAD.buffRange;
        maxHitNum = skill.details.increaseAD.maxHitNum;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_DecreaseAR : SS_Buff
{
    protected override void Init()
    {
        buffOn = skill.details.decreaseAR.buffOn;
        buffRange = skill.details.decreaseAR.buffRange;
        maxHitNum = skill.details.decreaseAR.maxHitNum;
    }
}

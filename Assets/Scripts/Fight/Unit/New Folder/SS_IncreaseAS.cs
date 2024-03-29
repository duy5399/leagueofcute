using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_IncreaseAS : SS_Buff
{
    protected override void Init()
    {
        buffOn = skill.details.increaseAS.buffOn;
        buffRange = skill.details.increaseAS.buffRange;
        maxHitNum = skill.details.increaseAS.maxHitNum;
    }
}

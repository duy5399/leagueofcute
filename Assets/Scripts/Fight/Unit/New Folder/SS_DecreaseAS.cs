using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_DecreaseAS : SS_Buff
{
    protected override void Init()
    {
        buffOn = skill.details.decreaseAS.buffOn;
        buffRange = skill.details.decreaseAS.buffRange;
        maxHitNum = skill.details.decreaseAS.maxHitNum;
    }
}

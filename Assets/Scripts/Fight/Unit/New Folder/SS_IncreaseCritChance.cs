using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_IncreaseCritChance : SS_Buff
{
    protected override void Init()
    {
        buffOn = skill.details.increaseCritChance.buffOn;
        buffRange = skill.details.increaseCritChance.buffRange;
        maxHitNum = skill.details.increaseCritChance.maxHitNum;
    }
}

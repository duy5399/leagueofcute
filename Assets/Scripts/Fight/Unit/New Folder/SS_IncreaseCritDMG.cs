using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_IncreaseCritDMG : SS_Buff
{
    protected override void Init()
    {
        buffOn = skill.details.increaseCritDMG.buffOn;
        buffRange = skill.details.increaseCritDMG.buffRange;
        maxHitNum = skill.details.increaseCritDMG.maxHitNum;
    }
}

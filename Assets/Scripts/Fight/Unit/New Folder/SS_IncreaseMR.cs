using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_IncreaseMR : SS_Buff
{
    protected override void Init()
    {
        buffOn = skill.details.increaseMR.buffOn;
        buffRange = skill.details.increaseMR.buffRange;
        maxHitNum = skill.details.increaseMR.maxHitNum;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_DecreaseMR : SS_Buff
{
    protected override void Init()
    {
        buffOn = skill.details.decreaseMR.buffOn;
        buffRange = skill.details.decreaseMR.buffRange;
        maxHitNum = skill.details.decreaseMR.maxHitNum;
    }
}

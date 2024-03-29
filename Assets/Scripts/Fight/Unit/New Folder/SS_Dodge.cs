using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_Dodge : SS_Buff
{
    protected override void Init()
    {
        buffOn = skill.details.dodge.buffOn;
        buffRange = skill.details.dodge.buffRange;
        maxHitNum = skill.details.dodge.maxHitNum;
    }
}

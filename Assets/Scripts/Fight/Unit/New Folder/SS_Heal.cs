using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_Heal : SS_Buff
{
    protected override void Init()
    {
        buffOn = skill.details.heal.buffOn;
        buffRange = skill.details.heal.buffRange;
        maxHitNum = skill.details.heal.maxHitNum;
    }
}

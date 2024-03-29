using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_IncreasePhysicalVamp : SM_Buff
{
    [SerializeField] private float extraPhysicalVamp;

    public override void OnLaunch()
    {
        base.currentState._buffOnPhysicalVamp.Add(new StateBuff(this, StateBuff.TypeBuff.Add, extraPhysicalVamp));
    }

    public override void WhenNotActive()
    {
        if (base.info.currentState)
        {
            base.info.currentState._buffOnPhysicalVamp.RemoveAll(x => x.buff == this);
        }
    }
}


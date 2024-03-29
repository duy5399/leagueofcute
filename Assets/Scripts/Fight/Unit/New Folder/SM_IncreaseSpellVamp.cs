using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_IncreaseSpellVamp : SM_Buff
{
    [SerializeField] private float extraSpellVamp;

    public override void OnLaunch()
    {
        base.currentState._buffOnSpellVamp.Add(new StateBuff(this, StateBuff.TypeBuff.Add, extraSpellVamp));
    }

    public override void WhenNotActive()
    {
        if (base.info.currentState)
        {
            base.info.currentState._buffOnSpellVamp.RemoveAll(x => x.buff == this);
        }
    }
}
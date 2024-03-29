using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_Blind : SM_Buff
{
    public override void OnLaunch()
    {
        if ((bool)base.stateCtrl)
        {
            base.stateCtrl.blindTimeLeft = lifeTime;
        }
    }
}
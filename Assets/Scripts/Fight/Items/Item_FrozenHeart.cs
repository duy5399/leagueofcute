using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FrozenHeart : ItemBase
{
    protected override void FixedUpdate()
    {
        if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
        {
            return;
        }
        if(!isActive && base.info.skills.target != null)
        {
            _itemPassive.TriggerSpawn(base.info.skills.target.transform);
            isActive = true;
        }
    }

    public override void OnReset()
    {
        base.OnReset();
        isActive = false;
    }
}
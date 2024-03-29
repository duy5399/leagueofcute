using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Item_SunfireCape : ItemBase
{
    protected override void Awake()
    {
        base.Awake();
        cooldown = 2f;
    }
    protected override void FixedUpdate()
    {
        if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
        {
            return;
        }
        if (!base.info.skills.target)
        {
            return;
        }
        if (cooldown > 0)
        {
            cooldown -= Time.fixedDeltaTime;
        }
        else
        {
            _itemPassive.TriggerSpawn(base.info.skills.target.transform);
            cooldown = 2f;
        }
    }
    public override void OnReset()
    {
        base.OnReset();
        cooldown = 2f;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static SkillSpawn1;

public class Item_BrambleVest : ItemBase
{
    protected override void FixedUpdate()
    {
        if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
        {
            return;
        }
        //if (!isActive)
        //{
        //    cooldown -= Time.fixedDeltaTime;
        //    if(cooldown <= 0)
        //    {
        //        isActive = true;
        //        cooldown = 2f;
        //    }
        //}
        if (cooldown > 0)
        {
            cooldown -= Time.fixedDeltaTime;
        }
        else
        {
            isActive = false;
        }
    }

    public override void OnReset()
    {
        base.OnReset();
        isActive = false;
        cooldown = 0f;
    }

    public override void OnBeHited(Transform caster, float damage, bool isCritical)
    {
        if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
        {
            return;
        }
        if(!isActive)
        {
            Debug.Log("Item_BrambleVest OnBasicAttack: " + caster.name + " - " + base.info.name);
            _itemPassive.TriggerSpawn(caster);
            isActive = true;
            cooldown = 2f;
        }
    }
}
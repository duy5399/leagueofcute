using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillSpawn1;

public class Item_LastWhisper : ItemBase
{
    public override void OnHit(Transform target, float damage, bool isCritical)
    {
        if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
        {
            return;
        }
        Debug.Log("Item_LastWhisper OnHit");
        _itemPassive.TriggerSpawn(target);
    }
}
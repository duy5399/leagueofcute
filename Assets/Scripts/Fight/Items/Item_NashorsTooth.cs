using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item_NashorsTooth : ItemBase
{
    public override void OnSpecialAbility(SkillBase1 skillBase, Transform target)
    {
        if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
        {
            return;
        }
        Debug.Log("Item_NashorsTooth OnSpecialAbility");
        _itemPassive.TriggerSpawn(base.skills.target.transform);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_SteraksGage : ItemBase
{
    protected override void FixedUpdate()
    {
        if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
        {
            return;
        }
        if (!isActive)
        {
            if (!base.info.currentState.dead && base.info.currentState.hp / base.info.currentState.maxHP < 0.6)
            {
                //if (base.info.currentState._buffOnAttackDamage.Find(x => x.item == _item && x.amount == _item.passive.increaseAD.attackDamageMult[0]) == null)
                //{

                //}
                Debug.Log("Item_SteraksGage active: " + base.info.skills.target.name);
                base.info.currentState._buffOnAttackDamage.Add(new StateBuff(_item, StateBuff.TypeBuff.Mult, 1f + _item.passive.increaseAD.attackDamageMult[0]));
                _itemPassive.TriggerSpawn(base.info.skills.target.transform);
                isActive = true;
            }
        }
    }

    public override void OnReset()
    {
        base.OnReset();
        base.info.currentState._buffOnAttackDamage.RemoveAll(x => x.item == _item && x.amount == 1f + _item.passive.increaseAD.attackDamageMult[0]);
        isActive = false;
    }
}
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Bloodthirster : ItemBase
{
    protected override void FixedUpdate()
    {
        if (transform.GetComponent<PhotonView>().IsMine)
        {
            if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
            {
                return;
            }
            if (!isActive)
            {
                if (!base.info.currentState.dead && base.info.currentState.hp / base.info.currentState.maxHP < 0.4)
                {
                    //if (base.info.currentState._buffOnArmor.Find(x => x.item == _item && x.amount == _item.passive.increaseAR.armorAdd[0]) == null && base.info.currentState._buffOnMagicResistance.Find(x => x.item == _item && x.amount == _item.passive.increaseMR.magicResistanceAdd[0]) == null)
                    //{

                    //}
                    _itemPassive.TriggerSpawn(base.info.skills.target.transform);
                    isActive = true;
                }
            }
        }
    }

    public override void OnReset()
    {
        base.OnReset();
        isActive = false;
    }
}
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item_GargoyleStoneplate : ItemBase
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
                if (!base.info.currentState.dead && base.info.currentState.hp / base.info.currentState.maxHP < 0.4f)
                {
                    Debug.Log("Item_GargoyleStoneplate active");
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
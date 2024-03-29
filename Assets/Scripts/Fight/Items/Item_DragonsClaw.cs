using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_DragonsClaw : ItemBase
{
    protected override void Awake()
    {
        base.Awake();
        cooldown = 5f;
    }

    protected override void FixedUpdate()
    {
        if (transform.GetComponent<PhotonView>().IsMine)
        {
            if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
            {
                return;
            }
            if (cooldown > 0)
            {
                cooldown -= Time.fixedDeltaTime;
            }
            else
            {
                if (!base.info.currentState.dead && base.info.currentState.hp < base.info.currentState.maxHP)
                {
                    float healing = base.info.currentState.maxHP * _item.passive.heal.extraMaxHpPercentage[0];
                    Debug.Log("Item_DragonsClaw : " + base.info.currentState.maxHP + " - " + _item.passive.heal.extraMaxHpPercentage[0] + " - " + healing);
                    base.info.stateCtrl.TriggerHeal(healing);
                    cooldown = 5f;
                }
            }
        }
    }

    public override void OnReset()
    {
        base.OnReset();
        cooldown = 5f;
    }
}
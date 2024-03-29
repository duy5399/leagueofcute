using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ArchangelsStaff : ItemBase
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
                base.info.currentState._buffOnAbilityPower.Add(new StateBuff(_item, StateBuff.TypeBuff.Add, _item.passive.increaseAP.abilityDamageAdd[0]));
                cooldown = 5f;
            }
        }
    }

    public override void OnReset()
    {
        base.OnReset();
        Debug.Log("Item_ArchangelsStaff OnReset");
        base.info.currentState._buffOnAbilityPower.RemoveAll(x => x.item == _item && x.amount == _item.passive.increaseAP.abilityDamageAdd[0]);
        cooldown = 5f;
    }
}

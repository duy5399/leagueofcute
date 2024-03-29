using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_SpearOfShojin : ItemBase
{
    [SerializeField] private const float MANA_REGEN = 6;
    public override void OnBasicAttack(SkillBase1 skillBase, Transform target)
    {
        if (transform.GetComponent<PhotonView>().IsMine)
        {
            if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
            {
                return;
            }
            Debug.Log("Item_SpearOfShojin OnBasicAttack");
            base.info.stateCtrl.TriggerManaDelta(MANA_REGEN);
        }
    }
}
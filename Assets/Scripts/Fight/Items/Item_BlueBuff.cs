using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_BlueBuff : ItemBase
{
    [SerializeField] private const float MANA_REGEN = 20f;
    public override void OnSpecialAbility(SkillBase1 skillBase, Transform target)
    {
        base.OnSpecialAbility(skillBase, target);
        if (transform.GetComponent<PhotonView>().IsMine)
        {
            if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
            {
                return;
            }
            Debug.Log("Item_BlueBuff OnSpecialAbility");
            base.info.stateCtrl.TriggerManaDelta(MANA_REGEN);
        }
    }
}

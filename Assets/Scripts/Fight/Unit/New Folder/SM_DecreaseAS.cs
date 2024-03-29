using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_DecreaseAS : SM_Buff
{
    [SerializeField] private float attackSpeedMult;

    public override void _Init()
    {
        buffID = skill.details.decreaseAS.buffID;
        haveLifeTime = skill.details.decreaseAS.haveLifeTime;
        lifeTime = (skill.details.decreaseAS.lifeTimeCanChange == true) ? skill.details.decreaseAS.lifeTime[skill.info.chStat.currentLevel.star - 1] : skill.details.decreaseAS.lifeTime[0];
        lifeTimeLeft = lifeTime;
        destroyOnLifeEnding = skill.details.decreaseAS.destroyOnLifeEnding;
        addType = skill.details.decreaseAS.addType;
        maxStackUp = skill.details.decreaseAS.maxStackUp;
        attackSpeedMult = skill.details.decreaseAS.attackSpeedMultCanChange == true ? skill.details.decreaseAS.attackSpeedMult[skill.info.chStat.currentLevel.star - 1] : skill.details.decreaseAS.attackSpeedMult[0];
    }

    //public override void OnLaunch()
    //{
    //    base.info.currentState._buffOnAttackSpeed.Add(new StateBuff(this, StateBuff.TypeBuff.Mult, 1f - attackSpeedMult));
    //}

    public override void OnLaunch()
    {
        Debug.Log("SM_DecreaseAS OnLaunch: " + attackSpeedMult);
        transform.GetComponent<PhotonView>().RPC(nameof(RPC_ActiveDebuff), RpcTarget.All, attackSpeedMult);
    }

    [PunRPC]
    void RPC_ActiveDebuff(float _attackSpeedMult)
    {
        if (base.info.photonView.IsMine)
        {
            Debug.Log("SM_DecreaseAS ActiveDebuff: " + _attackSpeedMult);
            base.info.currentState._buffOnAttackSpeed.Add(new StateBuff(this, StateBuff.TypeBuff.Mult, 1f - _attackSpeedMult));
        }
    }

    public override void WhenNotActive()
    {
        //if (base.info.currentState)
        //{
        //    base.info.currentState._buffOnAttackSpeed.RemoveAll(x => x.buff == this);
        //}
        Debug.Log("SM_DecreaseAS WhenNotActive: " );
        transform.GetComponent<PhotonView>().RPC(nameof(RPC_DeactivateDebuff), RpcTarget.All);
    }

    [PunRPC]
    void RPC_DeactivateDebuff()
    {
        if (base.info.photonView.IsMine)
        {
            if (base.info.currentState)
            {
                base.info.currentState._buffOnAttackSpeed.RemoveAll(x => x.buff == this);
                float num = base.info.currentState.mfEx(base.info.chStat.attackSpeed, base.info.currentState._buffOnAttackSpeed);
                base.info.currentState.attackSpeed = (num > 5f) ? 5f : num;
            }
        }
    }

    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}

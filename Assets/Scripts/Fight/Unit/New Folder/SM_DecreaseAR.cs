using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_DecreaseAR : SM_Buff
{
    [SerializeField] private float armorAdd;
    [SerializeField] private float armorMult;

    public override void _Init()
    {
        buffID = skill.details.decreaseAR.buffID;
        haveLifeTime = skill.details.decreaseAR.haveLifeTime;
        lifeTime = (skill.details.decreaseAR.lifeTimeCanChange == true) ? skill.details.decreaseAR.lifeTime[skill.info.chStat.currentLevel.star - 1] : skill.details.decreaseAR.lifeTime[0];
        lifeTimeLeft = lifeTime;
        destroyOnLifeEnding = skill.details.decreaseAR.destroyOnLifeEnding;
        addType = skill.details.decreaseAR.addType;
        maxStackUp = skill.details.decreaseAR.maxStackUp;
        armorAdd = skill.details.decreaseAR.armorAddCanChange == true ? skill.details.decreaseAR.armorAdd[skill.info.chStat.currentLevel.star - 1] : skill.details.decreaseAR.armorAdd[0];
        armorMult = skill.details.decreaseAR.armorMultCanChange == true ? skill.details.decreaseAR.armorMult[skill.info.chStat.currentLevel.star - 1] : skill.details.decreaseAR.armorMult[0];
    }
    public override void OnLaunch()
    {
        Debug.Log("SM_DecreaseAR OnLaunch: " + armorAdd + " - " + armorMult);
        transform.GetComponent<PhotonView>().RPC(nameof(RPC_ActiveDebuff), RpcTarget.All, armorAdd, armorMult);
    }

    [PunRPC]
    void RPC_ActiveDebuff(float _armorAdd, float _armorMult)
    {
        if (base.info.photonView.IsMine)
        {
            Debug.Log("SM_DecreaseAR ActiveDebuff: " + _armorAdd + " - " + _armorMult);
            base.info.currentState._buffOnArmor.Add(new StateBuff(this, StateBuff.TypeBuff.Add, - _armorAdd));
            base.info.currentState._buffOnArmor.Add(new StateBuff(this, StateBuff.TypeBuff.Mult, 1 - _armorMult));
        }
    }

    public override void WhenNotActive()
    {
        //if (base.info.currentState)
        //{
        //    base.info.currentState._buffOnArmor.RemoveAll(x => x.buff == this);
        //}
        Debug.Log("SM_DecreaseAR WhenNotActive: ");
        transform.GetComponent<PhotonView>().RPC(nameof(RPC_DeactivateDebuff), RpcTarget.All);
    }
    [PunRPC]
    void RPC_DeactivateDebuff()
    {
        if (base.info.photonView.IsMine)
        {
            if (base.info.currentState)
            {
                base.info.currentState._buffOnArmor.RemoveAll(x => x.buff == this);
                float num = base.info.currentState.mfEx(base.info.chStat.armor, base.info.currentState._buffOnArmor);
                base.info.currentState.armor = (num < 0f) ? 0f : num;
            }
        }
    }
    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}

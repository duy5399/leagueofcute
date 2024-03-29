using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillBase1;

public class SM_Shield : SM_Buff
{
    [SerializeField] private float shieldAmount;
    [SerializeField] private float extraMaxHpPercentage;
    [SerializeField] private float extraCurrentHpPercentage;
    [SerializeField] private float extraMissingHpPercentage;
    [SerializeField] private float extraADPercentage;
    [SerializeField] private float extraAPPercentage;


    public override void _Init()
    {
        buffID = skill.details.shield.buffID;
        haveLifeTime = skill.details.shield.haveLifeTime;
        lifeTime = (skill.details.shield.lifeTimeCanChange == true) ? skill.details.shield.lifeTime[skill.info.chStat.currentLevel.star - 1] : skill.details.shield.lifeTime[0];
        lifeTimeLeft = lifeTime;
        destroyOnLifeEnding = skill.details.shield.destroyOnLifeEnding;
        addType = skill.details.shield.addType;
        maxStackUp = skill.details.shield.maxStackUp;
        shieldAmount = skill.details.shield.shieldAmountCanChange == true ? skill.details.shield.shieldAmount[skill.info.chStat.currentLevel.star - 1] : skill.details.shield.shieldAmount[0];
        extraMaxHpPercentage = skill.details.shield.extraMaxHpPercentageCanChange == true ? skill.details.shield.extraMaxHpPercentage[skill.info.chStat.currentLevel.star - 1] : skill.details.shield.extraMaxHpPercentage[0];
        extraCurrentHpPercentage = skill.details.shield.extraCurrentHpPercentageCanChange == true ? skill.details.shield.extraCurrentHpPercentage[skill.info.chStat.currentLevel.star - 1] : skill.details.shield.extraCurrentHpPercentage[0];
        extraMissingHpPercentage = skill.details.shield.extraMissingHpPercentageCanChange == true ? skill.details.shield.extraMissingHpPercentage[skill.info.chStat.currentLevel.star - 1] : skill.details.shield.extraMissingHpPercentage[0];
    }

    public float shieldValue
    {
        get
        {
            return (shieldAmount + (extraMaxHpPercentage * base.currentState.maxHP) + (extraCurrentHpPercentage * base.currentState.hp) + extraMissingHpPercentage * (base.currentState.maxHP - base.currentState.hp)) * currentCasterStatus.abilityPower/100;
        }
    }

    public override void OnLaunch()
    {
        if (base.info.currentState != null)
        {
            base.info.currentState._buffOnShield.Add(new StateBuff(this, StateBuff.TypeBuff.Add, shieldValue));
        }
    }

    public override void WhenNotActive()
    {
        if (base.info.currentState)
        {
            base.info.currentState._buffOnShield.RemoveAll(x => x.buff == this);
        }
    }

    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.PhotonSerializeView(stream, info);
        if (stream.IsWriting)       //gửi dữ liệu
        {
            stream.SendNext(buffID);
            stream.SendNext(haveLifeTime);
            stream.SendNext(lifeTime);
            stream.SendNext(lifeTimeLeft);
            stream.SendNext(destroyOnLifeEnding);
            stream.SendNext(controlType);
            stream.SendNext(addType);
            stream.SendNext(maxStackUp);
            stream.SendNext(isActive);

            stream.SendNext(shieldAmount);
            stream.SendNext(extraMaxHpPercentage);
            stream.SendNext(extraCurrentHpPercentage);
            stream.SendNext(extraMissingHpPercentage);
            stream.SendNext(extraADPercentage);
            stream.SendNext(extraAPPercentage);
        }
        else if (stream.IsReading)  //nhận dữ liệu
        {
            buffID = (string)stream.ReceiveNext();
            haveLifeTime = (bool)stream.ReceiveNext();
            lifeTime = (float)stream.ReceiveNext();
            lifeTimeLeft = (float)stream.ReceiveNext();
            destroyOnLifeEnding = (bool)stream.ReceiveNext();
            controlType = (ControlType)(int)stream.ReceiveNext();
            addType = (AddType)(int)stream.ReceiveNext();
            maxStackUp = (int)stream.ReceiveNext();
            isActive = (bool)stream.ReceiveNext();

            shieldAmount = (float)stream.ReceiveNext();
            extraMaxHpPercentage = (float)stream.ReceiveNext();
            extraCurrentHpPercentage = (float)stream.ReceiveNext();
            extraMissingHpPercentage = (float)stream.ReceiveNext();
            extraADPercentage = (float)stream.ReceiveNext();
            extraAPPercentage = (float)stream.ReceiveNext();
        }
    }
}

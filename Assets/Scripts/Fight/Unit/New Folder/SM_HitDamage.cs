using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SM_HitDamage;

public class SM_HitDamage : SkillSpawn1
{
    [Serializable]
    public class DamageInfo
    {
        public SkillBase1.DamageType damageType;
        public bool canCrit;
        public bool lifeSteal;
        public bool spellVamp;
        public bool blind;
    }

    [SerializeField] private DamageInfo _damageInfo;
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private float _multiplier = 1f;
    public DamageInfo damageInfo
    {
        get { return _damageInfo; }
        set { _damageInfo = value; }
    }
    public float multiplier
    {
        get { return _multiplier; }
        set { _multiplier = value; }
    }

    private float damage
    {
        get
        {
            float ad = (skill.details.hitDamage.adCanChange == true) ? skill.details.hitDamage.ad[skill.info.chStat.currentLevel.star -1] : skill.details.hitDamage.ad[0];
            float ap = (skill.details.hitDamage.apCanChange == true) ? skill.details.hitDamage.ap[skill.info.chStat.currentLevel.star - 1] : skill.details.hitDamage.ap[0];
            float trueDmg = (skill.details.hitDamage.trueDmgCanChange == true) ? skill.details.hitDamage.trueDmg[skill.info.chStat.currentLevel.star - 1] : skill.details.hitDamage.trueDmg[0];
            float adMultiplier = (skill.details.hitDamage.adMultiplierCanChange == true) ? skill.details.hitDamage.adMultiplier[skill.info.chStat.currentLevel.star - 1] : skill.details.hitDamage.adMultiplier[0];
            float apMultiplier = (skill.details.hitDamage.apMultiplierCanChange == true) ? skill.details.hitDamage.apMultiplier[skill.info.chStat.currentLevel.star - 1] : skill.details.hitDamage.apMultiplier[0];
            return ((ad + ap + trueDmg) * currentCasterStatus.abilityPower / 100) + (adMultiplier * currentCasterStatus.attackDamage) + (apMultiplier * currentCasterStatus.abilityPower/100);
        }
    }

    public void Launch()
    {
        TriggerDamage();
    }

    public void TriggerDamage()
    {
        if (base.currentState.hp <= 0 || base.currentState.dead)
        {
            DestroySpawn(this.gameObject);
            return;
        }
        if (skill == null || skill != null && (skill.details.condition == SkillBase1.Condition.BasicAttack && skill.info.stateCtrl.blindTimeLeft > 0f) || (skill.details.condition == SkillBase1.Condition.BasicAttack && base.stateCtrl.dodgeTimeLeft > 0f))
        {
            Debug.Log("TriggerDamage false");
            return;
        }
        else
        {
            float num = damage;
            bool isCritical = false;
            if (currentCasterStatus != null)
            {
                if (currentCasterStatus.skill.details.condition == SkillBase1.Condition.BasicAttack)
                {
                    if (damageInfo.canCrit && UnityEngine.Random.Range(0f, 1f) <= currentCasterStatus.criticalStrikeChance)
                    {
                        isCritical = true;
                        num *= currentCasterStatus.criticalStrikeDamage;
                    }
                }
                else if (currentCasterStatus.skill.details.condition == SkillBase1.Condition.Active)
                {
                    if (currentCasterStatus.skill.details.canCrit || currentCasterStatus.info.items.itemLst.Exists(x => x.GetComponent<ItemBase>().item.idItem == "infinity_edge" || x.GetComponent<ItemBase>().item.idItem == "jeweled_gauntlet"))
                    {
                        isCritical = true;
                        num *= currentCasterStatus.criticalStrikeDamage;
                    }
                }
                transform.parent.GetComponent<Weakness>().info.stateCtrl.TriggerDamage(num, currentCasterStatus, damageInfo, isCritical);
            }
            //Debug.Log("TriggerDamage SM_HitDamage: " + num + " - From: " + currentCasterStatus.owner + " - To: " + base.stateCtrl.name);
        }
        WaitFor(lifeTime, delegate
        {
            DestroySpawn(this.gameObject);
        });
    }

    
    public override void SetParent(int photonviewParent, string pathParent = null)
    {
        photonView.RPC(nameof(RPC_SetParent), RpcTarget.AllBuffered, photonviewParent, pathParent);
    }

    [PunRPC]
    void RPC_SetParent(int viewID, string pathParent)
    {
        Transform _parent = PhotonView.Find(viewID).transform;
        if (pathParent == null)
        {
            transform.parent = _parent;
        }
        else
        {
            Transform _pathParent = _parent.Find(pathParent);
            transform.parent = _pathParent;
        }
    }

    public virtual void Destroy()
    {
        photonView.RPC(nameof(RPC_Destroy), RpcTarget.All);
    }

    [PunRPC]
    protected virtual void RPC_Destroy()
    {
        Destroy(this.gameObject);
    }

    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}

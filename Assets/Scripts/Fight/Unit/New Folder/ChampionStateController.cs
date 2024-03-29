using BestHTTP.Examples;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static SkillBase1;
using static SM_HitDamage;
using static UnityEngine.UI.GridLayoutGroup;

public class ChampionStateController : ChampionBase
{
    public enum ColliderStatus
    {
        Normal = 0,
        Transparent = 1
    }

    [SerializeField] private bool _inCombat = false;
    [SerializeField] private float _silenceTimeLeft = 0f;
    [SerializeField] private float _blindTimeLeft = 0f;
    [SerializeField] private float _attackDisableTimeLeft = 0f;
    [SerializeField] private float _moveDisableTimeLeft = 0f;
    [SerializeField] private float _slowMoveTimeLeft = 0f;
    [SerializeField] private float _dodgeTimeLeft = 0f;

    public bool inCombat
    {
        get { return _inCombat; }
        set { _inCombat = value; }
    }
    public float silenceTimeLeft
    {
        get
        {
            return _silenceTimeLeft;
        }
        set
        {
            if (_silenceTimeLeft <= value)
                _silenceTimeLeft = value;
        }
    }
    public float blindTimeLeft
    {
        get
        {
            return _blindTimeLeft;
        }
        set
        {
            if (_blindTimeLeft <= value)
                _blindTimeLeft = value;
        }
    }
    public float attackDisableTimeLeft
    {
        get
        {
            return _attackDisableTimeLeft;
        }
        set
        {
            if (_attackDisableTimeLeft <= value)
                _attackDisableTimeLeft = value;
        }
    }
    public float moveDisableTimeLeft
    {
        get
        {
            return _moveDisableTimeLeft;
        }
        set
        {
            if (_moveDisableTimeLeft <= value)
                _moveDisableTimeLeft = value;
        }
    }
    public float slowMoveTimeLeft
    {
        get
        {
            return _slowMoveTimeLeft;
        }
        set
        {
            if (_slowMoveTimeLeft <= value)
                _slowMoveTimeLeft = value;
        }
    }
    public float dodgeTimeLeft
    {
        get
        {
            return _dodgeTimeLeft;
        }
        set
        {
            if (_dodgeTimeLeft <= value)
                _dodgeTimeLeft = value;
        }
    }

    private void Update()
    {
        if (_silenceTimeLeft > 0f)
        {
            _silenceTimeLeft = _silenceTimeLeft - Time.deltaTime;
        }
        else
        {
            _silenceTimeLeft = 0f;
        }
        if (_attackDisableTimeLeft > 0f)
        {
            _attackDisableTimeLeft = _attackDisableTimeLeft - Time.deltaTime;
        }
        else
        {
            _attackDisableTimeLeft = 0f;
        }
        if (_moveDisableTimeLeft > 0f)
        {
            _moveDisableTimeLeft = _moveDisableTimeLeft - Time.deltaTime;
        }
        else
        {
            _moveDisableTimeLeft = 0f;
        }
        if (_blindTimeLeft > 0f)
        {
            _blindTimeLeft = _blindTimeLeft - Time.deltaTime;
        }
        else
        {
            _blindTimeLeft = 0f;
        }
        if (_slowMoveTimeLeft > 0f)
        {
            _slowMoveTimeLeft = _slowMoveTimeLeft - Time.deltaTime;
        }
        else
        {
            _slowMoveTimeLeft = 0f;
        }
        if (_dodgeTimeLeft > 0f)
        {
            _dodgeTimeLeft = _dodgeTimeLeft - Time.deltaTime;
        }
        else
        {
            _dodgeTimeLeft = 0f;
        }
    }

    public void InitStateCtrl()
    {
        _inCombat = false;
        _silenceTimeLeft = 0f;
        _blindTimeLeft = 0f;
        _attackDisableTimeLeft = 0f;
        _moveDisableTimeLeft = 0f;
        _slowMoveTimeLeft = 0f;
        _dodgeTimeLeft = 0f;
    }

    //public void TriggerDamage(float damage, SkillSpawn1.CurrentCasterStatus casterStatus, SM_HitDamage.DamageInfo damageInfo, bool isCritical)
    //{
    //    if(base.info.currentState.hp <= 0 || base.info.currentState.dead)
    //    {
    //        return;
    //    }
    //    //Debug.Log("TriggerDamage ChampionCtrlState: " + damage + " - From: " + casterStatus.owner + " - To: " + base.info.transform.name);
    //    float num = damage;
    //    float num1 = num * 0.01f;
    //    num = CalculateArmor(num, casterStatus, damageInfo, isCritical);
    //    float num2 = num * 0.07f;
    //    //Debug.Log("triggerDamage regen mana: " + (num1 + num2));
    //    //TriggerManaDelta((num1 + num2) > 42.5f ? 42.5f : (num1 + num2));
    //    TriggerDamageEx(num, casterStatus, damageInfo, isCritical);
    //}
    public void TriggerDamage(float damage, SkillSpawn1.CurrentCasterStatus casterStatus, SM_HitDamage.DamageInfo damageInfo, bool isCritical)
    {
        this.GetComponent<PhotonView>().RPC(nameof(RPC_TriggerDamage), RpcTarget.All, damage, casterStatus.casterViewID,
                                                                                              casterStatus.owner,
                                                                                              casterStatus.attackDamage,
                                                                                              casterStatus.physicalVamp,
                                                                                              casterStatus.abilityPower,
                                                                                              casterStatus.spellVamp,
                                                                                              casterStatus.armorPenetration,
                                                                                              casterStatus.armorPenetrationPercentage,
                                                                                              casterStatus.magicPenetration,
                                                                                              casterStatus.magicPenetrationPercentage,
                                                                                              casterStatus.criticalStrikeChance,
                                                                                              casterStatus.criticalStrikeDamage,
                                                                                              (int)damageInfo.damageType,
                                                                                              damageInfo.canCrit,
                                                                                              damageInfo.lifeSteal,
                                                                                              damageInfo.spellVamp,
                                                                                              damageInfo.blind, isCritical);
    }

    private float CalculateArmor(float damage, int casterViewID,
                                                string owner,
                                                float attackDamage,
                                                float physicalVamp,
                                                float abilityPower,
                                                float spellVamp,
                                                float armorPenetration,
                                                float armorPenetrationPercentage,
                                                float magicPenetration,
                                                float magicPenetrationPercentage,
                                                float criticalStrikeChance,
                                                float criticalStrikeDamage,
                                                int damageType,
                                                bool canCrit,
                                                bool lifeSteal,
                                                bool damageInfoSpellVamp,
                                                bool blind, bool isCritical)
    {
        float num = damage;
        if ((DamageType)damageType == SkillBase1.DamageType.AD)
        {
            //Debug.Log("CalculateArmor: " + damage + " - " + base.info.transform.name + " / armor: " + base.currentState.armor);
            float num2 = (1f - armorPenetrationPercentage) * base.currentState.armor - armorPenetration;
            if (num2 < 0f)
            {
                num2 = 0f;
            }
            num = num * 100 / (100 + num2);
        }
        else if ((DamageType)damageType == SkillBase1.DamageType.AP)
        {
            float num3 = (1f - magicPenetrationPercentage) * base.currentState.magicResistance - magicPenetration;
            if (num3 < 0f)
            {
                num3 = 0f;
            }
            num = num * 100 / (100 + num3);
        }
        return num;
    }
    private void TriggerDamageEx(float damage, int casterViewID,
                                                string owner,
                                                float attackDamage,
                                                float physicalVamp,
                                                float abilityPower,
                                                float spellVamp,
                                                float armorPenetration,
                                                float armorPenetrationPercentage,
                                                float magicPenetration,
                                                float magicPenetrationPercentage,
                                                float criticalStrikeChance,
                                                float criticalStrikeDamage,
                                                int damageType,
                                                bool canCrit,
                                                bool lifeSteal,
                                                bool damageInfoSpellVamp,
                                                bool blind, bool isCritical)
    {
        float num = damage;
        ChampionInfo1 caster = PhotonView.Find(casterViewID).GetComponent<ChampionInfo1>();
        if (caster)
        {
            if (lifeSteal && physicalVamp > 0)
            {
                //Debug.Log("TriggerDamageEx lifeSteal: " + damage * casterStatus.physicalVamp);
                caster.info.stateCtrl.TriggerHeal(damage * physicalVamp);
            }
            if (damageInfoSpellVamp && spellVamp > 0)
            {
                //Debug.Log("TriggerDamageEx spellVamp: " + damage * casterStatus.spellVamp);
                caster.info.stateCtrl.TriggerHeal(damage * spellVamp);
            }
        }
        if (base.info.hpBar != null)
        {
            //Debug.Log("TriggerDamageEx: base.info.hpBar != null");
            switch ((DamageType)damageType)
            {
                case SkillBase1.DamageType.AD:
                    base.info.hpBar.SetDamagePopup(damage, isCritical ? HealthbarManager.ColorStyle.CriticalDamage : HealthbarManager.ColorStyle.PhysicalDamage);
                    break;
                case SkillBase1.DamageType.AP:
                    base.info.hpBar.SetDamagePopup(damage, HealthbarManager.ColorStyle.MagicalDamage);
                    break;
                case SkillBase1.DamageType.True:
                    base.info.hpBar.SetDamagePopup(damage, HealthbarManager.ColorStyle.TrueDamage);
                    break;
            }
        }
        num = TriggerShieldDamage(num);
        if (num > 0f)
        {
            base.currentState.hp -= num;
        }
        else
        {
            num = 0f;
        }
        if (caster.info.items)
        {
            //Debug.Log("TriggerDamage OnHit: From: " + casterStatus.owner + " - To: " + base.info.transform.name);
            caster.info.items.OnHit(base.transform, num, isCritical);
        }
        if (base.items)
        {
            //Debug.Log("TriggerDamage OnBeHited: From: " + casterStatus.owner + " - To: " + base.info.transform.name);
            base.items.OnBeHited(caster.transform, num, isCritical);
        }
        if (base.currentState.hp <= 0f && !base.currentState.dead)
        {
            //Debug.Log("TriggerDeath");
            TriggerDeath(caster.info.currentState);
        }
    }

    public virtual float TriggerShieldDamage(float damage)
    {
        if (damage <= 0f || base.currentState.shield <= 0f)
        {
            return damage;
        }
        float num = damage;
        while (base.currentState._buffOnShield.Count > 0 && num > 0f)
        {
            StateBuff shieldBuff = base.info.currentState._buffOnShield.LastOrDefault();
            if (shieldBuff.amount > num)
            {
                shieldBuff.amount = shieldBuff.amount - num;
                num = 0f;
            }
            else
            {
                num -= shieldBuff.amount;
                base.currentState._buffOnShield.Remove(shieldBuff);
            }
        }
        if (num > 0f)
        {
            if (num > base.currentState.shield)
            {
                base.currentState.shield = 0f;
                num -= base.currentState.shield;
            }
            else
            {
                base.currentState.shield -= num;
                num = 0f;
            }
        }
        return num;
    }

    public virtual void TriggerDeath(ChampionState stateFrom = null)
    {
        if (base.currentState.dead)
        {
            return;
        }
        SocketIO.instance._unitManagerSocketIO.Emit_RemoveUnitFromBattlefield(base.info.chStat, SocketIO.instance._battlefieldSocketIO.battlefieldName);
        Debug.Log("TriggerDeath: " + transform.name + " killer: " + stateFrom.name);
        if (stateFrom != null && (base.info.chCategory == ChampionInfo1.Categories.Hero || base.info.chCategory == ChampionInfo1.Categories.Monster))
        {
            //stateFrom.stateCtrl.TriggerManaDelta(20f);
        }
        base.info.currentState.unselectable = true;
        base.info.currentState.dead = true;
        if (base.info.skills != null)
        {
            base.info.skills.target = null;
            base.info.skills.ForceInterrupt();
        }
        if (base.info.moveManager != null)
        {
            base.info.moveManager.locked = true;
            base.info.moveManager.SetNearestTarget(null);
        }
        if (base.info.buffs)
        {
            //Transform buffManager = base.info.buffs.transform;
            //for (int i = 0; i < buffManager.childCount; i++)
            //{
            //    GameObject buff = buffManager.GetChild(i).gameObject;
            //    buff.GetComponent<SM_Buff>().Destroy();
            //}
            base.buffs.DestoryAllBuffs();
        }
        if (base.info.weakness)
        {
            Transform weakness = base.info.weakness.transform;
            for (int i = 0; i < weakness.childCount; i++)
            {
                GameObject obj = weakness.GetChild(i).gameObject;
                if (!obj.GetComponent<FloatingTextPopupManager>())
                {
                    try
                    {
                        obj.GetComponent<SM_Buff>().Destroy();
                    }
                    catch
                    {

                    }
                    //try
                    //{
                    //    obj.GetComponent<SM_HitDamage>().Destroy();
                    //}
                    //catch
                    //{

                    //}
                }
            }
        }
        if (base.info.anim)
        {
            base.info.anim.TriggerDeath(true);
        }
    }

    public void TriggerManaDelta(float amount)
    {
        //Debug.Log("TriggerManaDelta: " + amount + " - " + base.info.currentState.mana + " - " + base.info.currentState.maxMana);
        base.info.currentState.mana += amount;
        if (base.info.currentState.mana > base.info.currentState.maxMana)
        {
            base.info.currentState.mana = base.info.currentState.maxMana;
        }
        if (base.info.currentState.mana < 0f)
        {
            base.info.currentState.mana = 0f;
        }
    }

    public void TriggerHeal(float heal)
    {
        if (base.currentState.hp > 0f && !base.currentState.dead)
        {
            base.currentState.hp += heal;
            if (base.currentState.hp > base.currentState.maxHP)
            {
                base.currentState.hp = base.currentState.maxHP;
            }
            if (base.info.hpBar != null)
            {
                base.info.hpBar.SetDamagePopup(heal, HealthbarManager.ColorStyle.Heal);
            }
        }
    }

    public virtual ChampionStateController TriggerLifeSteal(float lifeSteal)
    {
        if (lifeSteal > 0f)
        {
            base.currentState.hp += lifeSteal;
            if (base.currentState.hp > base.currentState.maxHP)
            {
                base.currentState.hp = base.currentState.maxHP;
            }
            if (base.info.hpBar != null)
            {
                base.info.hpBar.SetDamagePopup(lifeSteal, HealthbarManager.ColorStyle.Heal);
            }
        }
        return this;
    }

    #region PunRPC
    [PunRPC]
    public void RPC_TriggerDamage(float damage, int casterViewID,
                                                string owner,
                                                float attackDamage,
                                                float physicalVamp,
                                                float abilityPower,
                                                float spellVamp,
                                                float armorPenetration,
                                                float armorPenetrationPercentage,
                                                float magicPenetration,
                                                float magicPenetrationPercentage,
                                                float criticalStrikeChance,
                                                float criticalStrikeDamage, 
                                                int damageType,
                                                bool canCrit,
                                                bool lifeSteal,
                                                bool damageInfoSpellVamp,
                                                bool blind, bool isCritical)
    {
        if (photonView.IsMine)
        {
            if (base.info.currentState.hp <= 0 || base.info.currentState.dead)
            {
                return;
            }
            Debug.Log("TriggerDamage ChampionCtrlState: " + damage + " - From: " + owner + " - To: " + base.info.transform.name);
            float num = damage;
            float num1 = num * 0.01f;
            num = CalculateArmor(num, casterViewID,
                                      owner,
                                      attackDamage,
                                      physicalVamp,
                                      abilityPower,
                                      spellVamp,
                                      armorPenetration,
                                      armorPenetrationPercentage,
                                      magicPenetration,
                                      magicPenetrationPercentage,
                                      criticalStrikeChance,
                                      criticalStrikeDamage,
                                      damageType,
                                      canCrit,
                                      lifeSteal,
                                      damageInfoSpellVamp,
                                      blind, isCritical);
            float num2 = num * 0.1f;
            //Debug.Log("triggerDamage regen mana: " + (num1 + num2));
            TriggerManaDelta((num1 + num2) > 42.5f ? 42.5f : (num1 + num2));
            TriggerDamageEx(num, casterViewID,
                                      owner,
                                      attackDamage,
                                      physicalVamp,
                                      abilityPower,
                                      spellVamp,
                                      armorPenetration,
                                      armorPenetrationPercentage,
                                      magicPenetration,
                                      magicPenetrationPercentage,
                                      criticalStrikeChance,
                                      criticalStrikeDamage,
                                      damageType,
                                      canCrit,
                                      lifeSteal,
                                      damageInfoSpellVamp,
                                      blind, isCritical);
        }
    }
    #endregion

    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)       //gửi dữ liệu
        {
            //stream.SendNext(inCombat);
            if (stateCtrl)
            {
                stream.SendNext(stateCtrl.inCombat);
                stream.SendNext(stateCtrl.silenceTimeLeft);
                stream.SendNext(stateCtrl.blindTimeLeft);
                stream.SendNext(stateCtrl.attackDisableTimeLeft);
                stream.SendNext(stateCtrl.moveDisableTimeLeft);
                stream.SendNext(stateCtrl.slowMoveTimeLeft);
                stream.SendNext(stateCtrl.dodgeTimeLeft);
            }
        }
        else if (stream.IsReading)  //nhận dữ liệu
        {
            //inCombat = (bool)stream.ReceiveNext();
            if (stateCtrl)
            {
                stateCtrl.inCombat = (bool)stream.ReceiveNext();
                stateCtrl.silenceTimeLeft = (float)stream.ReceiveNext();
                stateCtrl.blindTimeLeft = (float)stream.ReceiveNext();
                stateCtrl.attackDisableTimeLeft = (float)stream.ReceiveNext();
                stateCtrl.moveDisableTimeLeft = (float)stream.ReceiveNext();
                stateCtrl.slowMoveTimeLeft = (float)stream.ReceiveNext();
                stateCtrl.dodgeTimeLeft = (float)stream.ReceiveNext();
            }
        }
    }
}

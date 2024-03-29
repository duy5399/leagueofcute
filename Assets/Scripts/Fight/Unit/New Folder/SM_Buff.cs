using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SkillBase1;
using static SkillSpawn1;

public class SM_Buff : ChampionBase //SkillSpawn1
{
    [SerializeField] protected Transform _target;
    [SerializeField] protected SkillBase1 _skill;
    [SerializeField] protected CurrentCasterStatus _currentCasterStatus;

    [SerializeField] protected string buffID;
    [SerializeField] protected bool haveLifeTime;
    [SerializeField] protected float lifeTime;
    [SerializeField] protected float lifeTimeLeft;
    [SerializeField] protected bool destroyOnLifeEnding;
    [SerializeField] protected SkillBase1.ControlType controlType;
    [SerializeField] protected SkillBase1.AddType addType;
    [SerializeField] protected int maxStackUp;

    [SerializeField] protected bool isActive;

    public Transform target
    {
        get { return _target; }
        set { _target = value; }
    }

    public SkillBase1 skill
    {
        get { return _skill; }
        set { _skill = value; }
    }

    public CurrentCasterStatus currentCasterStatus
    {
        get { return _currentCasterStatus; }
        set { _currentCasterStatus = value; }
    }

    protected void FixedUpdate()
    {
        if (isActive == false)
        {
            return;
        }
        if (haveLifeTime == true)
        {
            if (lifeTimeLeft > 0f)
            {
                lifeTimeLeft -= Time.fixedDeltaTime;
            }
            else if (destroyOnLifeEnding)
            {
                DestorySelf();
            }
            else
            {
                isActive = false;
            }
        }
    }

    public void Init()
    {
        _Init();
    }

    public virtual void _Init()
    {
        buffID = skill.details.crowdControl.buffID;
        haveLifeTime = skill.details.crowdControl.haveLifeTime;
        lifeTime = (skill.details.crowdControl.lifeTimeCanChange == true) ? skill.details.crowdControl.lifeTime[skill.info.chStat.currentLevel.star - 1] : skill.details.crowdControl.lifeTime[0];
        lifeTimeLeft = lifeTime;
        destroyOnLifeEnding = skill.details.crowdControl.destroyOnLifeEnding;
        controlType = skill.details.crowdControl.controlType;
        addType = skill.details.crowdControl.addType;
        maxStackUp = skill.details.crowdControl.maxStackUp;
    }

    public virtual void Launch()
    {
        if (base.info.currentState == null || base.info.currentState.dead == true)
        {
            //Debug.Log("SM_Buff base.info.currentState == null || base.info.currentState.dead == true");
            if (transform.GetComponent<PhotonView>().IsMine)
            {
                PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
            }
        }

        Init();

        if (buffID != string.Empty && base.buffs.HasBuff(buffID))
        {
            switch (addType)
            {
                case SkillBase1.AddType.UpdateLifetime:
                    if (!base.buffs.HasBuff(buffID))
                    {
                        break;
                    }
                    base.buffs.GetBuffs(buffID).ForEach(x =>
                    {
                        if (x.lifeTimeLeft < lifeTime)
                        {
                            x.lifeTimeLeft = lifeTime;
                        }
                    });
                    ControlEffect(controlType,lifeTime);
                    if (transform.GetComponent<PhotonView>().IsMine)
                    {
                        PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
                    }
                    return;
                case SkillBase1.AddType.StackUp:
                    if (!base.buffs.HasBuff(buffID))
                    {
                        break;
                    }
                    List<SM_Buff> buffs = base.buffs.GetBuffs(buffID);
                    foreach(SM_Buff buff in buffs)
                    {
                        buff.lifeTimeLeft = lifeTime;
                    };
                    if (buffs.Count >= maxStackUp)
                    {
                        if (transform.GetComponent<PhotonView>().IsMine)
                        {
                            PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
                        }
                        return;
                    }
                    break;
            }
        }
        base.buffs.AddBuff(buffID, this);
        _Launch();
    }

    public virtual void _Launch()
    {
        lifeTimeLeft = lifeTime;
        isActive = true;
        ControlEffect(controlType, lifeTime);
        OnLaunch();
        TriggerSpawn();
        //Debug.Log("SM_Buff: _Launch");
    }

    public void InterrupterTargetsSkill()
    {
        if (base.skills != null)
        {
            base.skills.ForceInterrupt();
        }
    }

    public void ControlEffect(ControlType _controlType, float _lifetime)
    {
        photonView.RPC(nameof(RPC_ControlEffect), RpcTarget.AllBuffered, (int)_controlType, _lifetime);
        //if (base.stateCtrl != null)
        //{
        //    switch (controlType)
        //    {
        //        case SkillBase1.ControlType.Stun:
        //            base.stateCtrl.attackDisableTimeLeft = lifeTime;
        //            base.stateCtrl.moveDisableTimeLeft = lifeTime;
        //            base.stateCtrl.silenceTimeLeft = lifeTime;
        //            InterrupterTargetsSkill();
        //            break;
        //        case SkillBase1.ControlType.Silence:
        //            base.stateCtrl.silenceTimeLeft = lifeTime;
        //            InterrupterTargetsSkill();
        //            break;
        //        case SkillBase1.ControlType.Slow:
        //            base.stateCtrl.slowMoveTimeLeft = lifeTime;
        //            break;
        //        case SkillBase1.ControlType.Blind:
        //            base.stateCtrl.blindTimeLeft = lifeTime;
        //            break;
        //        default: 
        //            break;
        //    }
        //}
    }

    public void TriggerSpawn(Transform _target = null)
    {
        if (_target == null)
        {
            _target = target;
        }
        GetComponents<SkillSpawn1>().ToList().ForEach(x =>
        {
            Debug.Log(x.info.name + " - " + x.name);
            x.skill = skill;
            x.currentCasterStatus = currentCasterStatus;
            x.Spawn(_target);
        });
    }

    public virtual void OnLaunch()
    {
    }

    public virtual void WhenNotActive()
    {
    }

    public void DestorySelf()
    {
        isActive = false;
        if (base.buffs != null)
        {
            base.buffs.GetBuffs(buffID).Remove(this);
        }
        WhenNotActive();
        Destroy();
        //if (transform.GetComponent<PhotonView>().IsMine)
        //{
        //    PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
        //}
    }
    public virtual void OnBasicAttack(SkillBase1 skillBase, Transform target)
    {
    }

    public virtual void OnSpecialAbility(SkillBase1 skillBase, Transform target)
    {
    }

    public virtual void OnHit(Transform target, float damage, CurrentCasterStatus casterStatus, SM_HitDamage.DamageInfo damageInfo, bool isCritical)
    {
    }

    public virtual void OnBeHited(float damage, CurrentCasterStatus casterStatus, SM_HitDamage.DamageInfo damageInfo, bool isCritical)
    {
    }

    public override void SetParent(int photonviewParent, string pathParent = null)
    {
        photonView.RPC(nameof(RPC_SetParent), RpcTarget.AllBuffered, photonviewParent, pathParent);
    }

    public virtual void Destroy()
    {
        photonView.RPC(nameof(RPC_Destroy), RpcTarget.All);
    }

    #region PunRPC
    [PunRPC]
    public void RPC_ControlEffect(int _controlType, float _lifetime)
    {
        if (base.info.photonView.IsMine)
        {
            if (base.stateCtrl != null)
            {
                Debug.Log("RPC_ControlEffect: " + this.transform.name + " - " + _lifetime);

                switch ((ControlType)_controlType)
                {
                    case SkillBase1.ControlType.Stun:
                        base.stateCtrl.attackDisableTimeLeft = _lifetime;
                        base.stateCtrl.moveDisableTimeLeft = _lifetime;
                        base.stateCtrl.silenceTimeLeft = _lifetime;
                        InterrupterTargetsSkill();
                        break;
                    case SkillBase1.ControlType.Silence:
                        base.stateCtrl.silenceTimeLeft = _lifetime;
                        InterrupterTargetsSkill();
                        break;
                    case SkillBase1.ControlType.Slow:
                        base.stateCtrl.slowMoveTimeLeft = _lifetime;
                        break;
                    case SkillBase1.ControlType.Blind:
                        base.stateCtrl.blindTimeLeft = _lifetime;
                        break;
                    default:
                        break;
                }
            }
        }
    }
    [PunRPC]
    protected void RPC_SetParent(int viewID, string pathParent)
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

    [PunRPC]
    protected virtual void RPC_Destroy()
    {
        Destroy(this.gameObject);
    }

    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)       //gửi dữ liệu
        {
            stream.SendNext(buffID);
            stream.SendNext(haveLifeTime);
            stream.SendNext(lifeTime);
            stream.SendNext(lifeTimeLeft);
            stream.SendNext(destroyOnLifeEnding);
            stream.SendNext((int)controlType);
            stream.SendNext((int)addType);
            stream.SendNext(maxStackUp);
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
        }
    }
    #endregion
}

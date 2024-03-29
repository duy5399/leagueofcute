using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Animancer.Validate;
using static SkillSpawn1;

public class ThrowType : ChampionBase
{
    [SerializeField] protected Transform _target;
    [SerializeField] protected SkillBase1 _skill;
    [SerializeField] protected CurrentCasterStatus _currentCasterStatus;
    [SerializeField] protected float speedFly;
    [SerializeField] protected SkillBase1.TriggerOnHit triggerOnHit;
    [SerializeField] protected float hitRange = 0.3f;

    [SerializeField] protected bool isActive;

    [SerializeField] protected List<ChampionInfo1> objHitted;
    [SerializeField] protected bool canHitMultiTime;

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
    //public virtual void OnDrawGizmosSelected()
    //{
    //    if (hitRange > 0f)
    //    {
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawWireSphere(base.transform.position, hitRange);
    //    }
    //}
    private void OnEnable()
    {
        objHitted = new List<ChampionInfo1>();
    }

    public virtual void Update()
    {
        if (!isActive)
        {
            return;
        }
        if ((target == null || skill == null) && isActive)
        {
            isActive = false;
            Suicide();
            return;
        }
        Debug.Log("Throwtype: " + transform.name);
        List<Collider> hitColliders = GetCollidersInRange();
        string lstCollider = "hitColliders: ";
        foreach(Collider collider in hitColliders)
        {
            lstCollider += " - " + collider.name;
        }
        Debug.Log(transform.name + lstCollider);
        switch (triggerOnHit)
        {
            case SkillBase1.TriggerOnHit.FirstCollision:
                for (int i = 0; i < hitColliders.Count; i++)
                {
                    ChampionInfo1 chInfo = hitColliders[i].GetComponent<ChampionInfo1>();
                    if (chInfo != null && chInfo != base.info && skill.TargetAvailable(chInfo))
                    {
                        //Debug.Log("TriggerOnHit.FirstCollision");
                        TriggerSpawn(chInfo.transform);
                        Suicide();
                        break;
                    }
                }
                break;
            case SkillBase1.TriggerOnHit.EveryCollision:
                for (int i = 0; i < hitColliders.Count; i++)
                {
                    ChampionInfo1 chInfo = hitColliders[i].GetComponent<ChampionInfo1>();
                    //if (chInfo != null && chInfo != base.info && skill.TargetAvailable(chInfo) && !objHitted.Contains(chInfo))
                    //{
                    //    Debug.Log("TriggerOnHit.EveryCollision");
                    //    objHitted.Add(chInfo);
                    //    TriggerSpawn(chInfo.weakness.transform);
                    //}
                    //if(chInfo != null)
                    //{
                    //    Debug.Log(transform.name + " TriggerOnHit.EveryCollision chInfo != null " + hitColliders[i].name + " - " + chInfo.name);
                    //    if (chInfo != base.info)
                    //    {
                    //        Debug.Log(transform.name + " TriggerOnHit.EveryCollision chInfo != base.info " + hitColliders[i].name + " - " + chInfo.name);
                    //    }
                    //    if (!skill.TargetAvailable(chInfo))
                    //    {
                    //        Debug.Log(transform.name + " TriggerOnHit.EveryCollision !skill.TargetAvailable(chInfo) " + hitColliders[i].name + " - " + chInfo.name);
                    //    }
                    //}
                    if (chInfo != null && chInfo != base.info && skill.TargetAvailable(chInfo))
                    {
                        if (!canHitMultiTime && !objHitted.Contains(chInfo))
                        {
                            //Debug.Log(transform.name + " TriggerOnHit.EveryCollision canHitMultiTime");
                            objHitted.Add(chInfo);
                            TriggerSpawn(chInfo.transform);
                        }
                        else if (canHitMultiTime)
                        {
                            TriggerSpawn(chInfo.transform);
                        }
                    }
                }
                break;
            case SkillBase1.TriggerOnHit.HitTarget:
                for (int i = 0; i < hitColliders.Count; i++)
                {
                    Transform _target = hitColliders[i].transform;
                    if(target == _target)
                    {
                        ChampionInfo1 chInfo = hitColliders[i].GetComponent<ChampionInfo1>();
                        if (chInfo != null && skill.TargetAvailable(chInfo))
                        {
                            //Debug.Log("TriggerOnHit.HitTarget");
                            TriggerSpawn(chInfo.transform);
                            Suicide();
                            break;
                        }
                    }
                }
                break;
        }
    }

    protected virtual void FixedUpdate()
    {
    }

    public virtual List<Collider> GetCollidersInRange()
    {
        Debug.Log("ThrowType GetCollidersInRange" + transform.name);
        return Physics.OverlapSphere(base.transform.position, hitRange).ToList();
    }

    public virtual void Launch()
    {
    }

    protected virtual void Suicide()
    {
        if (target != null && target.tag == "Temp")
        {
            Destroy(target.gameObject);
        }
        isActive = false;
        if(photonView.IsMine)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }

    public void TriggerSpawn(Transform _target = null)
    {
        if (_target == null)
        {
            _target = target;
        }
        GetComponents<SkillSpawn1>().ToList().ForEach(x =>
        {
            //Debug.Log(x.info.name + " - " + x.name);
            x.skill = skill;
            x.currentCasterStatus = currentCasterStatus;
            x.Spawn(_target);
        });
    }
    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}

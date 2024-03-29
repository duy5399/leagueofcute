using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TT_Bounce : ThrowType
{
    [SerializeField] private int maxBounce;
    [SerializeField] protected float bounceRange;

    public SkillEffect skillEffect;

    //public override void OnDrawGizmosSelected()
    //{
    //    if (bounceRange > 0f)
    //    {
    //        Gizmos.color = Color.white;
    //        Gizmos.DrawWireSphere(base.transform.position, bounceRange);
    //    }
    //}
    public override void Launch()
    {
        if (target == null)
        {
            return;
        }
        maxBounce = skill.details.bounce.maxBounceCanChange == true ? skill.details.bounce.maxBounces[skill.info.chStat.currentLevel.star - 1] : skill.details.bounce.maxBounces[0];
        speedFly = skill.details.bounce.speedFly;
        bounceRange = skill.details.bounce.bounceRange;
        triggerOnHit = skill.details.bounce.triggerOnHit;
        skillEffect = base.transform.GetComponentInChildren<SkillEffect>();
        isActive = true;
    }

    public void Bounce()
    {
        if (maxBounce > 0)
        {
            Transform transform = ReselectTarget(target);
            if (transform != null && transform.GetComponent<ChampionBase>().info.weakness != null)
            {
                target = transform;
                maxBounce--;
                isActive = true;
                skillEffect.Play();
            }
            else
            {
                base.Suicide();
            }
        }
        else
        {
            base.Suicide();
        }
    }

    public Transform ReselectTarget(Transform target)
    {
        ChampionBase component = target.GetComponent<ChampionBase>();
        ChampionInfo1 t = null;
        if (component != null)
        {
            t = component.info;
        }
        List<Collider> colliders = Physics.OverlapSphere(target.position, bounceRange).ToList();
        foreach (var x in colliders)
        {
            ChampionBase component2 = x.GetComponent<ChampionBase>();
            if (component2 != null)
            {
                ChampionInfo1 chInfo = component2.info;
                if (chInfo != null && chInfo != t && skill.TargetAvailable(chInfo))
                {
                    if (chInfo == skill.info && !skill.details.canUseSelf)
                    {
                        continue;
                    }
                    return x.transform;
                }
            }
        }
        return null;
    }

    protected override void FixedUpdate()
    {
        if (isActive && target != null)
        {
            base.transform.position = Vector3.MoveTowards(base.transform.position, target.GetComponent<ChampionBase>().weakness.transform.position, speedFly * Time.fixedDeltaTime);
            base.transform.LookAt(target.GetComponent<ChampionBase>().weakness.transform.position);
        }
        else
        {
            Bounce();
        }
    }

    protected override void Suicide()
    {
        int maxBounce = skill.details.bounce.maxBounceCanChange == true ? skill.details.bounce.maxBounces[skill.info.chStat.currentLevel.star - 1] : skill.details.bounce.maxBounces[0];
        if (maxBounce > 0)
        {
            isActive = false;
        }
        else
        {
            base.Suicide();
        }
    }
    public override void SetParent(int photonviewParent, string pathParent = null)
    {
        photonView.RPC(nameof(RPC_SetParent), RpcTarget.AllBuffered, photonviewParent, pathParent);
    }

    [PunRPC]
    void RPC_SetParent(int viewID, string pathParent)
    {
        Transform _parent = PhotonView.Find(viewID).transform;
        Debug.Log("TT_FollowTarget serparent: " + _parent.name + " - " + pathParent);
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

    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(new Vector3(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z));
        }
        else if (stream.IsReading)
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.localRotation = Quaternion.Euler((Vector3)stream.ReceiveNext());
        }
    }
}

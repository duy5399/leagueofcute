using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SS_AOE;

public class TT_FixedDirection : ThrowType
{
    [SerializeField] protected Vector3 prePos;
    [SerializeField] protected float activeDistance;
    public override void Launch()
    {
        if (target == null)
        {
            //Destroy(base.gameObject);
            return;
        }
        prePos = base.transform.position;
        activeDistance = skill.details.fixedDirection.activeDistance;
        speedFly = skill.details.fixedDirection.speedFly;
        triggerOnHit = skill.details.fixedDirection.triggerOnHit;
        hitRange = skill.details.fixedDirection.hitRange;
        isActive = true;
    }

    protected override void FixedUpdate()
    {
        if (isActive)
        {
            if (Vector3.Distance(prePos, base.transform.position) < activeDistance)
            {
                base.transform.Translate(Vector3.forward * speedFly * Time.fixedDeltaTime);
                //base.transform.position = Vector3.MoveTowards(base.transform.position, target.position, speed * Time.fixedDeltaTime);
                //base.transform.LookAt(target);
            }
            else
            {
                Suicide();
            }
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

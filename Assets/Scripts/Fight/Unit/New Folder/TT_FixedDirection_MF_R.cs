using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TT_FixedDirection_MF_R : TT_FixedDirection
{
    public override void Launch()
    {
        base.Launch();
        hitRange = 0.1f;
    }
    protected override void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (hitRange < skill.details.fixedDirection.hitRange)
            {
                hitRange = skill.details.fixedDirection.hitRange * Vector3.Distance(prePos, base.transform.position) / activeDistance;
            }
            else
            {
                hitRange = skill.details.fixedDirection.hitRange;
            }
            if (isActive)
            {
                if (Vector3.Distance(prePos, base.transform.position) < (activeDistance - skill.details.fixedDirection.hitRange))
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
}

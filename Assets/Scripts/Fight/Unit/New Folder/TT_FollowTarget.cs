using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TT_FollowTarget : ThrowType
{
    public override void Launch()
    {
        if (target == null)
        {
            Suicide();
            return;
        }
        speedFly = skill.details.followTarget.speedFly;
        triggerOnHit = skill.details.followTarget.triggerOnHit;
        hitRange = skill.details.followTarget.hitRange;
        isActive = true;
    }

    protected override void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if(target == null || target.GetComponent<ChampionInfo1>().currentState.dead)
            {
                //Debug.Log("TT_FollowTarget FixedUpdate Suicide: " + photonView.ViewID);
                Destroy();
            }
            if (isActive)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.GetComponent<ChampionBase>().weakness.transform.position, speedFly * Time.fixedDeltaTime);
                transform.LookAt(target.GetComponent<ChampionBase>().weakness.transform.position);
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
        else if(stream.IsReading)
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.localRotation = Quaternion.Euler((Vector3)stream.ReceiveNext());
        }
    }

    public void Destroy()
    {
        photonView.RPC(nameof(RPC_Destroy), RpcTarget.All);
    }

    [PunRPC]
    protected virtual void RPC_Destroy()
    {
        Debug.Log("TT_FollowTarget FixedUpdate Suicide: " + photonView.ViewID);
        Destroy(this.gameObject);
    }
}

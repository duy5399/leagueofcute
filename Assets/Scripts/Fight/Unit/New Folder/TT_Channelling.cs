using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TT_Channelling : ThrowType
{
    [SerializeField] private float timeChanneling;
    [SerializeField] private float tickInterval;
    [SerializeField] private int tickTimes;
    [SerializeField] private float nextTriggerTime;

    public override void Launch()
    {
        timeChanneling = skill.details.channelling.timeChanneling;
        tickInterval = skill.details.channelling.tickInterval;
        tickTimes = skill.details.channelling.tickTimes;
        if (skill.details.channelling.triggerRightAway)
        {
            nextTriggerTime = timeChanneling;
        }
        else
        {
            nextTriggerTime = timeChanneling - tickInterval;
        }
    }

    protected override void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (timeChanneling <= 0 || tickTimes <= 0)
            {
                Suicide();
            }
            timeChanneling -= Time.fixedDeltaTime;
            if (timeChanneling <= nextTriggerTime)
            {
                TriggerSpawn();
                nextTriggerTime -= tickInterval;
                tickTimes--;
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

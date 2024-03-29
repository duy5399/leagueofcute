using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TT_AOE : ThrowType
{
    [SerializeField] protected int maxHitNum;
    [SerializeField] protected float lifeTime;
    public override void Launch()
    {
        hitRange = skill.details.aoe.aoeRange;
        maxHitNum = skill.details.aoe.maxHitNum;
        lifeTime = skill.details.aoe.lifeTime;
        triggerOnHit = skill.details.aoe.triggerOnHit;
        canHitMultiTime = skill.details.aoe.canHitMultiTime;
        isActive = true;
        WaitFor(lifeTime, delegate
        {
            Suicide();
            //Destroy(base.gameObject);
        });
    }

    public void TriggerHit()
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
        List<Collider> hitColliders = Physics.OverlapSphere(base.transform.position, hitRange).ToList();
        for (int i = 0; i < hitColliders.Count && i < maxHitNum; i++)
        {
            ChampionInfo1 chInfo = hitColliders[i].GetComponent<ChampionInfo1>();
            if (chInfo != null && chInfo != base.info && skill.TargetAvailable(chInfo))
            {
                TriggerSpawn(chInfo.weakness.transform);
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

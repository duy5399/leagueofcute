using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TT_AOE_Lux_R : TT_AOE
{
    public override List<Collider> GetCollidersInRange()
    {
        Debug.Log("TT_AOE_Lux_R GetCollidersInRange" + transform.name);
        Vector3 endPoint = base.transform.TransformPoint(Vector3.forward * 25);
        return Physics.OverlapCapsule(base.transform.position, endPoint, hitRange).ToList();
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
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static SS_HitDamage;

public class TT_FlyOutAndBack : ThrowType
{
    [SerializeField] private float activeDistance;
    [SerializeField] private Transform tempTargetPoint;
    [SerializeField] private bool flyOut;

    public override void Launch()
    {
        if (target == null)
        {
            //Destroy(base.gameObject);
            return;
        }
        Debug.Log("TT_FlyOutAndBack Launch");
        activeDistance = skill.details.flyOutAndBack.activeDistance;
        speedFly = skill.details.flyOutAndBack.speedFly;
        triggerOnHit = skill.details.flyOutAndBack.triggerOnHit;
        hitRange = skill.details.flyOutAndBack.hitRange;
        flyOut = true;
        tempTargetPoint = new GameObject("tempTargetPoint").transform;
        tempTargetPoint.tag = "Temp";
        tempTargetPoint.parent = skill.info.weakness.transform;
        tempTargetPoint.localPosition = transform.localRotation * Vector3.forward * activeDistance + transform.localPosition;
        isActive = true;
    }

    protected override void FixedUpdate()
    {
        if (isActive && target != null)
        {
            if (tempTargetPoint != null)
            {
                if (base.transform.position == tempTargetPoint.position)
                {
                    flyOut = false;
                    objHitted.Clear();
                }
                if(flyOut)
                {
                    base.transform.position = Vector3.MoveTowards(base.transform.position, tempTargetPoint.position, speedFly * Time.fixedDeltaTime);
                    base.transform.LookAt(tempTargetPoint);
                }
                else
                {
                    base.transform.position = Vector3.MoveTowards(base.transform.position, skill.transform.position, speedFly * Time.fixedDeltaTime);
                    base.transform.LookAt(skill.transform.position);
                }
            }
            if(flyOut == false && base.transform.position == skill.transform.position)
            {
                Destroy(tempTargetPoint.gameObject);
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

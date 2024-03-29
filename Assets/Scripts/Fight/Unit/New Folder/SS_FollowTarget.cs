using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SS_FollowTarget : SkillSpawn1
{

    public override void _Spawn(Transform target)
    {
        if(target == null)
        {
            return;
        }
        GameObject toSpawn = GetToSpawn<TT_FollowTarget>().gameObject;
        if (toSpawn != null)
        {
            TT_FollowTarget tT_FollowTarget = GenSpawnedObject(toSpawn, base.transform.position, base.transform.rotation, target).GetComponent<TT_FollowTarget>();
            if (tT_FollowTarget != null)
            {
                tT_FollowTarget.SetParent(base.info.GetComponent<PhotonView>().ViewID, "SkillManager/ObjPool");
                //Debug.Log("tT_FollowTarget != null: "+ base.info.name + "/"+base.info.GetComponent<PhotonView>().ViewID);
                tT_FollowTarget.target = target;
                tT_FollowTarget.skill = skill;
                tT_FollowTarget.currentCasterStatus = currentCasterStatus;
                tT_FollowTarget.Launch();
            }
        }
    }
}

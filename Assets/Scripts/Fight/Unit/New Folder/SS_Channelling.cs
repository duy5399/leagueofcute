using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_Channelling : SkillSpawn1
{
    public override void _Spawn(Transform target)
    {
        GameObject toSpawn = GetToSpawn<TT_Channelling>().gameObject;
        if (toSpawn != null)
        {
            TT_Channelling tT_ContinueInTime = GenSpawnedObject(toSpawn, base.transform.position, base.transform.rotation, target).GetComponent<TT_Channelling>();
            if (tT_ContinueInTime != null)
            {
                tT_ContinueInTime.SetParent(base.info.GetComponent<PhotonView>().ViewID, "SkillManager/ObjPool");
                Debug.Log("tT_ContinueInTime != null");
                tT_ContinueInTime.target = target;
                tT_ContinueInTime.skill = skill;
                tT_ContinueInTime.currentCasterStatus = currentCasterStatus;
                tT_ContinueInTime.Launch();
            }
        }
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_FixedDirection : SkillSpawn1
{
    public override void _Spawn(Transform target)
    {
        if(target == null)
        {
            return;
        }
        GameObject toSpawn = GetToSpawn<TT_FixedDirection>().gameObject;
        if (toSpawn != null)
        {
            TT_FixedDirection tT_FixedDirection = GenSpawnedObject(toSpawn, base.transform.position, base.transform.rotation, target).GetComponent<TT_FixedDirection>();
            if (tT_FixedDirection != null)
            {
                tT_FixedDirection.SetParent(base.info.GetComponent<PhotonView>().ViewID, "SkillManager/ObjPool");
                Debug.Log("tT_FixedDirection != null");
                tT_FixedDirection.target = target;
                tT_FixedDirection.skill = skill;
                tT_FixedDirection.currentCasterStatus = currentCasterStatus;
                tT_FixedDirection.Launch();
            }
        }
    }
}

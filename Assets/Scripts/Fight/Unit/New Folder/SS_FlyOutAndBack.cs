using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_FlyOutAndBack : SkillSpawn1
{
    public override void _Spawn(Transform target)
    {
        if(target == null)
        {
            return;
        }
        GameObject toSpawn = GetToSpawn<TT_FlyOutAndBack>().gameObject;
        if (toSpawn != null)
        {
            TT_FlyOutAndBack tT_FlyOutAndBack = GenSpawnedObject(toSpawn, base.transform.position, base.transform.rotation, target).GetComponent<TT_FlyOutAndBack>();
            if (tT_FlyOutAndBack != null)
            {
                tT_FlyOutAndBack.SetParent(base.info.GetComponent<PhotonView>().ViewID, "SkillManager/ObjPool");
                Debug.Log("tT_FlyOutAndBack != null");
                tT_FlyOutAndBack.target = target;
                tT_FlyOutAndBack.skill = skill;
                tT_FlyOutAndBack.currentCasterStatus = currentCasterStatus;
                tT_FlyOutAndBack.Launch();
            }
        }
    }
}

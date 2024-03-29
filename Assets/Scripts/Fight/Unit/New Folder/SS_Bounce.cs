using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SS_Bounce : SkillSpawn1
{
    public override void _Spawn(Transform target)
    {
        int maxBounce = skill.details.bounce.maxBounceCanChange == true ? skill.details.bounce.maxBounces[skill.info.chStat.currentLevel.star - 1] : skill.details.bounce.maxBounces[0];
        if (target == null || maxBounce == 0 || skill.details.bounce.bounceRange <= 0)
        {
            return;
        }
        GameObject toSpawn = GetToSpawn<TT_Bounce>().gameObject;
        if (toSpawn != null)
        {
            TT_Bounce tT_Bounce = GenSpawnedObject(toSpawn, base.transform.position, base.transform.rotation, target).GetComponent<TT_Bounce>();
            if (tT_Bounce != null)
            {
                tT_Bounce.SetParent(base.info.GetComponent<PhotonView>().ViewID, "SkillManager/ObjPool");
                Debug.Log("tT_Bounce != null");
                tT_Bounce.target = target;
                tT_Bounce.skill = skill;
                tT_Bounce.currentCasterStatus = currentCasterStatus;
                tT_Bounce.Launch();
            }
        }
    }
}

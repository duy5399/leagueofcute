using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static SS_AOE;

public class SS_AOE : SkillSpawn1
{
    [SerializeField] private SkillBase1.AoePos aoePos;
    //public void OnDrawGizmosSelected()
    //{
    //    if (transform.GetComponent<SkillBase1>().details.aoe.aoeRange > 0f)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawWireSphere(base.transform.position, skill.details.aoe.aoeRange);
    //    }
    //}

    public override void _Spawn(Transform target)
    {
        if (target == null || skill.details.searchAmong == SkillBase1.SearchAmong.None || skill.details.aoe.aoePos == SkillBase1.AoePos.None)
        {
            return;
        }
        aoePos = skill.details.aoe.aoePos;
        Vector3 pos;
        switch (aoePos)
        {
            case SkillBase1.AoePos.Self:
                pos = skill.info.weakness.transform.position;
                break;
            case SkillBase1.AoePos.Target:
                pos = target.transform.position;
                break;
            default:
                return;
        }
        GameObject toSpawn = GetToSpawn<TT_AOE>().gameObject;
        if (toSpawn != null)
        {
            TT_AOE tT_AOE = GenSpawnedObject(toSpawn, pos, Quaternion.identity, target).GetComponent<TT_AOE>();
            if (tT_AOE != null)
            {
                tT_AOE.SetParent(base.info.GetComponent<PhotonView>().ViewID, "SkillManager/ObjPool");
                Debug.Log("sM_AOE " + transform.name + " != null " + tT_AOE.name);
                tT_AOE.target = target;
                tT_AOE.skill = skill;
                tT_AOE.currentCasterStatus = currentCasterStatus;
                tT_AOE.Launch();
            }
        }
    }
}

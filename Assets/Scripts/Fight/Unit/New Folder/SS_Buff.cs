using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using static SkillBase1;

public class SS_Buff : SkillSpawn1
{
    [SerializeField] protected SkillBase1.BuffOn buffOn;
    [SerializeField] protected float buffRange;
    [SerializeField] protected int maxHitNum;
    [SerializeField] protected List<Transform> hits;

    public void OnDrawGizmosSelected()
    {
        if (buffRange > 0f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(base.info.transform.position, buffRange);
        }
    }

    protected virtual void Init()
    {
        buffOn = skill.details.crowdControl.buffOn;
        buffRange = skill.details.crowdControl.buffRange;
        maxHitNum = skill.details.crowdControl.maxHitNum;
    }

    public override void _Spawn(Transform target)
    {
        Debug.Log("SS_Buff _Spawn");
        Init();
        hits = new List<Transform>();
        if (buffOn == SkillBase1.BuffOn.Self)
        {
            hits.Add(skill.info.transform);
        }
        else if (buffOn == SkillBase1.BuffOn.Target)
        {
            if (target == null || target.tag == "Temp")
            {
                return;
            }
            ChampionBase component = target.GetComponent<ChampionBase>();
            if (component == null || component.info == null)
            {
                return;
            }
            hits.Add(component.info.transform);
        }
        else
        {
            if (target == null)
            {
                return;
            }
            List<Collider> colliders = Physics.OverlapSphere(base.transform.position, buffRange).ToList();
            foreach (var x in colliders)
            {
                ChampionInfo1 chInfo = x.GetComponent<ChampionInfo1>();
                if (chInfo != null && skill.TargetAvailable(chInfo))
                {
                    if (chInfo == skill.info && !skill.details.canUseSelf)
                    {
                        continue;
                    }
                    hits.Add(x.transform);
                    Debug.Log("SS_Buff: " + x.name);
                }
            }
        }
        SpawnBuff(target);
    }

    public void SpawnBuff(Transform target)
    {
        for (int i = 0; i < hits.Count && i < maxHitNum; i++)
        {
            ChampionBase component = hits[i].GetComponent<ChampionBase>();
            if (component.info == null)
            {
                continue;
            }
            _SpawnBuff(hits[i]);
        }
    }

    public void _SpawnBuff(Transform target)
    {
        Debug.Log("_SpawnBuff: " + target.name);
        GameObject toSpawn = GetToSpawn<SM_Buff>().gameObject;
        if (toSpawn != null)
        {
            SM_Buff sM_Buff = GenSpawnedObject(toSpawn, target.position, target.rotation, target, target).GetComponent<SM_Buff>();
            if (sM_Buff != null)
            {
                sM_Buff.SetParent(target.GetComponent<PhotonView>().ViewID, "BuffManager");
                Debug.Log("sM_Buff != null");
                sM_Buff.target = target;
                sM_Buff.skill = skill;
                sM_Buff.currentCasterStatus = currentCasterStatus;
                sM_Buff.Launch();
            }
        }
    }
}

using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class SS_HitDamage : SkillSpawn1
{
    public override void _Spawn(Transform target)
    {
        if (target == null)
        {
            return;
        }
        Transform _target = target;
        if (skill.details.hitDamage.hitOn != SkillBase1.HitOn.HitTarget)
        {
            _target = new GameObject("tempPoint").transform;
            _target.gameObject.tag = "Temp";
            _target.position = (skill.details.hitDamage.hitOn == SkillBase1.HitOn.AroundTarget) ? target.position : skill.info.transform.position;
            if (skill.details.hitDamage.hitOn == SkillBase1.HitOn.AroundSelf)
            {
                _target.parent = skill.info.weakness.transform;
            }
        }
        SpawnDamage(_target);
        if (_target == null || _target.gameObject.tag != "Temp")
        {
            return;
        }
        Destroy(_target.gameObject, 1f);
    }

    public void SpawnDamage(Transform target)
    {
        if (target == null)
        {
            return;
        }
        if (target.tag != "Temp")
        {
            _SpawnDamage(target, 1f);
        }
        if (skill.details.hitDamage.aoeRange <= 0f || skill.details.hitDamage.aoeMultiplier <= 0f)
        {
            return;
        }
        ChampionBase component = target.GetComponent<ChampionBase>();
        ChampionInfo1 t = null;
        if (component != null)
        {
            t = component.info;
        }
        Physics.OverlapSphere(target.position, skill.details.hitDamage.aoeRange).ToList().ForEach(x =>
        {
            ChampionBase component2 = x.GetComponent<ChampionBase>();
            if (component2 != null)
            {
                ChampionInfo1 chInfo = component2.info;
                if (chInfo != null && chInfo != t && skill.TargetAvailable(chInfo))
                {
                    Debug.Log("AroundTarget: " + chInfo.transform.name);
                   _SpawnDamage(chInfo.transform, skill.details.hitDamage.aoeMultiplier);
                }
            }
        });
    }

    public void _SpawnDamage(Transform target, float multiplier)
    {
        if(target == null)
        {
            return;
        }
        GameObject toSpawn = GetToSpawn<SM_HitDamage>().gameObject;
        if (toSpawn != null)
        {
            SM_HitDamage sM_HitDamage = GenSpawnedObject(toSpawn, target.position, target.rotation, target, target).GetComponent<SM_HitDamage>();
            if (sM_HitDamage != null)
            {
                sM_HitDamage.SetParent(target.GetComponent<PhotonView>().ViewID, "Weakness");
                //Debug.Log("sM_HitDamage != null");
                sM_HitDamage.skill = skill;
                sM_HitDamage.currentCasterStatus = currentCasterStatus;
                sM_HitDamage.damageInfo.damageType = skill.details.hitDamage.damageType;
                sM_HitDamage.damageInfo.canCrit = skill.details.canCrit;
                sM_HitDamage.damageInfo.lifeSteal = skill.details.hitDamage.damageType == SkillBase1.DamageType.AD ? true : false;
                sM_HitDamage.damageInfo.spellVamp = skill.details.hitDamage.damageType == SkillBase1.DamageType.AP ? true : false;
                sM_HitDamage.damageInfo.blind = skill.info.currentState.blind;
                sM_HitDamage.multiplier = multiplier;
                sM_HitDamage.Launch();
            }
        }
    }
}

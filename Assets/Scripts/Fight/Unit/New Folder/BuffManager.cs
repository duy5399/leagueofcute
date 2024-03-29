using AYellowpaper.SerializedCollections;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SkillSpawn1;

public class BuffManager : ChampionBase
{
    [SerializedDictionary("BuffID", "BuffList")]
    [SerializeField] private SerializedDictionary<string, List<SM_Buff>> _buffLst = new SerializedDictionary<string, List<SM_Buff>>();

    public SerializedDictionary<string, List<SM_Buff>> buffLst
    {
        get { return _buffLst; }
    }

    public void AddBuff(string buffid, SM_Buff buff)
    {
        if (!_buffLst.ContainsKey(buffid))
        {
            _buffLst[buffid] = new List<SM_Buff>();
        }
        _buffLst[buffid].Add(buff);
    }

    public bool HasBuff(string buffid)
    {
        return _buffLst.ContainsKey(buffid) && _buffLst[buffid].Count > 0;
    }

    public List<SM_Buff> GetBuffs(string buffid)
    {
        return _buffLst.ContainsKey(buffid) ? _buffLst[buffid] : new List<SM_Buff>();
    }

    public List<SM_Buff> GetAllBuffs()
    {
        List<SM_Buff> buffLst = _buffLst.Values.Aggregate(new List<SM_Buff>(), (List<SM_Buff> s1, List<SM_Buff> s2) => s1.Concat(s2).ToList());
        return buffLst;
    }

    public void DestoryAllBuffs()
    {
        GetAllBuffs().ForEach(x =>
        {
            if (x != null)
            {
                x.DestorySelf();
            }
        });
        _buffLst.Clear();
    }

    public void OnBasicAttack(SkillBase1 skillBase, Transform target)
    {
        foreach(var i in GetAllBuffs())
        {
            i.OnBasicAttack(skillBase, target);
        }
    }

    public void OnSpecialAbility(SkillBase1 skillBase, Transform target)
    {
        foreach (var i in GetAllBuffs())
        {
            i.OnSpecialAbility(skillBase, target);
        }
    }

    public virtual void OnHit(Transform target, float damage, CurrentCasterStatus casterStatus, SM_HitDamage.DamageInfo damageInfo, bool isCritical)
    {
        foreach (var i in GetAllBuffs())
        {
            i.OnHit(target, damage, casterStatus, damageInfo, isCritical);
        }
    }

    public virtual void OnBeHited(float damage, CurrentCasterStatus casterStatus, SM_HitDamage.DamageInfo damageInfo, bool isCritical)
    {
        foreach (var i in GetAllBuffs())
        {
            i.OnBeHited(damage, casterStatus, damageInfo, isCritical);
        }
    }
}

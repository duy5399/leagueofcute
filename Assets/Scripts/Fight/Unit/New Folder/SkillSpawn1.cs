using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillSpawn1 : ChampionBase
{
    [Serializable]
    public class CurrentCasterStatus
    {
        public int casterViewID;
        public string owner;
        public ChampionInfo1 info;
        public SkillBase1 skill;
        public float attackDamage;
        public float physicalVamp;
        public float abilityPower;
        public float spellVamp;
        public float armorPenetration;
        public float armorPenetrationPercentage;
        public float magicPenetration;
        public float magicPenetrationPercentage;
        public float criticalStrikeChance;
        public float criticalStrikeDamage;
    }

    [SerializeField] private SkillBase1 _skill;
    [SerializeField] private CurrentCasterStatus _currentCasterStatus;

    public SkillBase1 skill
    {
        get { return _skill; }
        set { _skill = value; }
    }

    public CurrentCasterStatus currentCasterStatus
    {
        get { return _currentCasterStatus; }
        set { _currentCasterStatus = value; }
    }

    public virtual void Spawn(Transform target)
    {
        _Spawn(target);
    }

    public virtual void _Spawn(Transform target)
    {
    }

    public void DestroySpawn(GameObject go)
    {
        GameObject _go = base.info.skills.skillObjPool.objPool.FirstOrDefault(x => x == go);
        if (_go != null)
        {
            base.skills.skillObjPool.objPool.Remove(go);
        }
        if (go.GetPhotonView().IsMine)
        {
            PhotonNetwork.Destroy(go.GetComponent<PhotonView>());
        }
    }

    private string GetGameObjectPath(Transform _transform)
    {
        string path = _transform.name;
        while (_transform.parent != null && !(bool)_transform.GetComponentInChildren<ChampionInfo1>())
        {
            _transform = _transform.parent;
            path = _transform.name + "/" + path;
        }
        return path;
    }

    protected Transform GetToSpawn<T>() where T : Component
    {
        foreach(Transform child in base.transform)
        {
            if(child.GetComponent<T>() == true)
            {
                //Debug.Log("GetToSpawn: " + child.name);
                return child;
            }
        }
        return null;
    }

    protected GameObject CreateSpawn(GameObject toSpawn, Vector3 pos, Quaternion rot)
    {
        GameObject obj = base.info.skills.skillObjPool.objPool.FirstOrDefault(x => x.name == toSpawn.name && x.activeSelf == false);
        if (obj != null)
        {
            obj.transform.position = pos;
            obj.transform.rotation = base.info.skills.skillObjPool.transform.rotation;
            return obj;
        }
        string path = "";
        if (toSpawn.transform.parent != null &&( toSpawn.transform.parent.tag == "ItemPassive" || toSpawn.transform.parent.tag == "SkillItem"))
        {
            path = "prefabs/fight/items/" + toSpawn.transform.parent.name + "/" + toSpawn.name;
        }
        else
        {
            path = "prefabs/fight/units/" + currentCasterStatus.info.chStat.championName + "/skills/" + skill.details.condition.ToString() + "/" + toSpawn.name;
        }
        //string path = GetGameObjectPath(toSpawn.transform);
        //Debug.Log("GetGameObjectPath: " + path);
        GameObject spawnObj = PhotonNetwork.Instantiate(path, pos, rot);
        //GameObject spawnObj = Instantiate(toSpawn, pos, base.skills.skillObjPool.transform.rotation);
        //spawnObj.name = toSpawn.name;
        base.info.skills.skillObjPool.objPool.Add(spawnObj);
        return spawnObj;
    }

    public virtual GameObject GenSpawnedObject(GameObject toSpawn, Vector3 pos, Quaternion rot, Transform target, Transform parent = null)
    {
        GameObject spawnObj = CreateSpawn(toSpawn, pos, rot);
        //if (parent != null)
        //{
        //    spawnObj.transform.parent = parent.GetComponent<ChampionBase>().info.weakness.transform;
        //}
        //else
        //{
        //    spawnObj.transform.parent = base.skills.skillObjPool.transform;
        //}
        ThrowType component = gameObject.GetComponent<ThrowType>();
        if (component != null)
        {
            //Debug.Log("ThrowType : " + component.name);
            component.skill = skill;
            component.target = target;
            component.currentCasterStatus = currentCasterStatus;
        }
        spawnObj.SetActive(true);
        return spawnObj;
    }
    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            if (currentCasterStatus != null)
            {
                stream.SendNext(currentCasterStatus.casterViewID);
                stream.SendNext(currentCasterStatus.owner);
                stream.SendNext(currentCasterStatus.attackDamage);
                stream.SendNext(currentCasterStatus.physicalVamp);
                stream.SendNext(currentCasterStatus.abilityPower);
                stream.SendNext(currentCasterStatus.spellVamp);
                stream.SendNext(currentCasterStatus.armorPenetration);
                stream.SendNext(currentCasterStatus.armorPenetrationPercentage);
                stream.SendNext(currentCasterStatus.magicPenetration);
                stream.SendNext(currentCasterStatus.magicPenetrationPercentage);
                stream.SendNext(currentCasterStatus.criticalStrikeChance);
                stream.SendNext(currentCasterStatus.criticalStrikeDamage);
            }
        }
        else if (stream.IsReading)
        {
            if (currentCasterStatus != null)
            {
                currentCasterStatus.casterViewID = (int)stream.ReceiveNext();
                currentCasterStatus.owner = (string)stream.ReceiveNext();
                currentCasterStatus.attackDamage = (float)stream.ReceiveNext();
                currentCasterStatus.physicalVamp = (float)stream.ReceiveNext();
                currentCasterStatus.abilityPower = (float)stream.ReceiveNext();
                currentCasterStatus.spellVamp = (float)stream.ReceiveNext();
                currentCasterStatus.armorPenetration = (float)stream.ReceiveNext();
                currentCasterStatus.armorPenetrationPercentage = (float)stream.ReceiveNext();
                currentCasterStatus.magicPenetration = (float)stream.ReceiveNext();
                currentCasterStatus.magicPenetrationPercentage = (float)stream.ReceiveNext();
                currentCasterStatus.criticalStrikeChance = (float)stream.ReceiveNext();
                currentCasterStatus.criticalStrikeDamage = (float)stream.ReceiveNext();
            }
        }
    }
}

using AYellowpaper.SerializedCollections;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class TraitsManager : MonoBehaviour
{
    public static TraitsManager instance { get; private set; }
    [SerializeField] private Transform background;
    [SerializeField] private GameObject prefab_trait;

    [SerializedDictionary("Class", "List Champions")]
    [SerializeField] private SerializedDictionary<GameObject, List<GameObject>> _dictTraits;

    [SerializeField] private AllTraitsBase _allTraitsBase;
    public Dictionary<GameObject, List<GameObject>> dictTraits
    {
        get { return _dictTraits; }
    }

    public AllTraitsBase allTraitsBase
    {
        get { return _allTraitsBase; }
        set { _allTraitsBase = value; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        _dictTraits = new SerializedDictionary<GameObject, List<GameObject>>();
    }

    public void AddClass(ClassBase.IdClass idClass, GameObject champion)
    {
        Debug.Log("AddClass: " + champion.name);
        GameObject goTrait = null;
        switch (idClass)
        {
            case ClassBase.IdClass.Ranger:
                goTrait = OnGetTrait<ClassRanger>(allTraitsBase.ranger);
                break;
            case ClassBase.IdClass.Assassin:
                goTrait = OnGetTrait<ClassAssassin>(allTraitsBase.assassin);
                break;
            case ClassBase.IdClass.Brawler:
                goTrait = OnGetTrait<ClassBrawler>(allTraitsBase.brawler);
                break;
            case ClassBase.IdClass.Mystic:
                goTrait = OnGetTrait<ClassMystic>(allTraitsBase.mystic);
                break;
            case ClassBase.IdClass.Defender:
                goTrait = OnGetTrait<ClassDefender>(allTraitsBase.defender);
                break;
            case ClassBase.IdClass.Sorcerer:
                goTrait = OnGetTrait<ClassSorcerer>(allTraitsBase.sorcerer);
                break;
            case ClassBase.IdClass.Skirmisher:
                goTrait = OnGetTrait<ClassSkirmisher>(allTraitsBase.skirmisher);
                break;
        }
        if (_dictTraits.ContainsKey(goTrait) && _dictTraits[goTrait].Contains(champion))
        {
            return;
        }
        if (!_dictTraits.ContainsKey(goTrait))
        {
            _dictTraits[goTrait] = new List<GameObject>();
        }
        _dictTraits[goTrait].Add(champion);
        Launch();
        OnChange(goTrait);
    }

    public void AddOrigin(OriginBase.IdOrigin idOrigin, GameObject champion)
    {
        Debug.Log("AddOrigin: " + champion.name);
        GameObject goTrait = null;
        switch (idOrigin)
        {
            case OriginBase.IdOrigin.Mascot:
                goTrait = OnGetTrait<OriginMascot>(allTraitsBase.mascot);
                break;
            case OriginBase.IdOrigin.Hextech:
                goTrait = OnGetTrait<OriginHextech>(allTraitsBase.hextech);
                break;
            case OriginBase.IdOrigin.Yordle:
                goTrait = OnGetTrait<OriginYordle>(allTraitsBase.yordle);
                break;
            case OriginBase.IdOrigin.Nightbringer:
                goTrait = OnGetTrait<OriginNightbringer>(allTraitsBase.nightbringer);
                break;
            case OriginBase.IdOrigin.Dawnbringer:
                goTrait = OnGetTrait<OriginDawnbringer>(allTraitsBase.dawnbringer);
                break;
            case OriginBase.IdOrigin.Duelist:
                goTrait = OnGetTrait<OriginDuelist>(allTraitsBase.duelist);
                break;
        }
        if (_dictTraits.ContainsKey(goTrait) && _dictTraits[goTrait].Contains(champion))
        {
            return;
        }
        if (!_dictTraits.ContainsKey(goTrait))
        {
            _dictTraits[goTrait] = new List<GameObject>();
        }
        _dictTraits[goTrait].Add(champion);
        Launch();
        OnChange(goTrait);
    }

    public void RemoveClass(ClassBase.IdClass idClass, GameObject champion)
    {
        Debug.Log("RemoveClass: " + champion.name);
        GameObject goTrait = null;
        switch (idClass)
        {
            case ClassBase.IdClass.Ranger:
                goTrait = OnGetTrait<ClassRanger>(allTraitsBase.ranger);
                break;
            case ClassBase.IdClass.Assassin:
                goTrait = OnGetTrait<ClassAssassin>(allTraitsBase.assassin);
                break;
            case ClassBase.IdClass.Brawler:
                goTrait = OnGetTrait<ClassBrawler>(allTraitsBase.brawler);
                break;
            case ClassBase.IdClass.Mystic:
                goTrait = OnGetTrait<ClassMystic>(allTraitsBase.mystic);
                break;
            case ClassBase.IdClass.Defender:
                goTrait = OnGetTrait<ClassDefender>(allTraitsBase.defender);
                break;
            case ClassBase.IdClass.Sorcerer:
                goTrait = OnGetTrait<ClassSorcerer>(allTraitsBase.sorcerer);
                break;
            case ClassBase.IdClass.Skirmisher:
                goTrait = OnGetTrait<ClassSkirmisher>(allTraitsBase.skirmisher);
                break;
        }
        if (_dictTraits.ContainsKey(goTrait) && !_dictTraits[goTrait].Contains(champion))
        {
            return;
        }
        ChampionInfo1 chInfo = champion.GetComponent<ChampionInfo1>();
        goTrait.GetComponent<ChTraits>().OnRemove(chInfo);
        foreach (var i in _dictTraits)
        {
            if (i.Key.GetComponent<ChTraits>().trait.buffOn == SkillBase1.BuffOn.Allies)
            {
                i.Key.GetComponent<ChTraits>().OnRemove(chInfo);
            }
        }
        _dictTraits[goTrait].Remove(champion);
        Launch();
        OnChange(goTrait);
    }

    public void RemoveOrigin(OriginBase.IdOrigin idOrigin, GameObject champion)
    {
        Debug.Log("RemoveOrigin: " + champion.name);
        GameObject goTrait = null;
        switch (idOrigin)
        {
            case OriginBase.IdOrigin.Mascot:
                goTrait = OnGetTrait<OriginMascot>(allTraitsBase.mascot);
                break;
            case OriginBase.IdOrigin.Hextech:
                goTrait = OnGetTrait<OriginHextech>(allTraitsBase.hextech);
                break;
            case OriginBase.IdOrigin.Yordle:
                goTrait = OnGetTrait<OriginYordle>(allTraitsBase.yordle);
                break;
            case OriginBase.IdOrigin.Nightbringer:
                goTrait = OnGetTrait<OriginNightbringer>(allTraitsBase.nightbringer);
                break;
            case OriginBase.IdOrigin.Dawnbringer:
                goTrait = OnGetTrait<OriginDawnbringer>(allTraitsBase.dawnbringer);
                break;
            case OriginBase.IdOrigin.Duelist:
                goTrait = OnGetTrait<OriginDuelist>(allTraitsBase.duelist);
                break;
        }
        if (_dictTraits.ContainsKey(goTrait) && !_dictTraits[goTrait].Contains(champion))
        {
            return;
        }
        ChampionInfo1 chInfo = champion.GetComponent<ChampionInfo1>();
        goTrait.GetComponent<ChTraits>().OnRemove(chInfo);
        foreach (var i in _dictTraits)
        {
            if (i.Key.GetComponent<ChTraits>().trait.buffOn == SkillBase1.BuffOn.Allies)
            {
                i.Key.GetComponent<ChTraits>().OnRemove(chInfo);
            }
        }
        _dictTraits[goTrait].Remove(champion);
        Launch();
        OnChange(goTrait);
    }


    //public void OnChange(TraitBase traitBase)
    //{
    //    if (_dictTraitsDisplay.ContainsKey(traitBase))
    //    {
    //        if (_dictTraits[traitBase].Count <= 0)
    //        {
    //            PhotonNetwork.Destroy(_dictTraitsDisplay[traitBase]);
    //            _dictTraitsDisplay.Remove(traitBase);
    //            _dictTraits.Remove(traitBase);
    //        }
    //        else
    //        {
    //            int breakpoint = _dictTraits[traitBase].Select(x => x.GetComponent<ChampionInfo1>().chStat.championName).Distinct().Count();
    //            _dictTraitsDisplay[traitBase].GetComponent<TraitInfoManager>().SetTrait(traitBase, breakpoint);
    //        }
    //    }
    //    else
    //    {
    //        GameObject traitObj = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/traits", prefab_trait.name), prefab_trait.transform.position, prefab_trait.transform.rotation);
    //        switch (traitBase)
    //        {

    //        }
    //        traitObj.AddComponent<component>
    //        traitObj.transform.SetParent(background);
    //        traitObj.transform.localScale = Vector3.one;
    //        int breakpoint = _dictTraits[traitBase].Select(x => x.GetComponent<ChampionInfo1>().chStat.championName).Distinct().Count();
    //        traitObj.GetComponent<TraitInfoManager>().SetTrait(traitBase, breakpoint);
    //        _dictTraitsDisplay.Add(traitBase, traitObj);
    //        if (!traitObj.GetPhotonView().IsMine)
    //        {
    //            traitObj.SetActive(false);
    //        }
    //    }
    //}

    public void Launch()
    {
        foreach (var kvp in _dictTraits)
        {
            //if (i.Key.activeAtTheStartOfCombat)
            //{
                
            //}
            List<GameObject> championsGetBuff = new List<GameObject>();
            switch (kvp.Key.GetComponent<ChTraits>().trait.buffOn)
            {
                case SkillBase1.BuffOn.Self:
                    //i.Key.chClass.Launch(i.Value);
                    championsGetBuff = kvp.Value;
                    break;
                case SkillBase1.BuffOn.Allies:
                    foreach (var j in kvp.Value)
                    {
                        Debug.Log("Launch SkillBase1.BuffOn.Allies: " + j.name);
                        championsGetBuff.Add(j);
                    }
                    //i.Key.chClass.Launch(i.Value, allies);
                    break;
            }
            int numDistinctChampion = kvp.Value.Select(x => x.GetComponent<ChampionInfo1>().chStat.championName).Distinct().Count();
            Debug.Log("kvp.Value.Count - breakpoint: " + numDistinctChampion + " championsGetBuff: " + championsGetBuff.Count);
            kvp.Key.GetComponent<ChTraits>().Launch(numDistinctChampion, championsGetBuff);
        }
    }

    public GameObject OnGetTrait<T>(TraitBase traitBase) where T : ChTraits
    {
        GameObject goTrait = _dictTraits.Keys.FirstOrDefault(x => x.GetComponent<T>());
        if (goTrait != null)
        {
            Debug.Log("_dictTraits.Keys.FirstOrDefault(x => x.GetComponent<T>()) != null");
            return goTrait;
        }
        else
        {
            Debug.Log("_dictTraits.Keys.FirstOrDefault(x => x.GetComponent<T>()) == null");
            GameObject traitObj = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/traits", prefab_trait.name), prefab_trait.transform.position, prefab_trait.transform.rotation);
            traitObj.AddComponent<T>();
            traitObj.GetComponent<T>().trait = traitBase;
            traitObj.transform.SetParent(background);
            traitObj.transform.localScale = Vector3.one;
            if (!traitObj.GetPhotonView().IsMine)
            {
                traitObj.SetActive(false);
            }
            return traitObj;
        }
    }

    public void OnChange(GameObject goTrait)
    {
        if (_dictTraits[goTrait].Count <= 0)
        {
            _dictTraits.Remove(goTrait);
            if (goTrait.GetPhotonView().IsMine)
            {
                PhotonNetwork.Destroy(goTrait.GetComponent<PhotonView>());
            }
            return;
        }
        int breakpoint = _dictTraits[goTrait].Select(x => x.GetComponent<ChampionInfo1>().chStat.championName).Distinct().Count();
        goTrait.GetComponent<TraitInfoManager>().SetTrait(goTrait.GetComponent<ChTraits>().trait, breakpoint);
    }
}

[Serializable]
public class AllTraitsBase
{
    public Ranger ranger;
    public Assassin assassin;
    public Brawler brawler;
    public Mystic mystic;
    public Defender defender;
    public Sorcerer sorcerer;
    public Skirmisher skirmisher;

    public Mascot mascot;
    public Hextech hextech;
    public Yordle yordle;
    public Nightbringer nightbringer;
    public Dawnbringer dawnbringer;
    public Duelist duelist;
}

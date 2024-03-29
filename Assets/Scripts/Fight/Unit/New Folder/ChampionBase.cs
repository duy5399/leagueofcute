using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using ExitGames.Client.Photon.StructWrapping;
using System.Security.Claims;
using static SkillBase1;
using UnityEngine.UIElements;
using System;
using static Animancer.Validate;
using Cinemachine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static Cinemachine.CinemachineTargetGroup;

public class ChampionBase : MonoBehaviourPun, IPunObservable
{
    [SerializeField] protected ChampionInfo1 _info;
    [SerializeField] protected ChampionState _currentState;
    [SerializeField] protected ChampionStateController _stateCtrl;
    [SerializeField] protected AnimManager1 _anim;
    [SerializeField] protected SkillManager1 _skills;
    [SerializeField] protected BuffManager _buffs;
    [SerializeField] protected MoveManager _moveManager;
    [SerializeField] protected HealthbarManager _hpBar;
    [SerializeField] protected Weakness _weakness;
    [SerializeField] protected DragDrop _dragDrop;
    [SerializeField] protected ItemManager _items;

    public ChampionInfo1 info
    {
        get
        {
            if (_info != null)
            {
                return _info;
            }
            if (base.gameObject != null)
            {
                _info = base.gameObject.GetComponent<ChampionInfo1>();
                if (_info == null)
                {
                    ChampionInfo1 component = null;
                    Transform _transform = base.gameObject.transform;
                    int maxDepth = 6;
                    while (_transform != null && maxDepth > 0)
                    {
                        component = _transform.GetComponent<ChampionInfo1>();
                        if (component != null)
                            break;
                        _transform = _transform.parent;
                        maxDepth--;
                    }
                    if (component != null)
                    {
                        _info = component;
                    }
                }
            }
            return _info;
        }
        set 
        { 
            _info = value; 
        }
    }

    public ChampionState currentState
    {
        get
        {
            return(_currentState != null) ? _currentState : ((info == null) ? null : (info == this) ? Get(ref _currentState) : (_currentState = info.currentState));
        }
    }

    public ChampionStateController stateCtrl
    {
        get
        {
            return (_stateCtrl != null) ? _stateCtrl : ((info == null) ? null : (info == this) ? Get(ref _stateCtrl) : (_stateCtrl = info.stateCtrl));
        }
    }

    public AnimManager1 anim
    {
        get
        {
            return (_anim != null) ? _anim : ((info == null) ? null : ((!(info != this)) ? Get(ref _anim) : (_anim = info.anim)));
            //if (_anim != null)
            //{
            //    return _anim;
            //}
            //if (info == null)
            //    return null;
            //else
            //{
            //    if (info == this))
            //        return Get(ref _anim);
            //    else
            //    {
            //        _anim = info.anim;
            //        return _anim;
            //    }
            //}
        }
    }

    public SkillManager1 skills
    {
        get
        {
            return (_skills != null) ? _skills : ((info == null) ? null : (info == this) ? Get(ref _skills, true) : (_skills = info.skills));
        }
    }

    public BuffManager buffs
    {
        get
        {
            return (_buffs != null) ? _buffs : ((info == null) ? null : (info == this) ? Get(ref _buffs, true) : (_buffs = info.buffs));
        }
    }

    public MoveManager moveManager
    {
        get
        {
            return (_moveManager != null) ? _moveManager : ((info == null) ? null : (info == this) ? Get(ref _moveManager, true) : (_moveManager = info.moveManager));
        }
    }

    public HealthbarManager hpBar
    {
        get
        {
            return (_hpBar != null) ? _hpBar : ((info == null) ? null : (info == this) ? Get(ref _hpBar, true) : (_hpBar = info.hpBar));
        }
    }

    public Weakness weakness
    {
        get
        {
            return (_weakness != null) ? _weakness : ((info == null) ? null : (info == this) ? Get(ref _weakness, true) : (_weakness = info.weakness));
        }
    }

    public DragDrop dragDrop
    {
        get
        {
            return (_dragDrop != null) ? _dragDrop : ((info == null) ? null : (info == this) ? Get(ref _dragDrop) : (_dragDrop = info.dragDrop));
        }
    }

    public ItemManager items
    {
        get
        {
            return (_items != null) ? _items : ((info == null) ? null : (info == this) ? Get(ref _items, true) : (_items = info.items));
        }
    }

    protected virtual void Awake()
    {
        _info = Get(ref _info);
        _currentState = Get(ref _currentState);
        _stateCtrl = Get(ref _stateCtrl);
        _anim = Get(ref _anim);
        _skills = Get(ref _skills, true);
        _buffs = Get(ref _buffs, true);
        _moveManager = Get(ref _moveManager, true);
        _weakness = Get(ref _weakness, true);
        _hpBar = Get(ref _hpBar, true);
        _dragDrop = Get(ref _dragDrop);
        _items = Get(ref _items, true);
    }

    private T Get<T>(ref T def, bool inDirectChild = false) where T : Component
    {
        if (def != null)
        {
            return def;
        }
        if (info != null)
        {
            if (!inDirectChild)
            {
                def = info.GetComponent<T>();
            }
            else
            {
                def = info.GetComponentInChildren<T>();
            }
        }
        return def;
    }

    public void WaitFor(float delay, Action func)
    {
        if (func == null)
        {
            return;
        }
        if (delay <= 0.0001f)
        {
            func();
        }
        else
        {
            Coroutine coroutine = StartCoroutine(_WaitFor(delay, func));
        }
    }

    public IEnumerator _WaitFor(float delay, Action func)
    {
        yield return new WaitForSeconds(delay);
        func();
    }

    public void SetName(string name)
    {
        photonView.RPC(nameof(RPC_SetName), RpcTarget.AllBuffered, name);
    }
    public virtual void SetParent(int photonviewParent, string pathParent = null)
    {
        photonView.RPC(nameof(RPC_SetParent), RpcTarget.AllBuffered, photonviewParent, pathParent);
    }
    public void SetPosition(Transform newPosition)
    {
        transform.position = newPosition.position;
    }

    [PunRPC]
    void RPC_SetName(string name)
    {
        base.gameObject.name = name;
    }

    [PunRPC]
    void RPC_SetParent(int viewID, string pathParent)
    {
        Transform _parent = PhotonView.Find(viewID).transform;
        if (pathParent == null)
        {
            transform.parent = _parent;
        }
        else
        {
            Transform _pathParent = _parent.Find(pathParent);
            transform.parent = _pathParent;
        }
    }
    public void SetActive(bool isActive)
    {
        photonView.RPC(nameof(RPC_SetActive), RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    void RPC_SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        PhotonSerializeView(stream, info);
    }

    public virtual void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)       //gửi dữ liệu
        //{
        //    if (this.info)
        //    {
        //        stream.SendNext(this.info.chStat._id);
        //        stream.SendNext(this.info.chStat.championName);
        //        stream.SendNext(this.info.chStat.background);
        //        stream.SendNext(this.info.chStat.currentLevel.star);
        //        stream.SendNext(this.info.chStat.abilityIcon);
        //        stream.SendNext(this.info.chStat.owner);
        //        stream.SendNext(this.info.chStat.currentLevel.star);
        //    }
        //    if (stateCtrl)
        //    {
        //        stream.SendNext(stateCtrl.inCombat);
        //        //stream.SendNext(stateCtrl.silenceTimeLeft);
        //        //stream.SendNext(stateCtrl.blindTimeLeft);
        //        //stream.SendNext(stateCtrl.attackDisableTimeLeft);
        //        //stream.SendNext(stateCtrl.moveDisableTimeLeft);
        //        //stream.SendNext(stateCtrl.slowMoveTimeLeft);
        //        //stream.SendNext(stateCtrl.dodgeTimeLeft);
        //    }
        //    if (currentState)
        //    {
        //        stream.SendNext(currentState.attackDamage);
        //        stream.SendNext(currentState.physicalVamp);
        //        stream.SendNext(currentState.abilityPower);
        //        stream.SendNext(currentState.spellVamp);
        //        stream.SendNext(currentState.armorPenetration);
        //        stream.SendNext(currentState.armorPenetrationPercentage);
        //        stream.SendNext(currentState.magicPenetration);
        //        stream.SendNext(currentState.magicPenetrationPercentage);
        //        stream.SendNext(currentState.criticalStrikeChance);
        //        stream.SendNext(currentState.criticalStrikeDamage);
        //        stream.SendNext(currentState.hp);
        //        stream.SendNext(currentState.maxHP);
        //        stream.SendNext(currentState.mana);
        //        stream.SendNext(currentState.maxMana);
        //        stream.SendNext(currentState.armor);
        //        stream.SendNext(currentState.magicResistance);
        //        stream.SendNext(currentState.shield);
        //        stream.SendNext(currentState.dead);
        //    }
        //    if (moveManager)
        //    {
        //        stream.SendNext(moveManager.positionNode);
        //    }
        //    if (hpBar)
        //    {
        //        stream.SendNext(hpBar._slider_HP.value);
        //        stream.SendNext(hpBar._slider_Mana.value);
        //    }
        //}
        //else if (stream.IsReading)  //nhận dữ liệu
        //{
        //    if (this.info)
        //    {
        //        this.info.chStat._id = (string)stream.ReceiveNext();
        //        this.info.chStat.championName = (string)stream.ReceiveNext();
        //        this.info.chStat.background = (string)stream.ReceiveNext();
        //        this.info.chStat.currentLevel.star = (int)stream.ReceiveNext();
        //        this.info.chStat.abilityIcon = (string)stream.ReceiveNext();
        //        this.info.chStat.owner = (string)stream.ReceiveNext();
        //        this.info.chStat.currentLevel.star = (int)stream.ReceiveNext();
        //    }
        //    if (stateCtrl)
        //    {
        //        stateCtrl.inCombat = (bool)stream.ReceiveNext();
        //        //stateCtrl.silenceTimeLeft = (float)stream.ReceiveNext();
        //        //stateCtrl.blindTimeLeft = (float)stream.ReceiveNext();
        //        //stateCtrl.attackDisableTimeLeft = (float)stream.ReceiveNext();
        //        //stateCtrl.moveDisableTimeLeft = (float)stream.ReceiveNext();
        //        //stateCtrl.slowMoveTimeLeft = (float)stream.ReceiveNext();
        //        //stateCtrl.dodgeTimeLeft = (float)stream.ReceiveNext();
        //    }
        //    if (currentState)
        //    {
        //        currentState.attackDamage = (float)stream.ReceiveNext();
        //        currentState.physicalVamp = (float)stream.ReceiveNext();
        //        currentState.abilityPower = (float)stream.ReceiveNext();
        //        currentState.spellVamp = (float)stream.ReceiveNext();
        //        currentState.armorPenetration = (float)stream.ReceiveNext();
        //        currentState.armorPenetrationPercentage = (float)stream.ReceiveNext();
        //        currentState.magicPenetration = (float)stream.ReceiveNext();
        //        currentState.magicPenetrationPercentage = (float)stream.ReceiveNext();
        //        currentState.criticalStrikeChance = (float)stream.ReceiveNext();
        //        currentState.criticalStrikeDamage = (float)stream.ReceiveNext();
        //        currentState.hp = (float)stream.ReceiveNext();
        //        currentState.maxHP = (float)stream.ReceiveNext();
        //        currentState.mana = (float)stream.ReceiveNext();
        //        currentState.maxMana = (float)stream.ReceiveNext();
        //        currentState.armor = (float)stream.ReceiveNext();
        //        currentState.magicResistance = (float)stream.ReceiveNext();
        //        currentState.shield = (float)stream.ReceiveNext();
        //        currentState.dead = (bool)stream.ReceiveNext();
        //    }
        //    if (moveManager)
        //    {
        //        moveManager.positionNode = (int[])stream.ReceiveNext();
        //    }
        //    if (hpBar)
        //    {
        //        hpBar._slider_HP.value = (float)stream.ReceiveNext();
        //        hpBar._slider_Mana.value = (float)stream.ReceiveNext();
        //    }
        //}
        if (stream.IsWriting)       //gửi dữ liệu
        {
            if (moveManager)
            {
                stream.SendNext(moveManager.positionNode);
            }
        }
        else if (stream.IsReading)  //nhận dữ liệu
        {
            if (moveManager)
            {
                moveManager.positionNode = (int[])stream.ReceiveNext();
            }
        }
    }

    public void SetImageStar(int star)
    {
        photonView.RPC(nameof(RPC_SetImageStar), RpcTarget.AllBuffered, star);
    }

    [PunRPC]
    void RPC_SetImageStar(int star)
    {
        hpBar.SetImageStar1(star);
    }

    public void SetAddItem(int viewID)
    {
        photonView.RPC(nameof(RPC_SetAddItem), RpcTarget.AllBuffered, viewID);
    }

    [PunRPC]
    void RPC_SetAddItem(int viewID)
    {
        GameObject item = PhotonView.Find(viewID).gameObject;
        items.itemLst.Add(item);
    }

    public byte[] ObjectToByteArray<T>(T obj) where T : class
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);

        return ms.ToArray();
    }

    // Convert a byte array to an Object
    public T ByteArrayToObject<T>(byte[] arrBytes) where T : class
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        T obj = (T)binForm.Deserialize(memStream);

        return obj;
    }
}

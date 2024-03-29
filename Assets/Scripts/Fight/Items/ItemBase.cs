using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using UnityEngine;
using static SkillSpawn1;
using System.Linq;
using Photon.Pun;
using static SkillBase1;

public class ItemBase : ChampionBase
{
    [SerializeField] protected Item _item;
    [SerializeField] protected bool _isEquipped;
    [SerializeField] protected SkillBase1 _itemPassive;

    [SerializeField] private bool _isActive;
    [SerializeField] private float _cooldown;

    public Item item
    {
        get { return _item; }
        set { _item = value; }
    }

    public bool isEquipped
    {
        get { return _isEquipped; }
        set { _isEquipped = value; }
    }

    public SkillBase1 itemPassive
    {
        get
        {
            return (_itemPassive != null) ? _itemPassive : GetComponentsInChildren<SkillBase1>().FirstOrDefault(s => s.tag == "ItemPassive");
        }
        set { _itemPassive = value; }
    }

    public bool isActive
    {
        get { return _isActive; }
        set { _isActive = value; }
    }
    public float cooldown
    {
        get { return _cooldown; }
        set { _cooldown = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        _itemPassive = GetComponentsInChildren<SkillBase1>().FirstOrDefault(s => s.tag == "ItemPassive");
    }

    protected virtual void FixedUpdate()
    {

    }

    public void OnEquip()
    {
        Debug.Log("OnEquip");
        if(base.info != null && !_isEquipped)
        {
            foreach(var i in _item.stats)
            {
                float amount = i.typeBuff == StateBuff.TypeBuff.Add ? i.amount : 1f + i.amount;
                switch (i.typeStat)
                {
                    case StatItem.TypeStat.AD:
                        base.currentState._buffOnAttackDamage.Add(new StateBuff(_item, i.typeBuff, amount));
                        break;
                    case StatItem.TypeStat.AP:
                        base.currentState._buffOnAbilityPower.Add(new StateBuff(_item, i.typeBuff, amount));
                        break;
                    case StatItem.TypeStat.AS:
                        base.currentState._buffOnAttackSpeed.Add(new StateBuff(_item, i.typeBuff, amount));
                        break;
                    case StatItem.TypeStat.HP:
                        base.currentState._buffOnMaxHP.Add(new StateBuff(_item, i.typeBuff, amount));
                        base.currentState.hp = base.currentState.maxHP;
                        break;
                    case StatItem.TypeStat.AR:
                        base.currentState._buffOnArmor.Add(new StateBuff(_item, i.typeBuff, amount));
                        break;
                    case StatItem.TypeStat.MR:
                        base.currentState._buffOnMagicResistance.Add(new StateBuff(_item, i.typeBuff, amount));
                        break;
                    case StatItem.TypeStat.MP:
                        base.currentState._buffOnMana.Add(new StateBuff(_item, i.typeBuff, amount));
                        break;
                    case StatItem.TypeStat.CritChance:
                        base.currentState._buffOnCriticalStrikeChance.Add(new StateBuff(_item, i.typeBuff, amount));
                        break;
                    case StatItem.TypeStat.PV:
                        base.currentState._buffOnPhysicalVamp.Add(new StateBuff(_item, i.typeBuff, amount));
                        break;
                    case StatItem.TypeStat.SV:
                        base.currentState._buffOnSpellVamp.Add(new StateBuff(_item, i.typeBuff, amount));
                        break;
                }
            }
            _itemPassive.details = _item.passive != null ? _item.passive : null;
            _isEquipped = true;
            gameObject.GetComponent<PhotonView>().Synchronization = ViewSynchronization.UnreliableOnChange;
        }
    }

    public void OnUnequip()
    {
        if (base.info != null && _isEquipped)
        {
            foreach (var i in _item.stats)
            {
                base.currentState._buffOnAttackDamage.RemoveAll(x => x.item == _item);
                base.currentState._buffOnAbilityPower.RemoveAll(x => x.item == _item);
                base.currentState._buffOnAttackSpeed.RemoveAll(x => x.item == _item);
                base.currentState._buffOnMaxHP.RemoveAll(x => x.item == _item);
                base.currentState._buffOnArmor.RemoveAll(x => x.item == _item);
                base.currentState._buffOnMagicResistance.RemoveAll(x => x.item == _item);
                base.currentState._buffOnMana.RemoveAll(x => x.item == _item);
                base.currentState._buffOnCriticalStrikeChance.RemoveAll(x => x.item == _item);
                base.currentState._buffOnPhysicalVamp.RemoveAll(x => x.item == _item);
                base.currentState._buffOnSpellVamp.RemoveAll(x => x.item == _item);

                base.currentState.hp = base.currentState.maxHP;
            }
            _isEquipped = false;
        }
    }

    public virtual void OnReset()
    {
        foreach (var i in _item.stats)
        {
            if (i.typeStat == StatItem.TypeStat.MP)
            {
                if(base.info.currentState._buffOnMana.FirstOrDefault(x => x.item == item && x.amount == i.amount) == null)
                {
                    base.info.currentState._buffOnMana.Add(new StateBuff(_item, i.typeBuff, i.amount));
                }
            }
        }
    }

    public virtual void OnBasicAttack(SkillBase1 skillBase, Transform target)
    {
    }

    public virtual void OnSpecialAbility(SkillBase1 skillBase, Transform target)
    {
        if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
        {
            return;
        }
        foreach (var i in _item.stats)
        {
            if (i.typeStat == StatItem.TypeStat.MP)
            {
                base.currentState._buffOnMana.RemoveAll(x => x.item == _item && x.amount == i.amount);
            }
        }
    }

    public virtual void OnHit(Transform target, float damage, bool isCritical)
    {
    }

    public virtual void OnBeHited(Transform caster, float damage, bool isCritical)
    {
    }

    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)       //gửi dữ liệu
        {
            //item
            stream.SendNext(item._id);
            stream.SendNext(item.idItem);
            stream.SendNext(item.name);
            stream.SendNext(item.icon);
            stream.SendNext(item.descriptionStat);
            stream.SendNext(item.descriptionPassive);
            stream.SendNext(item.typeItem);
            stream.SendNext(item.slotRequired);
            stream.SendNext(item.isUnique);
            //passive

            //
            stream.SendNext(isEquipped);
            stream.SendNext(isActive);
            stream.SendNext(cooldown);
        }
        else if (stream.IsReading)  //nhận dữ liệu
        {
            //item
            item._id = (string)stream.ReceiveNext();
            item.idItem = (string)stream.ReceiveNext();
            item.name = (string)stream.ReceiveNext();
            item.icon = (string)stream.ReceiveNext();
            item.descriptionStat = (string)stream.ReceiveNext();
            item.descriptionPassive = (string)stream.ReceiveNext();
            item.typeItem = (Item.TypeItem)(int)stream.ReceiveNext();
            item.slotRequired = (int)stream.ReceiveNext();
            item.isUnique = (bool)stream.ReceiveNext();
            //

            //
            isEquipped = (bool)stream.ReceiveNext();
            isActive = (bool)stream.ReceiveNext();
            cooldown = (float)stream.ReceiveNext();
        }
    }
}

[Serializable]
public class Item
{
    public enum TypeItem
    {
        BasicItem = 0,
        AdvancedItem = 1,
        RadiantItem = 2
    }

    public string _id;
    public string idItem;
    public string name;
    public string icon;
    public StatItem[] stats;
    public string descriptionStat;
    public string descriptionPassive;
    [JsonConverter(typeof(StringEnumConverter))]
    public TypeItem typeItem;
    public int slotRequired;
    public bool isUnique;
    public Recipe recipe;
    public SkillBase1.Details passive;
}

[Serializable]
public class StatItem
{
    public enum TypeStat
    {
        AD = 0,
        AP = 1,
        AS = 2,
        HP = 3,
        AR = 4,
        MR = 5,
        MP = 6,
        CritChance = 7,
        PV = 8,
        SV = 9
    }

    public TypeStat typeStat;
    public float amount;
    public StateBuff.TypeBuff typeBuff;
}

[Serializable]
public class Recipe
{
    public string bf_sword = string.Empty;
    public string recurve_bow = string.Empty;
    public string chain_vest = string.Empty;
    public string negatron_cloak = string.Empty;
    public string needlessly_large_rod = string.Empty;
    public string tear_of_the_goddess = string.Empty;
    public string giants_belt = string.Empty;
    public string sparring_gloves = string.Empty;
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionState : ChampionBase
{
    [SerializeField] private const float MANA_RESTORE_ON_HIT = 10;
    [SerializeField] private float _hp = 1f;
    [SerializeField] private float _maxHP = 1f;
    [SerializeField] private float _mana = 0f;
    [SerializeField] private float _maxMana = 1f;
    [SerializeField] private float _moveSpeed = 0f;
    [SerializeField] private float _slowMoveSpeed = 0;
    [SerializeField] private float _criticalStrikeChance = 0.25f;
    [SerializeField] private float _criticalStrikeDamage = 1.25f;
    [SerializeField] private float _attackDamage = 1f;
    [SerializeField] private float _attackSpeed = 0.1f;
    [SerializeField] private int _attackRange = 0;
    [SerializeField] private float _armorPenetration = 0;
    [SerializeField] private float _armorPenetrationPercentage = 0;
    [SerializeField] private float _abilityPower = 0;
    [SerializeField] private float _magicPenetration = 0;
    [SerializeField] private float _magicPenetrationPercentage = 0;
    [SerializeField] private float _armor = 0;
    [SerializeField] private float _magicResistance = 0;
    [SerializeField] private float _hpRegen = 0;
    [SerializeField] private float _manaRegen = 0;
    [SerializeField] private float _shield = 0;
    [SerializeField] private float _magicShield = 0;
    [SerializeField] private float _physicalVamp = 0;
    [SerializeField] private float _spellVamp = 0;

    [SerializeField] private bool _dead;

    [SerializeField] private HeroProperty traitsProperty = new HeroProperty();
    [SerializeField] private HeroProperty equipProperty = new HeroProperty();

    [SerializeField] private List<StatItem> equipProperty1 = new List<StatItem>();

    [SerializeField] private List<HeroProperty> traitsBuff = new List<HeroProperty>();

    [SerializeField] private int _maxItem = 3;

    public List<StateBuff> _buffOnManaRestoreOnHit = new List<StateBuff>();
    public List<StateBuff> _buffOnHP = new List<StateBuff>();
    public List<StateBuff> _buffOnMaxHP = new List<StateBuff>();
    public List<StateBuff> _buffOnMana = new List<StateBuff>();
    public List<StateBuff> _buffOnMaxMana = new List<StateBuff>();
    public List<StateBuff> _buffOnMoveSpeed = new List<StateBuff>();
    public List<StateBuff> _buffOnCriticalStrikeChance = new List<StateBuff>();
    public List<StateBuff> _buffOnCriticalStrikeDamage = new List<StateBuff>();    
    public List<StateBuff> _buffOnAttackDamage = new List<StateBuff>();
    public List<StateBuff> _buffOnAttackSpeed = new List<StateBuff>();
    public List<StateBuff> _buffOnArmorPenetration = new List<StateBuff>();
    public List<StateBuff> _buffOnArmorPenetrationPercentage = new List<StateBuff>();
    public List<StateBuff> _buffOnAbilityPower = new List<StateBuff>();
    public List<StateBuff> _buffOnMagicPenetration = new List<StateBuff>();
    public List<StateBuff> _buffOnMagicPenetrationPercentage = new List<StateBuff>();
    public List<StateBuff> _buffOnArmor = new List<StateBuff>();
    public List<StateBuff> _buffOnMagicResistance = new List<StateBuff>();
    public List<StateBuff> _buffOnHpRegen = new List<StateBuff>();
    public List<StateBuff> _buffOnManaRegen = new List<StateBuff>();
    public List<StateBuff> _buffOnShield = new List<StateBuff>();
    public List<StateBuff> _buffOnMagicShield = new List<StateBuff>();
    public List<StateBuff> _buffOnPhysicalVamp = new List<StateBuff>();
    public List<StateBuff> _buffOnSpellVamp = new List<StateBuff>();

    public bool unselectable;
    public bool dead
    {
        get { return _dead; }
        set { _dead = value; }  
    }

    public float hp
    {
        get
        {
            return mfEx(_hp, _buffOnHP);
        }
        set
        {
            _hp = value;
        }
    }

    public float maxHP
    {
        get
        {
            return mfEx(base.info.chStat.currentLevel.hp + traitsProperty.hp + equipProperty.hp, _buffOnMaxHP);
        }
        set
        {
            _maxHP = value;
        }
    }

    public float mana
    {
        get
        {
            return mfEx(_mana + traitsProperty.mana + equipProperty.mana, _buffOnMana);
        }
        set
        {
            _mana = value;
        }
    }

    public float maxMana
    {
        get
        {
            return mfEx(base.info.chStat.maxMana, _buffOnMaxMana);
        }
        set
        {
            _maxMana = value;
        }
    }

    public float manaRestoreOnHit
    {
        get
        {
            return mfEx(MANA_RESTORE_ON_HIT , _buffOnManaRestoreOnHit);
        }
    }

    public float moveSpeed
    {
        get
        {
            return base.info.chStat.moveSpeed;
        }
        set
        {
            _moveSpeed = value;
        }
    }

    public float criticalStrikeChance
    {
        get
        {
            return mfEx(base.info.chStat.criticalStrikeChance + traitsProperty.criticalStrikeChance + equipProperty.criticalStrikeChance, _buffOnCriticalStrikeChance);
        }
        set
        {
            _criticalStrikeChance = value;
        }
    }

    public float criticalStrikeDamage
    {
        get
        {
            return mfEx(base.info.chStat.criticalStrikeDamage + traitsProperty.criticalStrikeDamage + equipProperty.criticalStrikeDamage, _buffOnCriticalStrikeDamage);
        }
        set
        {
            _criticalStrikeDamage = value;
        }
    }

    public float attackDamage
    {
        get
        {
            return mfEx(base.info.chStat.currentLevel.attackDamage + traitsProperty.attackDamage + equipProperty.attackDamage, _buffOnAttackDamage);
        }
        set
        {
            _attackDamage = value;
        }
    }

    public float attackSpeed
    {
        get
        {
            float num = mfEx(base.info.chStat.attackSpeed + traitsProperty.attackSpeed + equipProperty.attackSpeed, _buffOnAttackSpeed);
            return (num > 5f) ? 5f : num;
        }
        set
        {
            _attackSpeed = value;
        }
    }
    public int attackRange
    {
        get
        {
            return base.info.chStat.attackRange;
        }
        set
        {
            _attackRange = value;
        }
    }

    public float armorPenetration
    {
        get
        {
            return mfEx(base.info.chStat.armorPenetration + traitsProperty.armorPenetration + equipProperty.armorPenetration, _buffOnArmorPenetration);
        }
        set
        {
            _armorPenetration = value;
        }
    }

    public float armorPenetrationPercentage
    {
        get
        {
            return mfEx(base.info.chStat.armorPenetrationPercentage + traitsProperty.armorPenetrationPercentage + equipProperty.armorPenetrationPercentage, _buffOnArmorPenetrationPercentage);
        }
        set
        {
            _armorPenetrationPercentage = value;
        }
    }

    public float abilityPower
    {
        get
        {
            return mfEx(base.info.chStat.abilityPower + traitsProperty.abilityPower + equipProperty.abilityPower, _buffOnAbilityPower);
        }
        set
        {
            _abilityPower = value;
        }
    }

    public float magicPenetration
    {
        get
        {
            return mfEx(base.info.chStat.magicPenetration + traitsProperty.magicPenetration + equipProperty.magicPenetration, _buffOnMagicPenetration);
        }
        set
        {
            _magicPenetration = value;
        }
    }

    public float magicPenetrationPercentage
    {
        get
        {
            return mfEx(base.info.chStat.magicPenetrationPercentage + traitsProperty.magicPenetrationPercentage + equipProperty.magicPenetrationPercentage, _buffOnMagicPenetrationPercentage);
        }
        set
        {
            _magicPenetrationPercentage = value;
        }
    }

    public float armor
    {
        get
        {
            float num = mfEx(base.info.chStat.armor + traitsProperty.armor + equipProperty.armor, _buffOnArmor);
            return (!(num > 0f)) ? 0f : num;
        }
        set
        {
            _armor = value;
        }
    }

    public float magicResistance
    {
        get
        {
            float num = mfEx(base.info.chStat.magicResistance + traitsProperty.magicResistance + equipProperty.magicResistance, _buffOnMagicResistance);
            return (!(num > 0f)) ? 0f : num;
        }
        set
        {
            _magicResistance = value;
        }
    }

    public float hpRegen
    {
        get
        {
            return mfEx(_hpRegen + traitsProperty.hpRegen + equipProperty.hpRegen, _buffOnHpRegen);
        }
        set
        {
            _hpRegen = value;
        }
    }

    public float manaRegen
    {
        get
        {
            return mfEx(_manaRegen + traitsProperty.manaRegen + equipProperty.manaRegen, _buffOnManaRegen);
        }
        set
        {
            _manaRegen = value;
        }
    }

    public float shield
    {
        get
        {
            return mfEx(_shield, _buffOnShield);
        }
        set
        {
            _shield = value;
        }
    }

    public float magicShield
    {
        get
        {
            return mfEx(_magicShield, _buffOnMagicShield);
        }
        set
        {
            _magicShield = value;
        }
    }

    public float physicalVamp
    {
        get
        {
            return mfEx(base.info.chStat.physicalVamp + traitsProperty.physicalVamp + equipProperty.physicalVamp, _buffOnPhysicalVamp);
        }
        set
        {
            _physicalVamp = value;
        }
    }

    public float spellVamp
    {
        get
        {
            return mfEx(base.info.chStat.spellVamp + traitsProperty.spellVamp + equipProperty.spellVamp, _buffOnSpellVamp);
        }
        set
        {
            _spellVamp = value;
        }
    }
    //
    public bool silenced
    {
        get
        {
            return (double)base.stateCtrl.silenceTimeLeft > 0.0001;
        }
    }

    public bool attackDisable
    {
        get
        {
            return (double)base.stateCtrl.attackDisableTimeLeft > 0.0001;
        }
    }

    public bool moveDisable
    {
        get
        {
            return (double)base.stateCtrl.moveDisableTimeLeft > 0.0001;
        }
    }

    public bool blind
    {
        get
        {
            return (double)base.stateCtrl.blindTimeLeft > 0.0001;
        }
    }

    public bool slowMove
    {
        get
        {
            return (double)base.stateCtrl.slowMoveTimeLeft > 0.0001;
        }
    }

    public float hpPercentage
    {
        get
        {
            return (!(maxHP <= 0f)) ? (hp / maxHP) : 0f;
        }
    }

    public float manaPercentage
    {
        get
        {
            return (!(maxMana <= 0f)) ? (mana / maxMana) : 0f;
        }
    }

    public int maxItem
    {
        get
        {
            return _maxItem;
        }
        set
        {
            _maxItem = value;
        }
    }

    private void OnEnable()
    {
        InitState();
    }

    private void Update()
    {
        if (dead && !unselectable)
        {
            unselectable = true;
        }
    }

    public float mfEx(float x, List<StateBuff> buffLst)
    {
        float num = 0f;
        float num2 = 1f;
        for (int i = 0; i < buffLst.Count; i++)
        {
            switch (buffLst[i].typeBuff)
            {
                case StateBuff.TypeBuff.Add:
                    num += buffLst[i].amount;
                    break;
                case StateBuff.TypeBuff.Mult:
                    num2 *= buffLst[i].amount;
                    break;
            }
        }
        return (x + num) * num2;
    }

    public void InitState()
    {
        //_hp = base.info.chStat.currentLevel.hp;
        //_maxHP = base.info.chStat.currentLevel.hp;
        _maxHP = maxHP;
        _hp = maxHP;
        _mana = base.info.chStat.startMana;
        _maxMana = maxMana;
        _moveSpeed = moveSpeed;
        _slowMoveSpeed = 0;
        _attackRange = attackRange;
        _criticalStrikeChance = criticalStrikeChance;
        _criticalStrikeDamage = criticalStrikeDamage;
        _attackDamage = attackDamage;
        _attackSpeed = attackSpeed;
        _armorPenetration = armorPenetration;
        _armorPenetrationPercentage = armorPenetrationPercentage;
        _abilityPower = abilityPower;
        _magicPenetration = magicPenetration;
        _magicPenetrationPercentage = magicPenetrationPercentage;
        _armor = armor;
        _magicResistance = magicResistance;
        _hpRegen = hpRegen;
        _manaRegen = manaRegen;
        _shield = 0;
        _magicShield = 0;
        _physicalVamp = physicalVamp;
        _spellVamp = spellVamp;
        //growthProperty = base.info.initGrowthProperty;
        //equipProperty = base.info.equipProperty;
        //runeProperty = base.info.runeProperty;
        dead = false;
        Debug.Log("InitState: " + base.info.chStat.championName+ ": " + _attackRange + " - " +base.info.chStat.attackRange);
    }

    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)       //gửi dữ liệu
        {
            stream.SendNext(attackDamage);
            stream.SendNext(physicalVamp);
            stream.SendNext(abilityPower);
            stream.SendNext(spellVamp);
            stream.SendNext(armorPenetration);
            stream.SendNext(armorPenetrationPercentage);
            stream.SendNext(magicPenetration);
            stream.SendNext(magicPenetrationPercentage);
            stream.SendNext(criticalStrikeChance);
            stream.SendNext(criticalStrikeDamage);
            stream.SendNext(hp);
            stream.SendNext(maxHP);
            stream.SendNext(mana);
            stream.SendNext(maxMana);
            stream.SendNext(armor);
            stream.SendNext(magicResistance);
            stream.SendNext(shield);
            stream.SendNext(dead);
        }
        else if (stream.IsReading)  //nhận dữ liệu
        {
            attackDamage = (float)stream.ReceiveNext();
            physicalVamp = (float)stream.ReceiveNext();
            abilityPower = (float)stream.ReceiveNext();
            spellVamp = (float)stream.ReceiveNext();
            armorPenetration = (float)stream.ReceiveNext();
            armorPenetrationPercentage = (float)stream.ReceiveNext();
            magicPenetration = (float)stream.ReceiveNext();
            magicPenetrationPercentage = (float)stream.ReceiveNext();
            criticalStrikeChance = (float)stream.ReceiveNext();
            criticalStrikeDamage = (float)stream.ReceiveNext();
            hp = (float)stream.ReceiveNext();
            maxHP = (float)stream.ReceiveNext();
            mana = (float)stream.ReceiveNext();
            maxMana = (float)stream.ReceiveNext();
            armor = (float)stream.ReceiveNext();
            magicResistance = (float)stream.ReceiveNext();
            shield = (float)stream.ReceiveNext();
            dead = (bool)stream.ReceiveNext();
        }
    }
}

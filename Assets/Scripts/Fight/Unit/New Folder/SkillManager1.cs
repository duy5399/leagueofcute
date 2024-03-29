using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static SkillBase1;

public class SkillManager1 : ChampionBase
{
    [SerializeField] private GameObject _target;
    [SerializeField] private SkillBase1 _basicAttack;
    [SerializeField] private SkillBase1 _specialAbility;
    [SerializeField] private ObjectPool _skillObjPool;
    [SerializeField] private SkillBase1 _currentCasting;

    public GameObject target
    {
        get { return _target; }
        set { _target = value; }
    }

    public SkillBase1 basicAttack
    {
        get { return (_basicAttack != null) ? _basicAttack : GetComponentsInChildren<SkillBase1>().FirstOrDefault(s => s.tag == "BasicAttack"); }
        set { _basicAttack = value; }
    }

    public SkillBase1 specialAbility
    {
        get { return (_specialAbility != null) ? _specialAbility : GetComponentsInChildren<SkillBase1>().FirstOrDefault(s => s.tag == "Ability"); }
        set { _specialAbility = value; }
    }

    public ObjectPool skillObjPool
    {
        get { return (_skillObjPool != null) ? _skillObjPool : GetComponentInChildren<ObjectPool>(); }
        set { _skillObjPool = value; }
    }

    public SkillBase1 currentCasting
    {
        get { return _currentCasting;  }
        set { _currentCasting = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        basicAttack = GetComponentsInChildren<SkillBase1>().FirstOrDefault(s => s.tag == "BasicAttack");
        specialAbility = GetComponentsInChildren<SkillBase1>().FirstOrDefault(s => s.tag == "Ability");
        skillObjPool = GetComponentInChildren<ObjectPool>();
    }

    private void Start()
    {
        if(_basicAttack != null && base.info.chStat.basicAttack != null)
        {
            basicAttack.details = base.info.chStat.basicAttack;
        }
        if(_specialAbility != null && base.info.chStat.ability != null)
        {
            specialAbility.details = base.info.chStat.ability;
        }
    }

    private void Update()
    {
        if (currentCasting != null || target == null || !basicAttack.IsInRange(target))
        {
            return;
        }
        if(target.GetComponent<ChampionBase>().currentState.hp <= 0 || target.GetComponent<ChampionBase>().currentState.dead)
        {
            target = null;
            base.moveManager.SetNearestTarget(null);
            return;
        }
        //Debug.Log("SkillManager: " + base.info.currentState.mana + " - " + base.info.currentState.maxMana);
        if (IsSkillAvailable(specialAbility) && base.info.currentState.mana >= base.info.currentState.maxMana)
        {
            specialAbility.TriggerSkill(target.transform);
            //base.info.stateCtrl.TriggerManaDelta(-base.currentState.maxMana);
            //Debug.Log(base.info.name + "specialAbility");
        }
        else if (IsSkillAvailable(basicAttack))
        {
            basicAttack.TriggerSkill(target.transform);
            Debug.Log(base.info.name + "basicAttack");
        }
    }

    public bool IsSkillAvailable(SkillBase1 skill)
    {
        if (skill == null || skill.isActive == true || (skill.details.canBeSilenced == true && base.currentState.silenced) || base.currentState.attackDisable || (!skill.details.canMoveWhenCast && base.moveManager.moveState != MoveManager.State.StandStill))
            return false;
        return true;
    }

    public void ForceInterrupt()
    {
        if (currentCasting != null)
        {
            Debug.Log(base.info.name + " ForceInterrupt");
            currentCasting.Interrupt(true);
        }
    }
}

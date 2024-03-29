using BestHTTP;
using DG.DemiLib;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Animancer;
using Photon.Pun;
using static SkillBase1;
using Unity.VisualScripting;
using static Animancer.Easing;

public class SkillBase1 : ChampionBase
{
    public enum Condition
    {
        BasicAttack = 0,
        Active = 1,
        Passive = 2
    }

    public enum CastStatus
    {
        None = 0,
        Casting = 1,
        End = 2
    }

    public enum SearchAmong
    {
        None = 0,
        Allies = 1,
        Enemies = 2,
        All = 3
    }

    public enum TargetFilter
    {
        None = 0,
        LowestHP = 1,
        HighestHP = 2,
        Nearest = 3,
        Farthest = 4
    }

    [SerializeField, Description("Âm thanh có thể bị gián đoạn không?")]
    private bool audioCouldInterrupt = true;
    public Transform target;
    public CastStatus nowState;
    public float nowStateLeftTime;
    public List<AudioClip> _audioLst;
    public SkillManager1 _manager;
    public List<SkillSpawn1> _skillSpawners;
    public float _animTime;

    public bool isActive;
    public bool isTriggered;


    public SkillManager1 manager
    {
        get
        {
            if (!_manager)
            {
                _manager = base.transform.parent.GetComponent<SkillManager1>();
            }
            return _manager;
        }
    }

    public List<SkillSpawn1> skillSpawners
    {
        get
        {
            if (_skillSpawners == null)
            {
                _skillSpawners = GetComponents<SkillSpawn1>().ToList();
            }
            return _skillSpawners;
        }
        set
        {
            _skillSpawners = value;
        }
    }

    public string animName
    {
        get
        {
            try
            {
                switch (base.gameObject.name)
                {
                    case "A":
                        return "attack01";
                    case "Q":
                        switch (base.info.chStat.championName)
                        {
                            case "Teemo":
                                return "attack02";
                        }
                        return "q";
                    case "W":
                        switch (base.info.chStat.championName)
                        {
                            case "Ryze":
                                return "skill0204";
                        }
                        return "w";
                    case "E":
                        return "e";
                    case "R":
                        switch (base.info.chStat.championName)
                        {
                            case "Ashe":
                                return "skill04";
                            case "Miss Fortune":
                                return "r_1";
                        }
                        return "r";
                }
            }
            catch
            {
                Debug.Log("Cant get anim SkillBase");
            }
            return null;
        }
    }

    public float animSpeed
    {
        get
        {
            return (details.isScaleAnimWithAS == true) ? base.currentState.attackSpeed : 1f;
        }
    }

    public float animTime
    {
        get
        {
            if(base.info.chStat.championName == "Janna" && animName == "r")
            {
                return 3f;
            }
            else if (base.info.chStat.championName == "Katarina" && animName == "r")
            {
                return 2.5f;
            }
            else if (base.info.chStat.championName == "Tristana" && animName == "q")
            {
                return 0.001f;
            }
            else if (base.info.chStat.championName == "Miss Fortune" && animName == "r_1")
            {
                return 2f;
            }
            AnimancerState state;
            base.anim.animancer.States.TryGet(animName, out state);
            //Debug.Log("animTime: " + ((state != null) ? state.Length / animSpeed : 1f));
            return (state != null) ? state.Length / animSpeed : 1f;
        }
    }

    public float delayTrigger
    {
        get
        {
            //Debug.Log("delayTrigger: " + (details.delayTrigger / animSpeed));
            return details.delayTrigger / animSpeed;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _skillSpawners = GetComponents<SkillSpawn1>().ToList();
    }

    public void FixedUpdate()
    {
        if (isActive)
        {
            if (nowStateLeftTime > 0f)
            {
                nowStateLeftTime -= Time.fixedDeltaTime;
                if(animTime - nowStateLeftTime >= delayTrigger && !isTriggered)
                {
                    isTriggered = true;
                    TriggerSpawn(target);
                }
            }
            else
            {
                ChangeCastStatus();
            }
        }
    }

    public bool IsInRange(GameObject target)
    {
        if (base.moveManager != null && base.moveManager.moveState == MoveManager.State.StandStill)
        {
            if (!details.needTarget)
            {
                return true;
            }
            else
            {
                if (target != null)
                {
                    ChampionBase component = target.GetComponent<ChampionBase>();
                    int[] node = new int[2];
                    if (base.info.chStat.owner != "PvE" && component.info.chStat.owner != "PvE")
                    {
                        node[0] = 5 - component.moveManager.positionNode[0];
                        node[1] = 5 - component.moveManager.positionNode[1];
                    }
                    else
                    {
                        node = component.moveManager.positionNode;
                    }
                    if (base.moveManager.GetDistance(base.moveManager.positionNode, node) <= base.info.currentState.attackRange)
                    {
                        //Debug.Log("skillBase GetDistance: " + node[0] + "_" + node[1]);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void Interrupt(bool isForce = false)
    {
        if (isForce)
        {
            if (isActive)
            {
                OnCastEnd(true);
            }
        }
        else
        {
            if (!details.couldInterrupt)
            {
                return;
            }
            if (isActive)
            {
                OnCastEnd(true);
            }
        }
    }

    private void ChangeCastStatus()
    {
        switch (nowState)
        {
            case CastStatus.None:
                nowState = CastStatus.Casting;
                nowStateLeftTime = animTime;
                OnCasting();
                break;
            case CastStatus.Casting:
                nowState = CastStatus.End;
                OnCastEnd();
                break;
        }
    }

    private void OnCasting()
    {
        if (details.isTriggerChAnim)
        {
            base.anim.TriggerAnim(animName, animSpeed, true);
            Debug.Log("OnCasting animname: " + animName);
        }
        //TriggerSpawn(target);
        if (base.info.chCategory != ChampionInfo1.Categories.None && target != null)
        {
            base.info.transform.LookAt(target);
        }
        if (base.buffs)
        {
            if (details.condition == Condition.BasicAttack)
            {
                base.buffs.OnBasicAttack(this, target);
            }
            else
            {
                base.buffs.OnSpecialAbility(this, target);
            }
        }
    }

    private void OnCastEnd(bool isInterrupt = false)
    {
        if (audioCouldInterrupt)
        {
            //audioLst.Foreach(delegate (GameObject x)
            //{
            //    x.enabled = false;
            //});
        }
        if (base.moveManager != null)
        {
            base.moveManager.locked = false;
        }
        if (base.anim != null)
        {
            if (isInterrupt && details.couldInterrupt)
            {
                base.anim.TriggerForceIdle();
            }
            else
            {
                base.anim.TriggerIdle();
            }
        }
        isActive = false;
        nowState = CastStatus.None;
        nowStateLeftTime = 0;
        manager.currentCasting = null;
    }
    public void TriggerSkill(Transform target)
    {
        isTriggered = false;
        nowState = CastStatus.None;
        nowStateLeftTime = 0f;
        manager.currentCasting = this;
        if (details.targetFilter != 0)
        {
            this.target = FilterTarget(target);
        }
        else
        {
            this.target = target;
        }
        if (base.info.chCategory != ChampionInfo1.Categories.None && target != null)
        {
            base.info.transform.LookAt(target);
        }
        if (base.moveManager && !details.canMoveWhenCast)
        {
            base.moveManager.moveState = MoveManager.State.StandStill;
            base.moveManager.StandStill();
            base.moveManager.locked = true;
        }
        if (details.condition == Condition.Active)
        {
            base.info.stateCtrl.TriggerManaDelta(-base.info.currentState.maxMana);
        }
        else
        {
            base.info.stateCtrl.TriggerManaDelta(10);
        }
        //base.buffs.OnBeforeTriggerSkill(this);
        if (base.items)
        {
            if (details.condition == Condition.BasicAttack)
            {
                base.info.items.OnBasicAttack(this, target);
            }
            else
            {
                base.info.items.OnSpecialAbility(this, target);
            }
        }
        isActive = true;
    }
    public void TriggerSpawn(Transform target)
    {
        //Debug.Log("TriggerSpawn: " + this.transform.name + target.name);
        SkillSpawn1.CurrentCasterStatus currentCasterStatus = new SkillSpawn1.CurrentCasterStatus();
        SaveCurrentStatus(currentCasterStatus);
        skillSpawners.ForEach((x) =>
        {
            x.currentCasterStatus = currentCasterStatus;
            x.skill = this;
            x.Spawn(target);
        });
    }

    public void SaveCurrentStatus(SkillSpawn1.CurrentCasterStatus currCasterStatus)
    {
        currCasterStatus.casterViewID = base.info.photonView.ViewID;
        currCasterStatus.owner = base.info.name;
        currCasterStatus.info = base.info;
        currCasterStatus.skill = this;
        currCasterStatus.attackDamage = base.info.currentState.attackDamage;
        currCasterStatus.physicalVamp = base.info.currentState.physicalVamp;
        currCasterStatus.abilityPower = base.info.currentState.abilityPower;
        currCasterStatus.spellVamp = base.info.currentState.spellVamp;
        currCasterStatus.armorPenetration = base.info.currentState.armorPenetration;
        currCasterStatus.armorPenetrationPercentage = base.info.currentState.armorPenetrationPercentage;
        currCasterStatus.magicPenetration = base.info.currentState.magicPenetration;
        currCasterStatus.magicPenetrationPercentage = base.info.currentState.magicPenetrationPercentage;
        currCasterStatus.criticalStrikeChance = base.info.currentState.criticalStrikeChance;
        currCasterStatus.criticalStrikeDamage = base.info.currentState.criticalStrikeDamage;
    }

    public Transform FilterTarget(Transform target)
    {
        Debug.Log("FilterTarget: " + details.targetFilter);
        List<Collider> hitColliders = Physics.OverlapSphere(base.transform.position, details.skillRange).ToList().FindAll(x => TargetAvailable(x.GetComponent<ChampionInfo1>()));
        switch (details.targetFilter)
        {
            case TargetFilter.LowestHP:
                hitColliders.Sort((a, b) => a.GetComponent<ChampionInfo1>().currentState.hp.CompareTo(b.GetComponent<ChampionInfo1>().currentState.hp));
                //hitColliders.OrderBy(x => x.GetComponent<ChampionInfo1>().currentState.hp).ToList();
                break;
            case TargetFilter.HighestHP:
                hitColliders.Sort((a, b) => b.GetComponent<ChampionInfo1>().currentState.hp.CompareTo(a.GetComponent<ChampionInfo1>().currentState.hp));
                break;
            case TargetFilter.Nearest:
                hitColliders.Sort((a, b) =>
                {
                    int distanceA = moveManager.GetDistance(base.moveManager.positionNode, a.GetComponent<ChampionInfo1>().moveManager.positionNode);
                    int distanceB = moveManager.GetDistance(base.moveManager.positionNode, b.GetComponent<ChampionInfo1>().moveManager.positionNode);
                    return distanceA - distanceB;
                });
                break;
            case TargetFilter.Farthest:
                hitColliders.Sort((a, b) =>
                {
                    int distanceA = moveManager.GetDistance(base.moveManager.positionNode, a.GetComponent<ChampionInfo1>().moveManager.positionNode);
                    int distanceB = moveManager.GetDistance(base.moveManager.positionNode, b.GetComponent<ChampionInfo1>().moveManager.positionNode);
                    return distanceB - distanceA;
                });
                break;
        }
        if(hitColliders != null)
        {
            return hitColliders[0].transform;
        }
        return target;
    }

    public bool TargetAvailable(ChampionInfo1 chInfo)
    {
        if (chInfo == null || !chInfo.stateCtrl.inCombat || chInfo.currentState.hp <= 0 || chInfo.currentState.dead || (details.searchAmong == SearchAmong.Allies && chInfo.chStat.owner != base.info.chStat.owner) || (details.searchAmong == SearchAmong.Enemies && chInfo.chStat.owner == base.info.chStat.owner) || chInfo.gameObject.tag != "Fightable")
        {
            //if(chInfo == null)
            //{
            //    Debug.Log("TargetAvailable: chInfo == null " + chInfo.name);
            //}
            //if (!chInfo.stateCtrl.inCombat)
            //{
            //    Debug.Log("TargetAvailable:!chInfo.stateCtrl.inCombat " + chInfo.name);
            //}
            //if (details.searchAmong == SearchAmong.Allies && chInfo.chStat.owner != base.info.chStat.owner)
            //{
            //    Debug.Log("TargetAvailable: details.searchAmong == SearchAmong.Allies && chInfo.chStat.owner != base.info.chStat.owner " + chInfo.name);
            //}
            //if (details.searchAmong == SearchAmong.Enemies && base.info.chStat.owner != "PvE" && chInfo.chStat.owner != SocketIO.instance._stageBattleSocketIO.opponent)
            //{
            //    Debug.Log("TargetAvailable: details.searchAmong == SearchAmong.Enemies && base.info.chStat.owner != \"PvE\" && chInfo.chStat.owner != SocketIO.instance._stageBattleSocketIO.opponent " + chInfo.name);
            //}
            //if (chInfo.gameObject.tag != "Fightable")
            //{
            //    Debug.Log("TargetAvailable: chInfo.gameObject.tag != \"Fightable\" " + chInfo.name);
            //}
            return false;
        }
        return true;
    }

    public enum TriggerOnHit
    {
        None = 0,
        FirstCollision = 1,
        EveryCollision = 2,
        HitTarget = 3
    }

    public enum DamageType
    {
        AD = 0,
        AP = 1,
        True = 2
    }

    [SerializeField] private Details _details;

    public Details details
    {
        get { return _details; }
        set { _details = value; }
    }

    [Serializable]
    public class Details
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Condition condition;
        public float skillRange;
        [JsonConverter(typeof(StringEnumConverter))]
        public TargetFilter targetFilter;
        public bool needTarget;
        public bool canUseSelf;
        public bool canBeSilenced;
        [JsonConverter(typeof(StringEnumConverter))]
        public SearchAmong searchAmong;
        public bool isTriggerChAnim;
        public bool isScaleAnimWithAS;
        public bool couldInterrupt;
        public bool canMoveWhenCast;
        public float delayTrigger;
        public bool canCrit;
        public HitDamage hitDamage = new HitDamage();
        public FollowTarget followTarget = new FollowTarget();
        public FixedDirection fixedDirection = new FixedDirection();
        public FlyOutAndBack flyOutAndBack = new FlyOutAndBack();
        public Bounce bounce = new Bounce();
        public AoE aoe = new AoE();
        public Channelling channelling = new Channelling();

        public CrowdControl crowdControl = new CrowdControl();
        public Heal heal = new Heal();
        public Shield shield = new Shield();
        public Dodge dodge = new Dodge();
        public IncreaseCritChance increaseCritChance = new IncreaseCritChance();
        public IncreaseCritDMG increaseCritDMG = new IncreaseCritDMG();
        public IncreaseAD increaseAD = new IncreaseAD();
        public IncreaseAP increaseAP = new IncreaseAP();
        public IncreaseAS increaseAS = new IncreaseAS();
        public IncreaseAR increaseAR = new IncreaseAR();
        public IncreaseMR increaseMR = new IncreaseMR();

        public DecreaseAS decreaseAS = new DecreaseAS();
        public DecreaseAR decreaseAR = new DecreaseAR();
        public DecreaseMR decreaseMR = new DecreaseMR();
        public Details() { }
    }
    public enum HitOn
    {
        None = 0,
        HitTarget = 1,
        AroundTarget = 2,
        AroundSelf = 3
    }
    [Serializable]
    public class HitDamage
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HitOn hitOn;
        public float aoeRange;
        public float aoeMultiplier;

        [JsonConverter(typeof(StringEnumConverter))]
        public DamageType damageType;
        public float[] ad;
        public float[] ap;
        public float[] trueDmg;
        public float[] adMultiplier;
        public float[] apMultiplier;
        public bool adCanChange;
        public bool apCanChange;
        public bool trueDmgCanChange;
        public bool adMultiplierCanChange;
        public bool apMultiplierCanChange;
        public HitDamage() {
            hitOn = HitOn.None;
            aoeRange = 0;
            aoeMultiplier = 0;
            damageType = DamageType.AD;
            ad = new float[0];
            ap = new float[0];
            trueDmg = new float[0];
            adMultiplier = new float[0];
            apMultiplier = new float[0];
            adCanChange = false;
            apCanChange = false;
            trueDmgCanChange = false;
            adMultiplierCanChange = false;
            apMultiplierCanChange = false;
        }
    }
    [Serializable]
    public class FollowTarget
    {
        public float speedFly;
        [JsonConverter(typeof(StringEnumConverter))]
        public TriggerOnHit triggerOnHit;
        public float hitRange;
        public FollowTarget() {
            speedFly = 0;
            triggerOnHit = TriggerOnHit.None;
            hitRange = 0;
        }
    }
    [Serializable]
    public class FixedDirection
    {
        public float activeDistance;
        public float speedFly;
        [JsonConverter(typeof(StringEnumConverter))]
        public TriggerOnHit triggerOnHit;
        public float hitRange;
        public FixedDirection() {
            activeDistance = 0;
            speedFly = 0;
            triggerOnHit = TriggerOnHit.None;
            hitRange = 0;
        }
    }
    [Serializable]
    public class FlyOutAndBack
    {
        public float activeDistance;
        public float speedFly;
        [JsonConverter(typeof(StringEnumConverter))]
        public TriggerOnHit triggerOnHit;
        public float hitRange;
        public FlyOutAndBack() { 
            activeDistance= 0;
            speedFly= 0;
            triggerOnHit = TriggerOnHit.None;
            hitRange = 0;
        }
    }
    [Serializable]
    public class Bounce
    {
        public int[] maxBounces;
        public bool maxBounceCanChange;
        public float speedFly;
        public float bounceRange;
        [JsonConverter(typeof(StringEnumConverter))]
        public TriggerOnHit triggerOnHit;
        public Bounce() { 
            maxBounces = new int[0];
            maxBounceCanChange = false;
            speedFly = 0;
            bounceRange = 0;
            triggerOnHit = TriggerOnHit.None;
        }
    }
    [Serializable]
    public enum AoePos
    {
        None = 0,
        Self = 1,
        Target = 2
    }
    [Serializable]
    public class AoE
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public AoePos aoePos;
        public float aoeRange;
        public int maxHitNum;
        public float lifeTime;
        [JsonConverter(typeof(StringEnumConverter))]
        public TriggerOnHit triggerOnHit;
        public bool canHitMultiTime;
        public AoE() { 
            aoePos = AoePos.None;
            aoeRange = 0;
            maxHitNum = 0;
            lifeTime = 0;
            triggerOnHit = TriggerOnHit.None;
            canHitMultiTime = false;
        }
    }
    [Serializable]
    public class Channelling
    {
        public float timeChanneling;
        public float tickInterval;
        public int tickTimes;
        public bool triggerRightAway;
        public Channelling()
        {
            timeChanneling = 0;
            tickInterval = 0;
            tickTimes = 0;
            triggerRightAway = false;
        }
    }
    public enum BuffOn
    {
        None = 0,
        Self = 1,
        Target = 2,
        Allies = 3,
        Enemies = 4
    }
    public enum AddType
    {
        None = 0,
        UpdateLifetime = 1,
        StackUp = 2
    }
    public enum ControlType
    {
        None = 0,
        Stun = 1,
        Silence = 2,
        Slow = 3,
        Blind = 4
    }
    [Serializable]
    public class BuffDetail
    {
        public string buffID;
        [JsonConverter(typeof(StringEnumConverter))]
        public BuffOn buffOn;
        public float buffRange;
        public int maxHitNum;
        public bool haveLifeTime;
        public float[] lifeTime;
        public bool destroyOnLifeEnding;
        [JsonConverter(typeof(StringEnumConverter))]
        public AddType addType;
        public int maxStackUp;
        public bool lifeTimeCanChange;
        public BuffDetail()
        {
            buffID = String.Empty;
            buffOn = BuffOn.None;
            buffRange = 0;
            maxHitNum = 0;
            haveLifeTime = false;
            lifeTime = new float[0];
            destroyOnLifeEnding = false;
            addType = AddType.None;
            maxStackUp = 0;
            lifeTimeCanChange = false;
        }
    }
    [Serializable]
    public class CrowdControl : BuffDetail
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ControlType controlType;

        public CrowdControl() : base()
        {
            controlType = ControlType.None;
        }
    }
    [Serializable]
    public class DecreaseAS : BuffDetail
    {
        public float[] attackSpeedMult;
        public bool attackSpeedMultCanChange;
        public DecreaseAS() : base()
        {
            attackSpeedMult = new float[0];
            attackSpeedMultCanChange = false;
        }
    }
    [Serializable]
    public class DecreaseAR : BuffDetail
    {
        public float[] armorAdd;
        public float[] armorMult;
        public bool armorAddCanChange;
        public bool armorMultCanChange;
        public DecreaseAR() : base()
        {
            armorAdd = new float[0];
            armorMult = new float[0];
            armorAddCanChange = false;
            armorMultCanChange = false;
        }
    }
    [Serializable]
    public class DecreaseMR : BuffDetail
    {
        public float[] magicResistanceAdd;
        public float[] magicResistanceMult;
        public bool magicResistanceAddCanChange;
        public bool magicResistanceMultCanChange;
        public DecreaseMR() : base()
        {
            magicResistanceAdd = new float[0];
            magicResistanceMult = new float[0];
            magicResistanceMultCanChange = false;
            magicResistanceAddCanChange = false;
        }
    }
    [Serializable]
    public class Heal : BuffDetail
    {
        public float[] healing;
        public float[] extraMaxHpPercentage;
        public float[] extraCurrentHpPercentage;
        public float[] extraMissingHpPercentage;
        public bool healingCanChange;
        public bool extraMaxHpPercentageCanChange;
        public bool extraCurrentHpPercentageCanChange;
        public bool extraMissingHpPercentageCanChange;
        public Heal() : base()
        {
            healing = new float[0];
            extraMaxHpPercentage = new float[0];
            extraCurrentHpPercentage = new float[0];
            extraMissingHpPercentage = new float[0];
            healingCanChange = false;
            extraMaxHpPercentageCanChange = false;
            extraCurrentHpPercentageCanChange = false;
            extraMissingHpPercentageCanChange = false;
        }
    }
    [Serializable]
    public class Shield : BuffDetail
    {
        public int[] maxHit;
        public bool maxHitCanChange;
        public float[] shieldAmount;
        public float[] extraMaxHpPercentage;
        public float[] extraCurrentHpPercentage;
        public float[] extraMissingHpPercentage;
        public float[] extraADPercentage;
        public float[] extraAPPercentage;
        public bool shieldAmountCanChange;
        public bool extraMaxHpPercentageCanChange;
        public bool extraCurrentHpPercentageCanChange;
        public bool extraMissingHpPercentageCanChange;
        public bool extraADPercentageCanChange;
        public bool extraAPPercentageCanChange;
        public Shield() : base()
        {
            maxHit = new int[0];
            maxHitCanChange = false;
            shieldAmount = new float[0];
            extraMaxHpPercentage = new float[0];
            extraCurrentHpPercentage = new float[0];
            extraMissingHpPercentage = new float[0];
            extraADPercentage = new float[0];
            extraAPPercentage = new float[0];
            shieldAmountCanChange = false;
            extraMaxHpPercentageCanChange = false;
            extraCurrentHpPercentageCanChange = false;
            extraMissingHpPercentageCanChange = false;
            extraADPercentageCanChange = false;
            extraAPPercentageCanChange = false;
        }
    }
    [Serializable]
    public class Dodge : BuffDetail
    {
        public float[] dodgeLifetime;
        public bool dodgeLifetimeCanChange;
        public Dodge() : base()
        {
            dodgeLifetime = new float[0];
            dodgeLifetimeCanChange = false;
        }
    }
    [Serializable]
    public class IncreaseAD : BuffDetail
    {
        public float[] attackDamageAdd;
        public float[] attackDamageMult;
        public bool attackDamageAddCanChange;
        public bool attackDamageMultCanChange;
        public IncreaseAD() : base()
        {
            attackDamageAdd = new float[0];
            attackDamageMult = new float[0];
            attackDamageAddCanChange = false;
            attackDamageMultCanChange = false;
        }
    }
    [Serializable]
    public class IncreaseAP : BuffDetail
    {
        public float[] abilityDamageAdd;
        public float[] abilityDamageMult;
        public bool abilityDamageAddCanChange;
        public bool abilityDamageMultCanChange;
        public IncreaseAP() : base()
        {
            abilityDamageAdd = new float[0];
            abilityDamageMult = new float[0];
            abilityDamageAddCanChange = false;
            abilityDamageMultCanChange = false;
        }
    }
    [Serializable]
    public class IncreaseCritChance : BuffDetail
    {
        public float[] criticalChanceAdd;
        public bool criticalChanceAddCanChange;
        public IncreaseCritChance() : base()
        {
            criticalChanceAdd = new float[0];
            criticalChanceAddCanChange = false;
        }
    }
    [Serializable]
    public class IncreaseCritDMG : BuffDetail
    {
        public float[] criticalDamageAdd;
        public bool criticalDamageAddCanChange;
        public IncreaseCritDMG() : base()
        {
            criticalDamageAdd = new float[0];
            criticalDamageAddCanChange = false;
        }
    }
    [Serializable]
    public class IncreaseAS : BuffDetail
    {
        public float[] attackSpeedMult;
        public bool attackSpeedMultCanChange;
        public IncreaseAS() : base()
        {
            attackSpeedMult = new float[0];
            attackSpeedMultCanChange = false;
        }
    }
    [Serializable]
    public class IncreaseAR : BuffDetail
    {
        public float[] armorAdd;
        public float[] armorMult;
        public bool armorAddCanChange;
        public bool armorMultCanChange;
        public IncreaseAR() : base()
        {
            armorAdd = new float[0];
            armorMult = new float[0];
            armorAddCanChange = false;
            armorMultCanChange = false;
        }
    }
    [Serializable]
    public class IncreaseMR : BuffDetail
    {
        public float[] magicResistanceAdd;
        public float[] magicResistanceMult;
        public bool magicResistanceAddCanChange;
        public bool magicResistanceMultCanChange;
        public IncreaseMR() : base()
        {
            magicResistanceAdd = new float[0];
            magicResistanceMult = new float[0];
            magicResistanceAddCanChange = false;
            magicResistanceMultCanChange = false;
        }
    }

    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)       //gửi dữ liệu
        {
            stream.SendNext(details.condition);
            stream.SendNext(details.skillRange);
            stream.SendNext(details.targetFilter);
            stream.SendNext(details.needTarget);
            stream.SendNext(details.canUseSelf);
            stream.SendNext(details.canBeSilenced);
            stream.SendNext(details.searchAmong);
            stream.SendNext(details.isTriggerChAnim);
            stream.SendNext(details.isScaleAnimWithAS);
            stream.SendNext(details.couldInterrupt);
            stream.SendNext(details.canMoveWhenCast);
            stream.SendNext(details.delayTrigger);
            stream.SendNext(details.canCrit);
            //hitDamage
            if (details.hitDamage != null)
            {
                stream.SendNext(details.hitDamage.hitOn);
                stream.SendNext(details.hitDamage.aoeRange);
                stream.SendNext(details.hitDamage.aoeMultiplier);
                stream.SendNext(details.hitDamage.damageType);
                stream.SendNext(details.hitDamage.ad);
                stream.SendNext(details.hitDamage.ap);
                stream.SendNext(details.hitDamage.trueDmg);
                stream.SendNext(details.hitDamage.adMultiplier);
                stream.SendNext(details.hitDamage.apMultiplier);
                stream.SendNext(details.hitDamage.adCanChange);
                stream.SendNext(details.hitDamage.apCanChange);
                stream.SendNext(details.hitDamage.trueDmgCanChange);
                stream.SendNext(details.hitDamage.adMultiplierCanChange);
                stream.SendNext(details.hitDamage.apMultiplierCanChange);
            }
            //followTarget
            //public FollowTarget followTarget;
            if (details.followTarget != null)
            {
                stream.SendNext(details.followTarget.speedFly);
                stream.SendNext(details.followTarget.triggerOnHit);
                stream.SendNext(details.followTarget.hitRange);
            }
            //fixedDirection
            //public FixedDirection fixedDirection;
            if (details.fixedDirection != null)
            {
                stream.SendNext(details.fixedDirection.activeDistance);
                stream.SendNext(details.fixedDirection.speedFly);
                stream.SendNext(details.fixedDirection.triggerOnHit);
                stream.SendNext(details.fixedDirection.hitRange);
            }
            //flyOutAndBack
            //public FlyOutAndBack flyOutAndBack;
            if (details.flyOutAndBack != null)
            {
                stream.SendNext(details.flyOutAndBack.activeDistance);
                stream.SendNext(details.flyOutAndBack.speedFly);
                stream.SendNext(details.flyOutAndBack.triggerOnHit);
                stream.SendNext(details.flyOutAndBack.hitRange);
            }
            //bounce
            //public Bounce bounce;
            if (details.bounce != null)
            {
                stream.SendNext(details.bounce.maxBounces);
                stream.SendNext(details.bounce.maxBounceCanChange);
                stream.SendNext(details.bounce.speedFly);
                stream.SendNext(details.bounce.bounceRange);
                stream.SendNext(details.bounce.triggerOnHit);
            }
            //aoe
            //public AoE aoe;
            if (details.aoe != null)
            {
                stream.SendNext(details.aoe.aoePos);
                stream.SendNext(details.aoe.aoeRange);
                stream.SendNext(details.aoe.maxHitNum);
                stream.SendNext(details.aoe.lifeTime);
                stream.SendNext(details.aoe.triggerOnHit);
                stream.SendNext(details.aoe.canHitMultiTime);
            }
            //channelling
            //public Channelling channelling;
            if (details.channelling != null)
            {
                stream.SendNext(details.channelling.timeChanneling);
                stream.SendNext(details.channelling.tickInterval);
                stream.SendNext(details.channelling.tickTimes);
                stream.SendNext(details.channelling.triggerRightAway);
            }
            //crowdControl
            //public CrowdControl crowdControl;
            if (details.crowdControl != null)
            {
                stream.SendNext(details.crowdControl.buffID);
                stream.SendNext(details.crowdControl.buffOn);
                stream.SendNext(details.crowdControl.buffRange);
                stream.SendNext(details.crowdControl.maxHitNum);
                stream.SendNext(details.crowdControl.haveLifeTime);
                stream.SendNext(details.crowdControl.lifeTime);
                stream.SendNext(details.crowdControl.destroyOnLifeEnding);
                stream.SendNext(details.crowdControl.addType);
                stream.SendNext(details.crowdControl.maxStackUp);
                stream.SendNext(details.crowdControl.lifeTimeCanChange);

                stream.SendNext(details.crowdControl.controlType);
            }
            //heal
            //public Heal heal;
            if (details.heal != null)
            {
                stream.SendNext(details.heal.buffID);
                stream.SendNext(details.heal.buffOn);
                stream.SendNext(details.heal.buffRange);
                stream.SendNext(details.heal.maxHitNum);
                stream.SendNext(details.heal.haveLifeTime);
                stream.SendNext(details.heal.lifeTime);
                stream.SendNext(details.heal.destroyOnLifeEnding);
                stream.SendNext(details.heal.addType);
                stream.SendNext(details.heal.maxStackUp);
                stream.SendNext(details.heal.lifeTimeCanChange);

                stream.SendNext(details.heal.healing);
                stream.SendNext(details.heal.extraMaxHpPercentage);
                stream.SendNext(details.heal.extraCurrentHpPercentage);
                stream.SendNext(details.heal.extraMissingHpPercentage);
                stream.SendNext(details.heal.healingCanChange);
                stream.SendNext(details.heal.extraMaxHpPercentageCanChange);
                stream.SendNext(details.heal.extraCurrentHpPercentageCanChange);
                stream.SendNext(details.heal.extraMissingHpPercentageCanChange);
            }
            //shield
            //public Shield shield;
            if (details.shield != null)
            {
                stream.SendNext(details.shield.buffID);
                stream.SendNext(details.shield.buffOn);
                stream.SendNext(details.shield.buffRange);
                stream.SendNext(details.shield.maxHitNum);
                stream.SendNext(details.shield.haveLifeTime);
                stream.SendNext(details.shield.lifeTime);
                stream.SendNext(details.shield.destroyOnLifeEnding);
                stream.SendNext(details.shield.addType);
                stream.SendNext(details.shield.maxStackUp);
                stream.SendNext(details.shield.lifeTimeCanChange);

                stream.SendNext(details.shield.maxHit);
                stream.SendNext(details.shield.maxHitCanChange);
                stream.SendNext(details.shield.shieldAmount);
                stream.SendNext(details.shield.extraMaxHpPercentage);
                stream.SendNext(details.shield.extraCurrentHpPercentage);
                stream.SendNext(details.shield.extraMissingHpPercentage);
                stream.SendNext(details.shield.extraADPercentage);
                stream.SendNext(details.shield.extraAPPercentage);
                stream.SendNext(details.shield.shieldAmountCanChange);
                stream.SendNext(details.shield.extraMaxHpPercentageCanChange);
                stream.SendNext(details.shield.extraCurrentHpPercentageCanChange);
                stream.SendNext(details.shield.extraMissingHpPercentageCanChange);
                stream.SendNext(details.shield.extraADPercentageCanChange);
                stream.SendNext(details.shield.extraAPPercentageCanChange);
            }
            //public Dodge dodge;
            if (details.dodge != null)
            {
                stream.SendNext(details.dodge.buffID);
                stream.SendNext(details.dodge.buffOn);
                stream.SendNext(details.dodge.buffRange);
                stream.SendNext(details.dodge.maxHitNum);
                stream.SendNext(details.dodge.haveLifeTime);
                stream.SendNext(details.dodge.lifeTime);
                stream.SendNext(details.dodge.destroyOnLifeEnding);
                stream.SendNext(details.dodge.addType);
                stream.SendNext(details.dodge.maxStackUp);
                stream.SendNext(details.dodge.lifeTimeCanChange);

                stream.SendNext(details.dodge.dodgeLifetime);
                stream.SendNext(details.dodge.dodgeLifetimeCanChange);
            }
            //increaseCritChance
            //public IncreaseCritChance increaseCritChance;
            if (details.increaseCritChance != null)
            {
                stream.SendNext(details.increaseCritChance.buffID);
                stream.SendNext(details.increaseCritChance.buffOn);
                stream.SendNext(details.increaseCritChance.buffRange);
                stream.SendNext(details.increaseCritChance.maxHitNum);
                stream.SendNext(details.increaseCritChance.haveLifeTime);
                stream.SendNext(details.increaseCritChance.lifeTime);
                stream.SendNext(details.increaseCritChance.destroyOnLifeEnding);
                stream.SendNext(details.increaseCritChance.addType);
                stream.SendNext(details.increaseCritChance.maxStackUp);
                stream.SendNext(details.increaseCritChance.lifeTimeCanChange);

                stream.SendNext(details.increaseCritChance.criticalChanceAdd);
                stream.SendNext(details.increaseCritChance.criticalChanceAddCanChange);
            }
            //increaseCritDMG
            //public IncreaseCritDMG increaseCritDMG;
            if (details.increaseCritDMG != null)
            {
                stream.SendNext(details.increaseCritDMG.buffID);
                stream.SendNext(details.increaseCritDMG.buffOn);
                stream.SendNext(details.increaseCritDMG.buffRange);
                stream.SendNext(details.increaseCritDMG.maxHitNum);
                stream.SendNext(details.increaseCritDMG.haveLifeTime);
                stream.SendNext(details.increaseCritDMG.lifeTime);
                stream.SendNext(details.increaseCritDMG.destroyOnLifeEnding);
                stream.SendNext(details.increaseCritDMG.addType);
                stream.SendNext(details.increaseCritDMG.maxStackUp);
                stream.SendNext(details.increaseCritDMG.lifeTimeCanChange);

                stream.SendNext(details.increaseCritDMG.criticalDamageAdd);
                stream.SendNext(details.increaseCritDMG.criticalDamageAddCanChange);
            }
            //increaseAD
            //public IncreaseAD increaseAD;
            if (details.increaseAD != null)
            {
                stream.SendNext(details.increaseAD.buffID);
                stream.SendNext(details.increaseAD.buffOn);
                stream.SendNext(details.increaseAD.buffRange);
                stream.SendNext(details.increaseAD.maxHitNum);
                stream.SendNext(details.increaseAD.haveLifeTime);
                stream.SendNext(details.increaseAD.lifeTime);
                stream.SendNext(details.increaseAD.destroyOnLifeEnding);
                stream.SendNext(details.increaseAD.addType);
                stream.SendNext(details.increaseAD.maxStackUp);
                stream.SendNext(details.increaseAD.lifeTimeCanChange);

                stream.SendNext(details.increaseAD.attackDamageAdd);
                stream.SendNext(details.increaseAD.attackDamageMult);
                stream.SendNext(details.increaseAD.attackDamageAddCanChange);
                stream.SendNext(details.increaseAD.attackDamageMultCanChange);
            }
            //increaseAP
            //public IncreaseAP increaseAP;
            if (details.increaseAP != null)
            {
                stream.SendNext(details.increaseAP.buffID);
                stream.SendNext(details.increaseAP.buffOn);
                stream.SendNext(details.increaseAP.buffRange);
                stream.SendNext(details.increaseAP.maxHitNum);
                stream.SendNext(details.increaseAP.haveLifeTime);
                stream.SendNext(details.increaseAP.lifeTime);
                stream.SendNext(details.increaseAP.destroyOnLifeEnding);
                stream.SendNext(details.increaseAP.addType);
                stream.SendNext(details.increaseAP.maxStackUp);
                stream.SendNext(details.increaseAP.lifeTimeCanChange);

                stream.SendNext(details.increaseAP.abilityDamageAdd);
                stream.SendNext(details.increaseAP.abilityDamageMult);
                stream.SendNext(details.increaseAP.abilityDamageAddCanChange);
                stream.SendNext(details.increaseAP.abilityDamageMultCanChange);
            }
            //increaseAS
            //public IncreaseAS increaseAS;
            if (details.increaseAS != null)
            {
                stream.SendNext(details.increaseAS.buffID);
                stream.SendNext(details.increaseAS.buffOn);
                stream.SendNext(details.increaseAS.buffRange);
                stream.SendNext(details.increaseAS.maxHitNum);
                stream.SendNext(details.increaseAS.haveLifeTime);
                stream.SendNext(details.increaseAS.lifeTime);
                stream.SendNext(details.increaseAS.destroyOnLifeEnding);
                stream.SendNext(details.increaseAS.addType);
                stream.SendNext(details.increaseAS.maxStackUp);
                stream.SendNext(details.increaseAS.lifeTimeCanChange);

                stream.SendNext(details.increaseAS.attackSpeedMult);
                stream.SendNext(details.increaseAS.attackSpeedMultCanChange);
            }
            //increaseAR
            //public IncreaseAR increaseAR;
            if (details.increaseAR != null)
            {
                stream.SendNext(details.increaseAR.buffID);
                stream.SendNext(details.increaseAR.buffOn);
                stream.SendNext(details.increaseAR.buffRange);
                stream.SendNext(details.increaseAR.maxHitNum);
                stream.SendNext(details.increaseAR.haveLifeTime);
                stream.SendNext(details.increaseAR.lifeTime);
                stream.SendNext(details.increaseAR.destroyOnLifeEnding);
                stream.SendNext(details.increaseAR.addType);
                stream.SendNext(details.increaseAR.maxStackUp);
                stream.SendNext(details.increaseAR.lifeTimeCanChange);

                stream.SendNext(details.increaseAR.armorAdd);
                stream.SendNext(details.increaseAR.armorMult);
                stream.SendNext(details.increaseAR.armorAddCanChange);
                stream.SendNext(details.increaseAR.armorMultCanChange);
            }
            //increaseMR
            //public IncreaseMR increaseMR;
            if (details.increaseMR != null)
            {
                stream.SendNext(details.increaseMR.buffID);
                stream.SendNext(details.increaseMR.buffOn);
                stream.SendNext(details.increaseMR.buffRange);
                stream.SendNext(details.increaseMR.maxHitNum);
                stream.SendNext(details.increaseMR.haveLifeTime);
                stream.SendNext(details.increaseMR.lifeTime);
                stream.SendNext(details.increaseMR.destroyOnLifeEnding);
                stream.SendNext(details.increaseMR.addType);
                stream.SendNext(details.increaseMR.maxStackUp);
                stream.SendNext(details.increaseMR.lifeTimeCanChange);

                stream.SendNext(details.increaseMR.magicResistanceAdd);
                stream.SendNext(details.increaseMR.magicResistanceMult);
                stream.SendNext(details.increaseMR.magicResistanceAddCanChange);
                stream.SendNext(details.increaseMR.magicResistanceMultCanChange);
            }
            //decreaseAS
            //public DecreaseAS decreaseAS;
            if (details.decreaseAS != null)
            {
                stream.SendNext(details.decreaseAS.buffID);
                stream.SendNext(details.decreaseAS.buffOn);
                stream.SendNext(details.decreaseAS.buffRange);
                stream.SendNext(details.decreaseAS.maxHitNum);
                stream.SendNext(details.decreaseAS.haveLifeTime);
                stream.SendNext(details.decreaseAS.lifeTime);
                stream.SendNext(details.decreaseAS.destroyOnLifeEnding);
                stream.SendNext(details.decreaseAS.addType);
                stream.SendNext(details.decreaseAS.maxStackUp);
                stream.SendNext(details.decreaseAS.lifeTimeCanChange);

                stream.SendNext(details.decreaseAS.attackSpeedMult);
                stream.SendNext(details.decreaseAS.attackSpeedMultCanChange);
            }
            //decreaseAR
            //public DecreaseAR decreaseAR;
            if (details.decreaseAR != null)
            {
                stream.SendNext(details.decreaseAR.buffID);
                stream.SendNext(details.decreaseAR.buffOn);
                stream.SendNext(details.decreaseAR.buffRange);
                stream.SendNext(details.decreaseAR.maxHitNum);
                stream.SendNext(details.decreaseAR.haveLifeTime);
                stream.SendNext(details.decreaseAR.lifeTime);
                stream.SendNext(details.decreaseAR.destroyOnLifeEnding);
                stream.SendNext(details.decreaseAR.addType);
                stream.SendNext(details.decreaseAR.maxStackUp);
                stream.SendNext(details.decreaseAR.lifeTimeCanChange);

                stream.SendNext(details.decreaseAR.armorAdd);
                stream.SendNext(details.decreaseAR.armorMult);
                stream.SendNext(details.decreaseAR.armorAddCanChange);
                stream.SendNext(details.decreaseAR.armorMultCanChange);
            }
            //decreaseMR
            //public DecreaseMR decreaseMR;
            if (details.decreaseMR != null)
            {
                stream.SendNext(details.decreaseMR.buffID);
                stream.SendNext(details.decreaseMR.buffOn);
                stream.SendNext(details.decreaseMR.buffRange);
                stream.SendNext(details.decreaseMR.maxHitNum);
                stream.SendNext(details.decreaseMR.haveLifeTime);
                stream.SendNext(details.decreaseMR.lifeTime);
                stream.SendNext(details.decreaseMR.destroyOnLifeEnding);
                stream.SendNext(details.decreaseMR.addType);
                stream.SendNext(details.decreaseMR.maxStackUp);
                stream.SendNext(details.decreaseMR.lifeTimeCanChange);

                stream.SendNext(details.decreaseMR.magicResistanceAdd);
                stream.SendNext(details.decreaseMR.magicResistanceMult);
                stream.SendNext(details.decreaseMR.magicResistanceMultCanChange);
                stream.SendNext(details.decreaseMR.magicResistanceAddCanChange);
            }
    
        }
        else if (stream.IsReading)  //nhận dữ liệu
        {
            details.condition = (Condition)(int)stream.ReceiveNext();
            details.skillRange = (float)stream.ReceiveNext();
            details.targetFilter = (TargetFilter)(int)stream.ReceiveNext();
            details.needTarget = (bool)stream.ReceiveNext();
            details.canUseSelf = (bool)stream.ReceiveNext();
            details.canBeSilenced = (bool)stream.ReceiveNext();
            details.searchAmong = (SearchAmong)(int)stream.ReceiveNext();
            details.isTriggerChAnim = (bool)stream.ReceiveNext();
            details.isScaleAnimWithAS = (bool)stream.ReceiveNext();
            details.couldInterrupt = (bool)stream.ReceiveNext();
            details.canMoveWhenCast = (bool)stream.ReceiveNext();
            details.delayTrigger = (float)stream.ReceiveNext();
            details.canCrit = (bool)stream.ReceiveNext();

            if (details.hitDamage != null)
            {
                details.hitDamage.hitOn = (HitOn)(int)stream.ReceiveNext();
                details.hitDamage.aoeRange = (float)stream.ReceiveNext();
                details.hitDamage.aoeMultiplier = (float)stream.ReceiveNext();
                details.hitDamage.damageType = (DamageType)(int)stream.ReceiveNext();
                details.hitDamage.ad = (float[])stream.ReceiveNext();
                details.hitDamage.ap = (float[])stream.ReceiveNext();
                details.hitDamage.trueDmg = (float[])stream.ReceiveNext();
                details.hitDamage.adMultiplier = (float[])stream.ReceiveNext();
                details.hitDamage.apMultiplier = (float[])stream.ReceiveNext();
                details.hitDamage.adCanChange = (bool)stream.ReceiveNext();
                details.hitDamage.apCanChange = (bool)stream.ReceiveNext();
                details.hitDamage.trueDmgCanChange = (bool)stream.ReceiveNext();
                details.hitDamage.adMultiplierCanChange = (bool)stream.ReceiveNext();
                details.hitDamage.apMultiplierCanChange = (bool)stream.ReceiveNext();
            }

            //arget followTarget;
            if (details.followTarget != null)
            {
                details.followTarget.speedFly = (float)stream.ReceiveNext();
                details.followTarget.triggerOnHit = (TriggerOnHit)(int)stream.ReceiveNext();
                details.followTarget.hitRange = (float)stream.ReceiveNext();
            }

            //rection fixedDirection;
            if (details.fixedDirection != null)
            {
                details.fixedDirection.activeDistance = (float)stream.ReceiveNext();
                details.fixedDirection.speedFly = (float)stream.ReceiveNext();
                details.fixedDirection.triggerOnHit = (TriggerOnHit)(int)stream.ReceiveNext();
                details.fixedDirection.hitRange = (float)stream.ReceiveNext();
            }

            //ndBack flyOutAndBack;
            if (details.flyOutAndBack != null)
            {
                details.flyOutAndBack.activeDistance = (float)stream.ReceiveNext();
                details.flyOutAndBack.speedFly = (float)stream.ReceiveNext();
                details.flyOutAndBack.triggerOnHit = (TriggerOnHit)(int)stream.ReceiveNext();
                details.flyOutAndBack.hitRange = (float)stream.ReceiveNext();
            }

            //bounce;
            if (details.bounce != null)
            {
                details.bounce.maxBounces = (int[])stream.ReceiveNext();
                details.bounce.maxBounceCanChange = (bool)stream.ReceiveNext();
                details.bounce.speedFly = (float)stream.ReceiveNext();
                details.bounce.bounceRange = (float)stream.ReceiveNext();
                details.bounce.triggerOnHit = (TriggerOnHit)(int)stream.ReceiveNext();
            }

            //aoe
            if (details.aoe != null)
            {
                details.aoe.aoePos = (AoePos)(int)stream.ReceiveNext();
                details.aoe.aoeRange = (float)stream.ReceiveNext();
                details.aoe.maxHitNum = (int)stream.ReceiveNext();
                details.aoe.lifeTime = (float)stream.ReceiveNext();
                details.aoe.triggerOnHit = (TriggerOnHit)(int)stream.ReceiveNext();
                details.aoe.canHitMultiTime = (bool)stream.ReceiveNext();
            }

            //ling channelling;
            if (details.channelling != null)
            {
                details.channelling.timeChanneling = (float)stream.ReceiveNext();
                details.channelling.tickInterval = (float)stream.ReceiveNext();
                details.channelling.tickTimes = (int)stream.ReceiveNext();
                details.channelling.triggerRightAway = (bool)stream.ReceiveNext();
            }

            //ntrol crowdControl;
            if (details.crowdControl != null)
            {
                details.crowdControl.buffID = (string)stream.ReceiveNext();
                details.crowdControl.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.crowdControl.buffRange = (float)stream.ReceiveNext();
                details.crowdControl.maxHitNum = (int)stream.ReceiveNext();
                details.crowdControl.haveLifeTime = (bool)stream.ReceiveNext();
                details.crowdControl.lifeTime = (float[])stream.ReceiveNext();
                details.crowdControl.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.crowdControl.addType = (AddType)(int)stream.ReceiveNext();
                details.crowdControl.maxStackUp = (int)stream.ReceiveNext();
                details.crowdControl.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.crowdControl.controlType = (ControlType)(int)stream.ReceiveNext();
            }

            //heal;
            if (details.heal != null)
            {
                details.heal.buffID = (string)stream.ReceiveNext();
                details.heal.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.heal.buffRange = (float)stream.ReceiveNext();
                details.heal.maxHitNum = (int)stream.ReceiveNext();
                details.heal.haveLifeTime = (bool)stream.ReceiveNext();
                details.heal.lifeTime = (float[])stream.ReceiveNext();
                details.heal.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.heal.addType = (AddType)(int)stream.ReceiveNext();
                details.heal.maxStackUp = (int)stream.ReceiveNext();
                details.heal.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.heal.healing = (float[])stream.ReceiveNext();
                details.heal.extraMaxHpPercentage = (float[])stream.ReceiveNext();
                details.heal.extraCurrentHpPercentage = (float[])stream.ReceiveNext();
                details.heal.extraMissingHpPercentage = (float[])stream.ReceiveNext();
                details.heal.healingCanChange = (bool)stream.ReceiveNext();
                details.heal.extraMaxHpPercentageCanChange = (bool)stream.ReceiveNext();
                details.heal.extraCurrentHpPercentageCanChange = (bool)stream.ReceiveNext();
                details.heal.extraMissingHpPercentageCanChange = (bool)stream.ReceiveNext();
            }

            //shield;
            if (details.shield != null)
            {
                details.shield.buffID = (string)stream.ReceiveNext();
                details.shield.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.shield.buffRange = (float)stream.ReceiveNext();
                details.shield.maxHitNum = (int)stream.ReceiveNext();
                details.shield.haveLifeTime = (bool)stream.ReceiveNext();
                details.shield.lifeTime = (float[])stream.ReceiveNext();
                details.shield.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.shield.addType = (AddType)(int)stream.ReceiveNext();
                details.shield.maxStackUp = (int)stream.ReceiveNext();
                details.shield.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.shield.maxHit = (int[])stream.ReceiveNext();
                details.shield.maxHitCanChange = (bool)stream.ReceiveNext();
                details.shield.shieldAmount = (float[])stream.ReceiveNext();
                details.shield.extraMaxHpPercentage = (float[])stream.ReceiveNext();
                details.shield.extraCurrentHpPercentage = (float[])stream.ReceiveNext();
                details.shield.extraMissingHpPercentage = (float[])stream.ReceiveNext();
                details.shield.extraADPercentage = (float[])stream.ReceiveNext();
                details.shield.extraAPPercentage = (float[])stream.ReceiveNext();
                details.shield.shieldAmountCanChange = (bool)stream.ReceiveNext();
                details.shield.extraMaxHpPercentageCanChange = (bool)stream.ReceiveNext();
                details.shield.extraCurrentHpPercentageCanChange = (bool)stream.ReceiveNext();
                details.shield.extraMissingHpPercentageCanChange = (bool)stream.ReceiveNext();
                details.shield.extraADPercentageCanChange = (bool)stream.ReceiveNext();
                details.shield.extraAPPercentageCanChange = (bool)stream.ReceiveNext();
            }
            //public Dodge dodge;
            if (details.dodge != null)
            {
                details.dodge.buffID = (string)stream.ReceiveNext();
                details.dodge.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.dodge.buffRange = (float)stream.ReceiveNext();
                details.dodge.maxHitNum = (int)stream.ReceiveNext();
                details.dodge.haveLifeTime = (bool)stream.ReceiveNext();
                details.dodge.lifeTime = (float[])stream.ReceiveNext();
                details.dodge.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.dodge.addType = (AddType)(int)stream.ReceiveNext();
                details.dodge.maxStackUp = (int)stream.ReceiveNext();
                details.dodge.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.dodge.dodgeLifetime = (float[])stream.ReceiveNext();
                details.dodge.dodgeLifetimeCanChange = (bool)stream.ReceiveNext();
            }
            //increaseCritChance
            //public IncreaseCritChance increaseCritChance;
            if (details.increaseCritChance != null)
            {
                details.increaseCritChance.buffID = (string)stream.ReceiveNext();
                details.increaseCritChance.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.increaseCritChance.buffRange = (float)stream.ReceiveNext();
                details.increaseCritChance.maxHitNum = (int)stream.ReceiveNext();
                details.increaseCritChance.haveLifeTime = (bool)stream.ReceiveNext();
                details.increaseCritChance.lifeTime = (float[])stream.ReceiveNext();
                details.increaseCritChance.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.increaseCritChance.addType = (AddType)(int)stream.ReceiveNext();
                details.increaseCritChance.maxStackUp = (int)stream.ReceiveNext();
                details.increaseCritChance.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.increaseCritChance.criticalChanceAdd = (float[])stream.ReceiveNext();
                details.increaseCritChance.criticalChanceAddCanChange = (bool)stream.ReceiveNext();
            }
            //increaseCritDMG
            //public IncreaseCritDMG increaseCritDMG;
            if (details.increaseCritDMG != null)
            {
                details.increaseCritDMG.buffID = (string)stream.ReceiveNext();
                details.increaseCritDMG.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.increaseCritDMG.buffRange = (float)stream.ReceiveNext();
                details.increaseCritDMG.maxHitNum = (int)stream.ReceiveNext();
                details.increaseCritDMG.haveLifeTime = (bool)stream.ReceiveNext();
                details.increaseCritDMG.lifeTime = (float[])stream.ReceiveNext();
                details.increaseCritDMG.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.increaseCritDMG.addType = (AddType)(int)stream.ReceiveNext();
                details.increaseCritDMG.maxStackUp = (int)stream.ReceiveNext();
                details.increaseCritDMG.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.increaseCritDMG.criticalDamageAdd = (float[])stream.ReceiveNext();
                details.increaseCritDMG.criticalDamageAddCanChange = (bool)stream.ReceiveNext();
            }
            //increaseAD
            //public IncreaseAD increaseAD;
            if (details.increaseAD != null)
            {
                details.increaseAD.buffID = (string)stream.ReceiveNext();
                details.increaseAD.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.increaseAD.buffRange = (float)stream.ReceiveNext();
                details.increaseAD.maxHitNum = (int)stream.ReceiveNext();
                details.increaseAD.haveLifeTime = (bool)stream.ReceiveNext();
                details.increaseAD.lifeTime = (float[])stream.ReceiveNext();
                details.increaseAD.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.increaseAD.addType = (AddType)(int)stream.ReceiveNext();
                details.increaseAD.maxStackUp = (int)stream.ReceiveNext();
                details.increaseAD.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.increaseAD.attackDamageAdd = (float[])stream.ReceiveNext();
                details.increaseAD.attackDamageMult = (float[])stream.ReceiveNext();
                details.increaseAD.attackDamageAddCanChange = (bool)stream.ReceiveNext();
                details.increaseAD.attackDamageMultCanChange = (bool)stream.ReceiveNext();
            }
            //increaseAP
            //public IncreaseAP increaseAP;
            if (details.increaseAP != null)
            {
                details.increaseAP.buffID = (string)stream.ReceiveNext();
                details.increaseAP.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.increaseAP.buffRange = (float)stream.ReceiveNext();
                details.increaseAP.maxHitNum = (int)stream.ReceiveNext();
                details.increaseAP.haveLifeTime = (bool)stream.ReceiveNext();
                details.increaseAP.lifeTime = (float[])stream.ReceiveNext();
                details.increaseAP.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.increaseAP.addType = (AddType)(int)stream.ReceiveNext();
                details.increaseAP.maxStackUp = (int)stream.ReceiveNext();
                details.increaseAP.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.increaseAP.abilityDamageAdd = (float[])stream.ReceiveNext();
                details.increaseAP.abilityDamageMult = (float[])stream.ReceiveNext();
                details.increaseAP.abilityDamageAddCanChange = (bool)stream.ReceiveNext();
                details.increaseAP.abilityDamageMultCanChange = (bool)stream.ReceiveNext();
            }
            //increaseAS
            //public IncreaseAS increaseAS;
            if (details.increaseAS != null)
            {
                details.increaseAS.buffID = (string)stream.ReceiveNext();
                details.increaseAS.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.increaseAS.buffRange = (float)stream.ReceiveNext();
                details.increaseAS.maxHitNum = (int)stream.ReceiveNext();
                details.increaseAS.haveLifeTime = (bool)stream.ReceiveNext();
                details.increaseAS.lifeTime = (float[])stream.ReceiveNext();
                details.increaseAS.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.increaseAS.addType = (AddType)(int)stream.ReceiveNext();
                details.increaseAS.maxStackUp = (int)stream.ReceiveNext();
                details.increaseAS.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.increaseAS.attackSpeedMult = (float[])stream.ReceiveNext();
                details.increaseAS.attackSpeedMultCanChange = (bool)stream.ReceiveNext();
            }
            //increaseAR
            //public IncreaseAR increaseAR;
            if (details.increaseAR != null)
            {
                details.increaseAR.buffID = (string)stream.ReceiveNext();
                details.increaseAR.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.increaseAR.buffRange = (float)stream.ReceiveNext();
                details.increaseAR.maxHitNum = (int)stream.ReceiveNext();
                details.increaseAR.haveLifeTime = (bool)stream.ReceiveNext();
                details.increaseAR.lifeTime = (float[])stream.ReceiveNext();
                details.increaseAR.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.increaseAR.addType = (AddType)(int)stream.ReceiveNext();
                details.increaseAR.maxStackUp = (int)stream.ReceiveNext();
                details.increaseAR.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.increaseAR.armorAdd = (float[])stream.ReceiveNext();
                details.increaseAR.armorMult = (float[])stream.ReceiveNext();
                details.increaseAR.armorAddCanChange = (bool)stream.ReceiveNext();
                details.increaseAR.armorMultCanChange = (bool)stream.ReceiveNext();
            }
            //increaseMR
            //public IncreaseMR increaseMR;
            if (details.increaseMR != null)
            {
                details.increaseMR.buffID = (string)stream.ReceiveNext();
                details.increaseMR.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.increaseMR.buffRange = (float)stream.ReceiveNext();
                details.increaseMR.maxHitNum = (int)stream.ReceiveNext();
                details.increaseMR.haveLifeTime = (bool)stream.ReceiveNext();
                details.increaseMR.lifeTime = (float[])stream.ReceiveNext();
                details.increaseMR.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.increaseMR.addType = (AddType)(int)stream.ReceiveNext();
                details.increaseMR.maxStackUp = (int)stream.ReceiveNext();
                details.increaseMR.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.increaseMR.magicResistanceAdd = (float[])stream.ReceiveNext();
                details.increaseMR.magicResistanceMult = (float[])stream.ReceiveNext();
                details.increaseMR.magicResistanceAddCanChange = (bool)stream.ReceiveNext();
                details.increaseMR.magicResistanceMultCanChange = (bool)stream.ReceiveNext();
            }
            //decreaseAS
            //public DecreaseAS decreaseAS;
            if (details.decreaseAS != null)
            {
                details.decreaseAS.buffID = (string)stream.ReceiveNext();
                details.decreaseAS.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.decreaseAS.buffRange = (float)stream.ReceiveNext();
                details.decreaseAS.maxHitNum = (int)stream.ReceiveNext();
                details.decreaseAS.haveLifeTime = (bool)stream.ReceiveNext();
                details.decreaseAS.lifeTime = (float[])stream.ReceiveNext();
                details.decreaseAS.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.decreaseAS.addType = (AddType)(int)stream.ReceiveNext();
                details.decreaseAS.maxStackUp = (int)stream.ReceiveNext();
                details.decreaseAS.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.decreaseAS.attackSpeedMult = (float[])stream.ReceiveNext();
                details.decreaseAS.attackSpeedMultCanChange = (bool)stream.ReceiveNext();
            }
            //decreaseAR
            //public DecreaseAR decreaseAR;
            if (details.decreaseAR != null)
            {
                details.decreaseAR.buffID = (string)stream.ReceiveNext();
                details.decreaseAR.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.decreaseAR.buffRange = (float)stream.ReceiveNext();
                details.decreaseAR.maxHitNum = (int)stream.ReceiveNext();
                details.decreaseAR.haveLifeTime = (bool)stream.ReceiveNext();
                details.decreaseAR.lifeTime = (float[])stream.ReceiveNext();
                details.decreaseAR.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.decreaseAR.addType = (AddType)(int)stream.ReceiveNext();
                details.decreaseAR.maxStackUp = (int)stream.ReceiveNext();
                details.decreaseAR.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.decreaseAR.armorAdd = (float[])stream.ReceiveNext();
                details.decreaseAR.armorMult = (float[])stream.ReceiveNext();
                details.decreaseAR.armorAddCanChange = (bool)stream.ReceiveNext();
                details.decreaseAR.armorMultCanChange = (bool)stream.ReceiveNext();
            }
            //decreaseMR
            //public DecreaseMR decreaseMR;
            if (details.decreaseMR != null)
            {
                details.decreaseMR.buffID = (string)stream.ReceiveNext();
                details.decreaseMR.buffOn = (BuffOn)(int)stream.ReceiveNext();
                details.decreaseMR.buffRange = (float)stream.ReceiveNext();
                details.decreaseMR.maxHitNum = (int)stream.ReceiveNext();
                details.decreaseMR.haveLifeTime = (bool)stream.ReceiveNext();
                details.decreaseMR.lifeTime = (float[])stream.ReceiveNext();
                details.decreaseMR.destroyOnLifeEnding = (bool)stream.ReceiveNext();
                details.decreaseMR.addType = (AddType)(int)stream.ReceiveNext();
                details.decreaseMR.maxStackUp = (int)stream.ReceiveNext();
                details.decreaseMR.lifeTimeCanChange = (bool)stream.ReceiveNext();

                details.decreaseMR.magicResistanceAdd = (float[])stream.ReceiveNext();
                details.decreaseMR.magicResistanceMult = (float[])stream.ReceiveNext();
                details.decreaseMR.magicResistanceMultCanChange = (bool)stream.ReceiveNext();
                details.decreaseMR.magicResistanceAddCanChange = (bool)stream.ReceiveNext();
            }
        }
    }
}

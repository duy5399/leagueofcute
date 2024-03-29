using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TraitBase
{
    public string name;
    public string icon;
    public string description;
    public string[][] composition;

    [JsonConverter(typeof(StringEnumConverter))]
    public SkillBase1.BuffOn buffOn;
    public bool isActiveInCombat;
    public bool haveTriggerTime;
    public float[] triggerTime;
    public bool triggerTimeCanChange;
    public bool haveLifeTime;
    public float[] lifeTime;
    public bool lifeTimeCanChange;
    public bool haveStackUp;
    public int[] maxStackUp;
    public bool maxStackUpCanChange;
    public int[] breakpoint;
    public string[] detailBreakpoint;
    public bool activeAtTheStartOfCombat;
}

[Serializable]
public class ClassBase : TraitBase
{
    public enum IdClass
    {
        None = 0,
        Ranger = 1,
        Assassin = 2,
        Brawler = 3,
        Mystic = 4,
        Defender = 5,
        Sorcerer = 6,
        Skirmisher = 7
    }
    public IdClass id;
}

[Serializable]
public class Ranger : ClassBase
{
    public float[] attackSpeedForAllies;
    public float[] attackSpeedForRanger;
}
[Serializable]
public class Assassin : ClassBase
{
    public bool skillCanCrit;
    public float[] criticalChance;
    public float[] criticalDamage;
}
[Serializable]
public class Brawler : ClassBase
{
    public float[] hpForAllies;
    public float[] hpForBrawler;
}
[Serializable]
public class Mystic : ClassBase
{
    public float[] magicResistForAllies;
    public float[] magicResistForMystic;
}
[Serializable]
public class Defender : ClassBase
{
    public float[] armorForAllies;
    public float[] armorForDefender;
}
[Serializable]
public class Sorcerer : ClassBase
{
    public float[] abilityPower;
}
[Serializable]
public class Skirmisher : ClassBase
{
    public float[] shieldAmount;
    public float[] attackDamagePerSecond;
}

[Serializable]
public class OriginBase : TraitBase
{
    public enum IdOrigin
    {
        None = 0,
        Mascot = 1,
        Hextech = 2,
        Yordle = 3,
        Nightbringer = 4,
        Dawnbringer = 5,
        Duelist = 6
    }

    public IdOrigin id;
}

[Serializable]
public class Mascot : OriginBase
{
    public float[] hpRegenForAllies;
    public float[] hpRegenForMascot;
}
[Serializable]
public class Hextech : OriginBase
{
    public float[] manaRegen;
}
[Serializable]
public class Yordle : OriginBase
{
    public float[] decreaseManaRequired;
    public float[] restoreMana;
}
[Serializable]
public class Nightbringer : OriginBase
{
    public float[] shieldAmout;
    public float[] armorPen;
    public float[] magicResistPen;
}
[Serializable]
public class Dawnbringer : OriginBase
{
    public float[] healing;
    public float[] physicalVamp;
    public float[] spellVamp;
}
[Serializable]
public class Duelist : OriginBase
{
    public float[] attackSpeed;
}


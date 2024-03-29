using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[Serializable]
public class HeroProperty
{
    public float hp = 0f;
    public float mana = 0f;

    public float hpRegen = 0f;
    public float manaRegen = 0f;

    public float moveSpeed = 0f;

    public float criticalStrikeChance = 0f;
    public float criticalStrikeDamage = 0f;

    public float attackDamage = 0f;
    public float attackSpeed = 0f;
    public float armorPenetration = 0f;
    public float armorPenetrationPercentage = 0f;

    public float abilityPower = 0f;
    public float magicPenetration = 0f;
    public float magicPenetrationPercentage = 0f;

    public float armor = 0f;
    public float magicResistance = 0f;

    public float physicalVamp = 0f;
    public float spellVamp = 0f;
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StateBuff
{
    public enum TypeBuff
    {
        None = 0,
        //additive
        Add = 1,
        //multiplicative
        Mult = 2
    }

    public SM_Buff buff;
    public TypeBuff typeBuff;
    public float amount;
    public ClassBase classBuff;
    public OriginBase originBuff;
    public Item item;

    public StateBuff(SM_Buff buff, TypeBuff typeBuff, float amount)
    {
        this.buff = buff;
        this.typeBuff = typeBuff;
        this.amount = amount;
    }

    public StateBuff(ClassBase classBuff, TypeBuff typeBuff, float amount)
    {
        this.classBuff = classBuff;
        this.typeBuff = typeBuff;
        this.amount = amount;
    }

    public StateBuff(OriginBase originBuff, TypeBuff typeBuff, float amount)
    {
        this.originBuff = originBuff;
        this.typeBuff = typeBuff;
        this.amount = amount;
    }

    public StateBuff(Item item, TypeBuff typeBuff, float amount)
    {
        this.item = item;
        this.typeBuff = typeBuff;
        this.amount = amount;
    }
}


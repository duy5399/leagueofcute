using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static SkillSpawn1;

public class Item_TitansResolve : ItemBase
{
    [SerializeField] private const int MAX_STACKS = 5;
    [SerializeField] private int stackCount = 0;

    protected override void FixedUpdate()
    {
        if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
        {
            return;
        }
        if (!isActive)
        {
            if (stackCount == MAX_STACKS)
            {
                Debug.Log("stackCount == MAX_STACKS");
                base.info.currentState._buffOnArmor.Add(new StateBuff(_item, StateBuff.TypeBuff.Add, _item.passive.increaseAR.armorAdd[0]));
                base.info.currentState._buffOnMagicResistance.Add(new StateBuff(_item, StateBuff.TypeBuff.Add, _item.passive.increaseMR.magicResistanceAdd[0]));
                isActive = true;
            }
        }
    }

    public override void OnReset()
    {
        base.OnReset();
        base.info.currentState._buffOnArmor.RemoveAll(x => x.item == _item && x.amount == _item.passive.increaseAR.armorAdd[0]);
        base.info.currentState._buffOnMagicResistance.RemoveAll(x => x.item == _item && x.amount == _item.passive.increaseMR.magicResistanceAdd[0]);
        base.info.currentState._buffOnAttackDamage.RemoveAll(x => x.item == _item && x.amount == 1f + _item.passive.increaseAD.attackDamageMult[0]);
        base.info.currentState._buffOnAbilityPower.RemoveAll(x => x.item == _item && x.amount == _item.passive.increaseAP.abilityDamageAdd[0]);
        isActive = false;
    }

    public override void OnBasicAttack(SkillBase1 skillBase, Transform target)
    {
        if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
        {
            return;
        }
        if(stackCount < MAX_STACKS)
        {
            Debug.Log("Item_TitansResolve OnBasicAttack");
            //_itemPassive.TriggerSpawn(base.skills.target.transform);
            base.info.currentState._buffOnAttackDamage.Add(new StateBuff(_item, StateBuff.TypeBuff.Mult, 1f + _item.passive.increaseAD.attackDamageMult[0]));
            base.info.currentState._buffOnAbilityPower.Add(new StateBuff(_item, StateBuff.TypeBuff.Add, _item.passive.increaseAP.abilityDamageAdd[0]));
            stackCount++;
        }
    }

    public override void OnSpecialAbility(SkillBase1 skillBase, Transform target)
    {
        base.OnSpecialAbility(skillBase, target);
        if (stackCount < MAX_STACKS)
        {
            Debug.Log("Item_TitansResolve OnSpecialAbility");
            //_itemPassive.TriggerSpawn(base.skills.target.transform);
            base.info.currentState._buffOnAttackDamage.Add(new StateBuff(_item, StateBuff.TypeBuff.Mult, 1f + _item.passive.increaseAD.attackDamageMult[0]));
            base.info.currentState._buffOnAbilityPower.Add(new StateBuff(_item, StateBuff.TypeBuff.Add, _item.passive.increaseAP.abilityDamageAdd[0]));
            stackCount++;
        }
    }

    public override void OnBeHited(Transform target, float damage, bool isCritical)
    {
        //if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
        //{
        //    return;
        //}
        //if (stackCount < MAX_STACKS)
        //{
        //    Debug.Log("Item_TitansResolve OnBeHited");
        //    //_itemPassive.TriggerSpawn(base.skills.target.transform);
        //    base.info.currentState._buffOnAttackDamage.Add(new StateBuff(_item, StateBuff.TypeBuff.Mult, 1f + _item.passive.increaseAD.attackDamageMult[0]));
        //    base.info.currentState._buffOnAbilityPower.Add(new StateBuff(_item, StateBuff.TypeBuff.Add, _item.passive.increaseAP.abilityDamageAdd[0]));
        //    stackCount++;
        //}
    }
}
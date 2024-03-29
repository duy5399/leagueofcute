using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_GuinsoosRageblade : ItemBase
{
    [SerializeField] private const int MAX_STACKS = 99;
    [SerializeField] private int currentStacks = 0;
    public override void OnReset()
    {
        base.OnReset();
        float amount = 1f + _item.passive.increaseAS.attackSpeedMult[0];
        base.info.currentState._buffOnAttackSpeed.RemoveAll(x => x.item == _item && x.amount == amount);
        currentStacks = 0;
    }

    public override void OnBasicAttack(SkillBase1 skillBase, Transform target)
    {
        if (base.info == null || !isEquipped || base.info.currentState.dead || !base.info.stateCtrl.inCombat || !itemPassive)
        {
            return;
        }
        if(currentStacks < MAX_STACKS)
        {
            Debug.Log("Item_GuinsoosRageblade OnBasicAttack");
            //_itemPassive.TriggerSpawn(base.skills.target.transform);
            base.info.currentState._buffOnAttackSpeed.Add(new StateBuff(_item, StateBuff.TypeBuff.Mult, 1f + _item.passive.increaseAS.attackSpeedMult[0]));
        }
    }
}

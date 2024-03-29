using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChTraits : MonoBehaviour
{
    [SerializeField] protected TraitBase _trait;
    [SerializeField] protected List<GameObject> championsGetBuff = new List<GameObject>();
    [SerializeField] protected int breakpoint;
    [SerializeField] protected bool _isActive = false;
    [SerializeField] protected float triggerTimeLeft;
    [SerializeField] protected int maxStackUp;

    public TraitBase trait
    {
        get { return _trait; }
        set { _trait = value; }
    }
    public bool isActive
    {
        get { return _isActive; }
        set { _isActive = value; }
    }


    protected virtual void FixedUpdate()
    {
        if (!isActive)
        {
            return;
        }
        if (trait.isActiveInCombat)
        {
            if (maxStackUp <= 0)
            {
                isActive = false;
                return;
            }
            triggerTimeLeft -= Time.fixedDeltaTime;
            if (triggerTimeLeft <= 0)
            {
                OnTriggerBuff();
                if (maxStackUp > 0)
                {
                    triggerTimeLeft = trait.triggerTimeCanChange == true ? trait.triggerTime[breakpoint] : trait.triggerTime[0];
                    maxStackUp--;
                }
                else
                {
                    isActive = false;
                }
            }
        }
    }

    public virtual void Launch(int numDistinctChampion, List<GameObject> championsGetBuff)
    {
        //số lượng unit khác nhau nếu không đủ mốc => gỡ buff trên tất cả unit
        if (numDistinctChampion < _trait.breakpoint[0])
        {
            foreach (var i in championsGetBuff)
            {
                ChampionInfo1 chInfo = i.GetComponent<ChampionInfo1>();
                OnRemove(chInfo);
            }
            return;
        }
        //kiểm tra xem có thể kích hoạt được mốc nào (ưu tiên từ cao > thấp)
        int num = -1;
        for (int i = _trait.breakpoint.Length - 1; i >= 0; i--)
        {
            if (numDistinctChampion >= _trait.breakpoint[i])
            {
                num = i;
                break;
            }
        }
        if (num == -1)
        {
            return;
        }
        this.championsGetBuff = championsGetBuff;
        this.breakpoint = num;
        if (_trait.haveTriggerTime)
        {
            triggerTimeLeft = _trait.triggerTimeCanChange == true ? _trait.triggerTime[num] : _trait.triggerTime[0];
        }
        if (_trait.haveStackUp)
        {
            maxStackUp = _trait.maxStackUpCanChange == true ? _trait.maxStackUp[num] : _trait.maxStackUp[0];
        }

        foreach (var i in championsGetBuff)
        {
            ChampionInfo1 chInfo = i.GetComponent<ChampionInfo1>();
            OnLaunch(chInfo, num);
        }
    }

    public virtual void OnLaunch(ChampionInfo1 chInfo, int breakpoint)
    {

    }
    public virtual void OnRemove(ChampionInfo1 chInfo)
    {

    }
    public virtual void OnTriggerBuff()
    {

    }
}

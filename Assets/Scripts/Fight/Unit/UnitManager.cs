using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using static UnitManagerSocketIO;

public class UnitManager : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private MoveManager movementManager;
    [SerializeField] private UnitInfo unitStat;

    [Header("Combat")]
    [SerializeField] private UnitTag unitTag;
    [SerializeField] private bool inCombat;

    [Header("Health And Mana")]
    [SerializeField] private int int_maxHealth;
    [SerializeField] private int int_maxMana;
    [SerializeField] private int int_currentHealth;
    [SerializeField] private int int_currentMana;

    [Header("OnPhotonSerializeView")]
    [SerializeField] private int other_maxHealth;
    [SerializeField] private int other_maxMana;
    [SerializeField] private int other_currentHealth;
    [SerializeField] private int other_currentMana;

    public UnitInfo _unitStat => unitStat;
    public UnitTag _unitTag => unitTag;
    public bool _inCombat => inCombat;

    public int _int_maxHealth => int_maxHealth;
    public int _int_maxMana => int_maxMana;
    public int _int_currentHealth => int_currentHealth;
    public int _int_currentMana => int_currentMana;

    private void Start()
    {
        movementManager = transform.GetComponentInChildren<MoveManager>();
        inCombat = false;
        int_maxHealth = (int)unitStat.currentLevel.hp;
        int_maxMana = (int)unitStat.maxMana;
        int_currentHealth = (int)unitStat.currentLevel.hp;
        int_currentMana = (int)unitStat.startMana;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (inCombat)
            {

            }
        }
        else
        {
            int_maxHealth = other_maxHealth;
            int_maxMana = other_maxMana;
            int_currentHealth = other_currentHealth;
            int_currentMana = other_currentMana;
        }
    }



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    public enum UnitTag
    {
        Player,
        Monster,
    }
}

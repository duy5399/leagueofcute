using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Tools.NetStats;
using UnityEngine;
using UnityEngine.UI;
using static PlayerDataInBattleSocketIO.PlayerDataJSON;
using static UnitManagerSocketIO;
using static UnitShopSocketIO;

public class UnitShopManager : MonoBehaviour
{
    public static UnitShopManager instance { get; private set; }

    [Header("Display Unit Can Buy")]
    [SerializeField] private List<Transform> list_tf_Unit = new List<Transform>();

    [Header("Rolling Chances")]
    [SerializeField] private List<TextMeshProUGUI> lst_RollingChances = new List<TextMeshProUGUI>();

    [Header("Gold")]
    [SerializeField] private TextMeshProUGUI text_Gold;

    [Header("Refresh Shop")]
    [SerializeField] private Button button_Refresh;
    
    [Header("Lock Shop")]
    [SerializeField] private Button button_Lock;
    [SerializeField] private Sprite sprite_Button_LockUnitShop;
    [SerializeField] private Sprite sprite_Button_OpenUnitShop;

    [Header("Champion Pool")]
    [SerializeField] private Transform tf_ChampionPool;

    [Header("Sell Unit")]
    [SerializeField] private Transform tf_SellUnit;
    [SerializeField] private TextMeshProUGUI text_SellPrice;

    [Header("Skill Unit")]
    [SerializeField] private Transform tf_UnitSkill;
    [SerializeField] private Image img_SkillIcon;
    [SerializeField] private TextMeshProUGUI text_SkillName;
    [SerializeField] private TextMeshProUGUI text_SkillDescription;

    private void Awake()
    {
        if(instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        ActiveSellUnit(false, null);
        ActiveSkillDescription(false, null, null, null);
    }

    public void SetNewUnitShop(UnitShop unitShop)
    {
        int i = 0;
        foreach(var unit in list_tf_Unit)
        {
            //if (!unit.gameObject.activeSelf)
            //{
            //    Debug.Log("!unit.gameObject.activeSelf");
            //    unit.gameObject.SetActive(true);
            //}
            switch (i)
            {
                case 0:
                    Debug.Log("unitShop: " + unitShop.slot0);
                    unit.GetComponent<UnitInTheShopManager>().LoadData("slot0", unitShop.slot0);
                    break;
                case 1:
                    Debug.Log("unitShop: " + unitShop.slot1);
                    unit.GetComponent<UnitInTheShopManager>().LoadData("slot1", unitShop.slot1);
                    break;
                case 2:
                    Debug.Log("unitShop: " + unitShop.slot2);
                    unit.GetComponent<UnitInTheShopManager>().LoadData("slot2", unitShop.slot2);
                    break;
                case 3:
                    Debug.Log("unitShop: " + unitShop.slot3);
                    unit.GetComponent<UnitInTheShopManager>().LoadData("slot3", unitShop.slot3);
                    break;
                case 4:
                    Debug.Log("unitShop: " + unitShop.slot4);
                    unit.GetComponent<UnitInTheShopManager>().LoadData("slot4", unitShop.slot4);
                    break;
            }
            unit.gameObject.SetActive(true);
            i++;
        }
    }

    public void SetGold(int gold)
    {
        text_Gold.text = gold.ToString();
    }

    public void SetRollingChances(RollingChances rollingChances)
    {
        lst_RollingChances[0].text = rollingChances.tier1.ToString() + "%";
        lst_RollingChances[1].text = rollingChances.tier2.ToString() + "%";
        lst_RollingChances[2].text = rollingChances.tier3.ToString() + "%";
        lst_RollingChances[3].text = rollingChances.tier4.ToString() + "%";
        lst_RollingChances[4].text = rollingChances.tier5.ToString() + "%";
    }

    public void SetLockUnitShop(bool isLock)
    {
        if (isLock)
            button_Lock.GetComponent<Image>().sprite = sprite_Button_LockUnitShop;
        else
            button_Lock.GetComponent<Image>().sprite = sprite_Button_OpenUnitShop;
    }


    public void BuyUnitSuccess(string slot)
    {
        switch (slot)
        {
            case "slot0":
                list_tf_Unit[0].gameObject.SetActive(false); break;
            case "slot1":
                list_tf_Unit[1].gameObject.SetActive(false); break;
            case "slot2":
                list_tf_Unit[2].gameObject.SetActive(false); break;
            case "slot3":
                list_tf_Unit[3].gameObject.SetActive(false); break;
            case "slot4":
                list_tf_Unit[4].gameObject.SetActive(false); break;
        }
        UnitShopManager.instance.ActiveSkillDescription(false, null, null, null);
    }


    public void ActiveSellUnit(bool isActive, UnitInfo unitStatJSON)
    {
        if (isActive)
            text_SellPrice.text = "Bán được " + unitStatJSON.currentLevel.sellPrice.ToString() + " vàng";
        tf_ChampionPool.gameObject.SetActive(!isActive);
        tf_SellUnit.gameObject.SetActive(isActive);
    }

    public void ActiveSkillDescription(bool isActive, Sprite skillIcon, string skillName, string skillDescription)
    {
        if (isActive)
        {
            img_SkillIcon.sprite = skillIcon;
            text_SkillName.text = skillName;
            text_SkillDescription.text = skillDescription;
        }
        tf_UnitSkill.gameObject.SetActive(isActive);
    }

    #region Event Button
    public void OnClick_RefreshUnitShop()
    {
        //SocketIO.instance.Emit_GetInstancePlayerStat();
        SocketIO.instance._unitShopSocketIO.Emit_RefreshUnitShop();
    }

    public void OnClick_LockUnitShop()
    {
        SocketIO.instance._unitShopSocketIO.Emit_LockUnitShop();
    }
    #endregion

    public enum SlotUnitShop
    {
        slot0 = 0,
        slot1 = 1,
        slot2 = 2,
        slot3 = 3,
        slot4 = 4,
    }
}

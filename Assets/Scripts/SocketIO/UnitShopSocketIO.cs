using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitManagerSocketIO;
using static PlayerDataInBattleSocketIO.PlayerDataJSON;
using UnityEngine.UIElements;
using static UnitShopManager;

[Serializable]
public class UnitShopSocketIO 
{
    private SocketManager socketManager;
    public void UnitShopSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<bool>("lock-unit-shop-success", (data) => {
            On_LockUnitShop(data);
        });
        socketManager.Socket.On<string>("refresh-unit-shop-success", (data) => {
            //Debug.Log(data);
            On_RefreshUnitShop(data);
        });
        socketManager.Socket.On<string>("refresh-unit-shop-fail", (data) => {
            Debug.Log(data);
        });
        socketManager.Socket.On<int>("update-gold", (gold) => {
            On_UpdateGold(gold);
        });
        socketManager.Socket.On<string>("update-rolling-chances", (chance) => {
            On_UpdateRollingChances(chance);
        });
        socketManager.Socket.On<string, string>("buy-unit-success", (data1, data2) => {
            On_BuyUnitSuccess(data1, data2);
        });
        socketManager.Socket.On<string>("buy-unit-fail", (data) => {
            Debug.Log("buy-unit-fail: " + data);
        });
        socketManager.Socket.On<string, string, string, string>("upgrade-unit-success", (slotUnitShop, position, slot, unit) => {
            On_OnUpgradeUnitSuccess(slotUnitShop, position, slot, unit);
        });
    }

    #region Listening to events
    private void On_LockUnitShop(bool data)
    {
        UnitShopManager.instance.SetLockUnitShop(data);
    }

    public void On_RefreshUnitShop(string data)
    {
        //Debug.Log("refresh-unit-shop-success");
        var newUnitShop = JsonConvert.DeserializeObject<UnitShop>(data);
        UnitShopManager.instance.SetNewUnitShop(newUnitShop);
    }
    private void On_UpdateGold(int gold)
    {
        UnitShopManager.instance.SetGold(gold);
    }

    private void On_UpdateRollingChances(string chance)
    {
        //Debug.Log("On_UpdateRollingChances: " + chance);
        RollingChances rollingChances = JsonConvert.DeserializeObject<RollingChances>(chance);
        UnitShopManager.instance.SetRollingChances(rollingChances);
    }

    public void On_BuyUnitSuccess(string data1, string data2)
    {
        //Debug.Log("On_BuyUnitSuccess" + data2);
        var newUnit = JsonConvert.DeserializeObject<SlotAndUnitStatJSON>(data2);
        UnitShopManager.instance.BuyUnitSuccess(data1);
        BenchManager.instance.AddNewUnit(newUnit);
    }

    public void On_OnUpgradeUnitSuccess(string slotUnitShop, string position, string slot, string unit)
    {
        Debug.Log("On_OnUpgradeUnitSuccess: " + slotUnitShop + " \n- " + position + " \n- " + slot + " \n- " + unit);
        UnitShopManager.instance.BuyUnitSuccess(slotUnitShop);
        var unitInfo = JsonConvert.DeserializeObject<UnitInfo>(unit);
        if(position == "Bench")
        {
            BenchManager.instance.UpgradeUnit(slot, unitInfo);
        }
        else
        {
            BattlefieldSideManager.instance.UpgradeUnit(slot, unitInfo);
        }
    }
    #endregion

    #region Emitting events
    public void Emit_LockUnitShop()
    {
        socketManager.Socket.Emit("lock-unit-shop");
    }

    public void Emit_RefreshUnitShop()
    {
        socketManager.Socket.Emit("refresh-unit-shop");
    }

    public void Emit_BuyUnit(SlotUnitShop slotUnitShop)
    {
        socketManager.Socket.Emit("buy-unit", slotUnitShop.ToString());
    }

    public void Emit_SellUnit(UnitInfo unitStatJSON)
    {
        socketManager.Socket.Emit("sell-unit", unitStatJSON);
    }
    #endregion
    public class SlotAndUnitStatJSON
    {
        public string slot;
        public UnitInfo unit;
    }
}

using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryClientSocketIO
{
    private SocketManager socketManager;
    [SerializeField] private UserInventoryJSON userInventory;
    public UserInventoryJSON _userInventory => userInventory;

    public void InventoryClientSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<string>("get-inventory", (data) => {
            On_GetInventory(data);
        });

        socketManager.Socket.On<string>("buy-item-success", (data) => {
            On_BuyItemSuccess(data);
        });

        socketManager.Socket.On<string>("buy-item-error", (data) => {
            On_BuyItemError(data);
        });
    }

    #region Listening to events
    private void On_GetInventory(string data)
    {
        userInventory = JsonConvert.DeserializeObject<UserInventoryJSON>(data);
        try
        {
            InfoManager.instance.SetMyCurrencies(userInventory.gold, userInventory.crystal);
            StoreManager.instance.SetCurrencies(userInventory.gold, userInventory.crystal);
        }
        catch
        {
            Debug.Log("SetCurrencies error");
        }
    }

    private void On_BuyItemSuccess(string data)
    {
        Debug.Log(data);
    }

    private void On_BuyItemError(string data)
    {
        Debug.Log(data);
    }
    #endregion

    #region Emitting events
    public void Emit_EquipItemClient(string itemID, string itemClass)
    {
        socketManager.Socket.Emit("equip-item-client", itemID, itemClass);
        Debug.Log("Emit_EquipItemClient");
    }
    #endregion

    [Serializable]
    public class UserInventoryJSON
    {
        public int gold;
        public int crystal;
        public string[] tacticians;
        public string[] arenaSkins;
        public string[] booms;
        public string tacticianEquip;
        public string arenaSkinEquip;
        public string boomEquip;
    }
}

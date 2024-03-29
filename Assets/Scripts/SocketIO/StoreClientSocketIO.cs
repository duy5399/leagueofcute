using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StoreClientSocketIO
{
    private SocketManager socketManager;
    [SerializeField] private ItemInStoreJSON[] tacticians;
    [SerializeField] private ItemInStoreJSON[] arenaSkins;
    [SerializeField] private ItemInStoreJSON[] booms;
    public ItemInStoreJSON[] _tacticians => tacticians;
    public ItemInStoreJSON[] _arenaSkins => arenaSkins;
    public ItemInStoreJSON[] _booms => booms;

    public void StoreClientSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<string, string, string>("get-store", (data1, data2, data3) => {
            On_GetStore(data1, data2, data3);
        });
    }

    #region Listening to events
    private void On_GetStore(string dataTacticians, string dataArenaSkins, string dataBooms)
    {
        tacticians = JsonConvert.DeserializeObject<ItemInStoreJSON[]>(dataTacticians);
        arenaSkins = JsonConvert.DeserializeObject<ItemInStoreJSON[]>(dataArenaSkins);
        booms = JsonConvert.DeserializeObject<ItemInStoreJSON[]>(dataBooms);
    }
    #endregion

    #region Emitting events
    public void Emit_BuyItem(string itemId, string itemClass)
    {
        socketManager.Socket.Emit("buy-item", itemId, itemClass);
    }
    #endregion

    [Serializable]
    public class ItemInStoreJSON
    {
        public string itemID;
        public string itemClass;
        public string displayName;
        public string displayImage;
        public Price price;

        [Serializable]
        public class Price
        {
            public string currency;
            public int amount;
        }
    }
}

using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnitManagerSocketIO;

[Serializable]
public class ItemsSocketIO
{
    private SocketManager socketManager;
    public void ItemsSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<string>("get-item-drop", (data) => {
            Debug.Log("get-item-drop: " + data);
            On_GetItemDrop(data);
        });
        socketManager.Socket.On<string, string, string>("combine-item-success", (data1, data2, data3) => {
            On_GetCombineSuccess(data1, data2, data3);
        });
        socketManager.Socket.On<bool>("reset-items-on-unit", (data) => {
            Debug.Log("reset-items-on-unit");
            On_ResetItemOnUnit(data);
        });
    }
    #region Listening to events
    private void On_GetItemDrop(string itemJSON)
    {
        var item = JsonConvert.DeserializeObject<Item>(itemJSON);
        Debug.Log("get-item-drop: " + item.idItem + " - " + item.name + " + " + item.descriptionStat);
        GameObject itemDrop = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/items/", item.idItem), new Vector3(0, 1.2f,0), Quaternion.identity);
        //itemDrop.name = item.idItem;
        itemDrop.GetComponent<ItemBase>().item = item;
    }
    private void On_GetCombineSuccess(string unitStatJSON, string itemJSON, string itemCombineJSON)
    {
        var unitStat = JsonConvert.DeserializeObject<UnitInfo>(unitStatJSON);
        var item = JsonConvert.DeserializeObject<Item>(itemJSON);
        var itemCombine = JsonConvert.DeserializeObject<Item>(itemCombineJSON);
        Debug.Log("combine-item-success: " + itemCombine.idItem + " - " + itemCombine.name + " + " + itemCombine.descriptionStat);
        List<GameObject> listUnit = new List<GameObject>();
        listUnit.AddRange(BattlefieldSideManager.instance.dict_BattlefieldSide.Values);
        listUnit.AddRange(BenchManager.instance._dict_Bench.Values);
        foreach (var i in listUnit)
        {
            if (i != null)
            {
                Debug.Log("i != null");
                ChampionInfo1 chInfo = i.GetComponent<ChampionInfo1>();
                if (chInfo.chStat._id == unitStat._id && chInfo.chStat.championName == unitStat.championName)
                {
                    Debug.Log("chInfo.chStat._id == unitStat._id && chInfo.chStat.championName == unitStat.championName)");
                    GameObject obj = chInfo.items.itemLst.FirstOrDefault(x => x.GetComponent<ItemBase>().item._id == item._id && x.GetComponent<ItemBase>().item.idItem == item.idItem);
                    if(obj != null)
                    {
                        Debug.Log("obj != null");
                        GameObject newItem = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/items/", itemCombine.idItem), obj.transform.localPosition, obj.transform.localRotation);
                        chInfo.items.RemoveItem(obj);
                        if (obj.GetPhotonView().IsMine)
                        {
                            PhotonNetwork.Destroy(obj.GetComponent<PhotonView>());
                        }
                        newItem.name = itemCombine.idItem;
                        newItem.GetComponent<ItemBase>().item = itemCombine;
                        newItem.GetComponent<ItemDragDrop>().selectingDropChampion = i.transform;
                        newItem.GetComponent<ItemDragDrop>().TryOnEquip();
                        //newItem.transform.parent = obj.transform.parent;
                        //newItem.transform.localPosition = obj.transform.localPosition;
                        //newItem.transform.localRotation = obj.transform.localRotation;
                        //newItem.transform.localScale = obj.transform.localScale;

                        //Debug.Log("obj != null");
                        //obj.GetComponent<ItemBase>().item = itemCombine;
                        //var materialsCopy = obj.GetComponent<Renderer>().materials;
                        //Material newMaterial = Resources.Load<Material>("materials/items/" + item.icon);
                        //materialsCopy[1] = newMaterial;
                        //obj.GetComponent<Renderer>().materials = materialsCopy;
                        break;
                    }
                }
            }
        }
    }

    private void On_ResetItemOnUnit(bool data)
    {
        if (data)
        {
            List<GameObject> listUnit = new List<GameObject>();
            listUnit.AddRange(BattlefieldSideManager.instance.dict_BattlefieldSide.Values);
            listUnit.AddRange(BenchManager.instance._dict_Bench.Values);
            foreach (var i in listUnit)
            {
                if (i != null)
                {
                    Debug.Log("On_ResetItemOnUnit i != null: " + i.name);
                    ChampionInfo1 chInfo = i.GetComponent<ChampionInfo1>();
                    if (chInfo != null && chInfo.items != null)
                    {
                        foreach (var item in chInfo.items.itemLst)
                        {
                            if (item != null)
                            {
                                item.GetComponent<ItemBase>().OnReset();
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion
    #region Emitting events
    public void Emit_CombineItem(UnitInfo unit, Item item1, Item item2)
    {
        socketManager.Socket.Emit("combine-item", unit, item1, item2);
    }
    #endregion
}

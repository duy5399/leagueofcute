using AYellowpaper.SerializedCollections;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnitManagerSocketIO;
using static UnitShopSocketIO;

public class BenchManager : MonoBehaviourPun
{
    public static BenchManager instance { get; private set; }

    [Header("Bench")]
    [SerializedDictionary("Slot", "Unit")]
    [SerializeField] private SerializedDictionary<GameObject, GameObject> dict_Bench;
    [SerializeField] private GameObject prefab_BenchSlot;
    [SerializeField] private const int MAX_ROWS = 1;
    [SerializeField] private const int MAX_COLS = 9;
    [SerializeField] private const float SPACE_SLOT = 2.1f;
    [SerializeField] private const float START_POINT = -9.13f;

    public SerializedDictionary<GameObject, GameObject> _dict_Bench => dict_Bench;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        CreateBench();
    }

    private void CreateBench()
    {
        for(int x = 0; x< MAX_ROWS; x++)
        {
            for(int y = 0; y < MAX_COLS; y++)
            {
                GameObject benchSlot = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/arenas", prefab_BenchSlot.name), prefab_BenchSlot.transform.position, Quaternion.identity);
                benchSlot.GetComponent<SlotManager>().SetNodeName("slot" + x + "_" + y);
                benchSlot.GetComponent<SlotManager>().SetNode(x,y);
                benchSlot.GetComponent<SlotManager>().SetName("slot" + x + "_" + y);
                benchSlot.GetComponent<SlotManager>().SetParent(gameObject.GetPhotonView().ViewID);
                benchSlot.GetComponent<SlotManager>().SetActive(false);
                benchSlot.transform.localPosition = new Vector3(START_POINT + y * SPACE_SLOT, benchSlot.transform.position.y, benchSlot.transform.position.z);
                dict_Bench.Add(benchSlot, null);
            }
        }
    }
    //public void SetActiveBench(bool boolean)
    //{
    //    foreach (var x in dict_Bench)
    //    {
    //        if (x.Key.tag == "BattlefieldSide" && x.Key.gameObject.layer == LayerMask.NameToLayer("DropArea"))
    //            x.Key.GetComponent<MeshRenderer>().enabled = boolean;
    //        else
    //            x.Key.GetComponent<MeshRenderer>().enabled = false;
    //    }
    //}

    public void SetActiveBench(bool boolean)
    {
        foreach (var x in dict_Bench)
            x.Key.GetComponent<SlotManager>().RPC_SetActive(boolean);
    }

    public void ActiveSelectDropPosition(GameObject selectDrop)
    {
        foreach (var x in dict_Bench)
        {
            if (x.Key == selectDrop)
                x.Key.GetComponent<SlotManager>().ActiveBorder(true);
            else
                x.Key.GetComponent<SlotManager>().ActiveBorder(false);
        }
    }

    public void AddNewUnit(SlotAndUnitStatJSON newUnit)
    {
        GameObject slotBench = dict_Bench.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._nodeName == newUnit.slot).Key;
        if (slotBench != null && dict_Bench[slotBench] == null)
        {
            GameObject obj = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/units", newUnit.unit.championName), slotBench.transform.position, Quaternion.identity);
            obj.GetComponent<ChampionInfo1>().chStat = newUnit.unit;
            obj.GetComponent<ChampionInfo1>().chCategory = ChampionInfo1.Categories.Hero;
            obj.GetComponent<ChampionInfo1>().dragDrop.currentParent = slotBench.transform;
            obj.GetComponent<ChampionInfo1>().SetName(newUnit.unit.championName + "_" + newUnit.unit._id);
            obj.GetComponent<ChampionInfo1>().SetParent(slotBench.GetPhotonView().ViewID);
            Debug.Log("InitState AddNewUnit");
            obj.GetComponent<ChampionInfo1>().currentState.InitState();
            dict_Bench[slotBench] = obj;
        }
    }

    public void RemoveUnit(string slot, UnitInfo unitInfo)
    {
        GameObject slotBench = dict_Bench.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._nodeName == slot).Key;
        if (slotBench != null && dict_Bench[slotBench].GetComponent<ChampionBase>().info.chStat._id == unitInfo._id)
        {
            if (dict_Bench[slotBench].GetPhotonView().IsMine)
            {
                ItemManager itemManager = dict_Bench[slotBench].GetComponent<ChampionInfo1>().items;
                if (itemManager)
                {
                    List<GameObject> itemLst = itemManager.itemLst;
                    for(int i = 0; i< itemLst.Count; i++)
                    {
                        Debug.Log("itemLst: " + itemLst[0].name);
                        ItemBase itemBase = itemLst[0].GetComponent<ItemBase>();
                        ItemDragDrop itemDragDrop = itemLst[0].GetComponent<ItemDragDrop>();
                        if (itemBase && itemDragDrop)
                        {
                            Debug.Log("Remove Item When Sell Unit: itemBase = true && itemDragDrop = true " + dict_Bench[slotBench].name);
                            itemManager.RemoveItem(itemLst[0]);
                            itemDragDrop.currentParent = null;
                            itemDragDrop.MoveToStorage();
                        }
                    }
                    //foreach (var item in itemLst)
                    //{
                    //    ItemBase itemBase = item.GetComponent<ItemBase>();
                    //    ItemDragDrop itemDragDrop = item.GetComponent<ItemDragDrop>();
                    //    if (itemBase && itemDragDrop)
                    //    {
                    //        Debug.Log("Remove Item When Sell Unit: itemBase = true && itemDragDrop = true " + dict_Bench[slotBench].name);
                    //        itemManager.RemoveItem(item);
                    //        itemDragDrop.currentParent = null;
                    //        itemDragDrop.MoveToStorage();
                    //    }
                    //}
                }
                PhotonNetwork.Destroy(dict_Bench[slotBench].GetComponent<PhotonView>());
                dict_Bench[slotBench] = null;
            }
        }
    }

    public void UpgradeUnit(string slot, UnitInfo upgradeUnit)
    {
        GameObject slotBench = dict_Bench.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._nodeName == slot).Key;
        if (slotBench != null)
        {
            ChampionInfo1 chInfo = dict_Bench[slotBench].GetComponent<ChampionInfo1>();
            if (chInfo != null && chInfo.chStat._id == upgradeUnit._id)
            {
                chInfo.chStat = upgradeUnit;
                chInfo.currentState.hp = chInfo.currentState.maxHP;
                chInfo.SetImageStar(upgradeUnit.currentLevel.star);
                var prefab_UpgradeUnitSuccess = Resources.Load<GameObject>("prefabs/vfx/vfx_MagicAbility_Electric");
                GameObject obj = PhotonNetwork.Instantiate(Path.Combine("prefabs/vfx", prefab_UpgradeUnitSuccess.name), slotBench.transform.position, Quaternion.identity);
            }
        }
    }

    public void SetDictBench(GameObject slotBench, GameObject unit)
    {
        dict_Bench[slotBench] = unit;
    }

    public void MoveBench(int viewIDParent)
    {
        transform.parent.GetComponent<PlayerPosManager>().ChangeParent(gameObject.GetPhotonView().ViewID, viewIDParent);
    }

    //---------------
    public void SetParent(int photonviewParent)
    {
        photonView.RPC(nameof(RPC_SetParent), RpcTarget.AllBuffered, photonviewParent);
    }

    public void SetLocalPosition(Vector3 newPosition)
    {
        transform.localPosition = newPosition;
    }

    public void SetLocalRotation(Vector3 newRotation)
    {
        transform.localRotation = Quaternion.Euler(newRotation.x, newRotation.y, newRotation.z);
    }

    [PunRPC]
    public void RPC_SetParent(int viewID)
    {
        Transform _parent = PhotonView.Find(viewID).transform;
        transform.parent = _parent;
    }
}

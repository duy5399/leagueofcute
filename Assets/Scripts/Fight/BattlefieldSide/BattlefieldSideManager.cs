using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper;
using AYellowpaper.SerializedCollections;
using static UnitManagerSocketIO;
using Photon.Pun;
using System.IO;
using System;
using Unity.VisualScripting;
using static UnitShopSocketIO;

public class BattlefieldSideManager : MonoBehaviourPun
{
    public static BattlefieldSideManager instance { get; private set; }

    [Header("Battlefield")]
    [SerializedDictionary("Slot", "Unit")]
    [SerializeField] private SerializedDictionary<GameObject, GameObject> _dict_BattlefieldSide;
    [SerializeField] private List<GameObject> _lstCurrentFormation;
    [SerializeField] private GameObject prefab_HexTile;
    [SerializeField] private const int MAX_ROWS = 6;
    [SerializeField] private const int MAX_COLS = 6;
    [SerializeField] private const float TILE_X_OFFSET = 2.8f;
    [SerializeField] private const float TILE_Y_OFFSET = 2.45f;

    public SerializedDictionary<GameObject, GameObject> dict_BattlefieldSide
    {
        get { return _dict_BattlefieldSide; }
        set { _dict_BattlefieldSide = value; }
    }
    public List<GameObject> lstCurrentFormation
    {
        get { return _lstCurrentFormation; }
        set { _lstCurrentFormation = value; }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        _lstCurrentFormation = new List<GameObject>();
    }

    private void Start()
    {
        CreateBattlefield();
    }

    private void CreateBattlefield()
    {
        for (int x = 0; x < MAX_ROWS; x++)
        {
            for (int y = 0; y < MAX_COLS; y++)
            {
                GameObject battlefieldSlot = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/arenas", prefab_HexTile.name), prefab_HexTile.transform.position, Quaternion.identity);
                battlefieldSlot.GetComponent<SlotManager>().SetNodeName("slot" + x + "_" + y);
                battlefieldSlot.GetComponent<SlotManager>().SetNode(x, y);
                battlefieldSlot.GetComponent<SlotManager>().SetName("slot" + x + "_" + y);
                battlefieldSlot.GetComponent<SlotManager>().SetParent(gameObject.GetPhotonView().ViewID);
                battlefieldSlot.GetComponent<SlotManager>().SetActive(false);
                if (x >= MAX_ROWS / 2)
                {
                    int layerIgnoreRaycast = LayerMask.NameToLayer("Default");
                    battlefieldSlot.layer = layerIgnoreRaycast;
                    battlefieldSlot.tag = "Untagged";
                }
                if (x % 2 == 0)
                    battlefieldSlot.transform.localPosition = new Vector3(y * TILE_X_OFFSET, 1, x * TILE_Y_OFFSET);
                else
                    battlefieldSlot.transform.localPosition = new Vector3(y * TILE_X_OFFSET - (TILE_X_OFFSET / 2), 1, x * TILE_Y_OFFSET);
                _dict_BattlefieldSide.Add(battlefieldSlot, null);
            }
        }
    }

    public void SetActiveBattlefieldSide(bool boolean)
    {
        foreach (var x in _dict_BattlefieldSide)
        {
            if (x.Key.tag == "Battlefield" && x.Key.gameObject.layer == LayerMask.NameToLayer("DropArea"))
                x.Key.GetComponent<SlotManager>().RPC_SetActive(boolean); ;
        }
    }

    public void ActiveSelectDropPosition(GameObject selectDrop)
    {
        foreach (var x in _dict_BattlefieldSide)
        {
            if (x.Key == selectDrop)
                x.Key.GetComponent<SlotManager>().ActiveBorder(true);
            else
                x.Key.GetComponent<SlotManager>().ActiveBorder(false);
        }
    }
    public void RemoveUnit(string slot, UnitInfo unitInfo)
    {
        GameObject slotBench = _dict_BattlefieldSide.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._nodeName == slot).Key;
        if (slotBench != null && _dict_BattlefieldSide[slotBench].GetComponent<ChampionBase>().info.chStat._id == unitInfo._id)
        {
            if (_dict_BattlefieldSide[slotBench].GetPhotonView().IsMine)
            {
                TraitsManager.instance.RemoveClass(_dict_BattlefieldSide[slotBench].GetComponent<ChampionInfo1>().info.chStat.idClass, _dict_BattlefieldSide[slotBench]);
                TraitsManager.instance.RemoveOrigin(_dict_BattlefieldSide[slotBench].GetComponent<ChampionInfo1>().info.chStat.idOrigin, _dict_BattlefieldSide[slotBench]);
                PhotonNetwork.Destroy(_dict_BattlefieldSide[slotBench].GetComponent<PhotonView>());
            }
            _dict_BattlefieldSide[slotBench] = null;
        }
    }

    public void UpgradeUnit(SlotAndUnitStatJSON upgradeUnit)
    {
        GameObject slotBench = _dict_BattlefieldSide.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._nodeName == upgradeUnit.slot).Key;
        if (slotBench != null && _dict_BattlefieldSide[slotBench].GetComponent<ChampionBase>().info.chStat._id == upgradeUnit.unit._id)
        {
            _dict_BattlefieldSide[slotBench].GetComponent<ChampionInfo1>().chStat = upgradeUnit.unit;
            _dict_BattlefieldSide[slotBench].GetComponent<ChampionBase>().SetImageStar(upgradeUnit.unit.currentLevel.star);
            var prefab_UpgradeUnitSuccess = Resources.Load<GameObject>("prefabs/vfx/vfx_MagicAbility_Electric");
            GameObject obj = PhotonNetwork.Instantiate(Path.Combine("prefabs/vfx", prefab_UpgradeUnitSuccess.name), slotBench.transform.position, Quaternion.identity);
        }
    }

    public void UpgradeUnit(string slot, UnitInfo upgradeUnit)
    {
        GameObject slotBattlefield = dict_BattlefieldSide.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._nodeName == slot).Key;
        if (slotBattlefield != null)
        {
            ChampionInfo1 chInfo = dict_BattlefieldSide[slotBattlefield].GetComponent<ChampionInfo1>();
            if (chInfo != null && chInfo.chStat._id == upgradeUnit._id)
            {
                chInfo.chStat = upgradeUnit;
                chInfo.currentState.hp = chInfo.currentState.maxHP;
                chInfo.SetImageStar(upgradeUnit.currentLevel.star);
                var prefab_UpgradeUnitSuccess = Resources.Load<GameObject>("prefabs/vfx/vfx_MagicAbility_Electric");
                GameObject obj = PhotonNetwork.Instantiate(Path.Combine("prefabs/vfx", prefab_UpgradeUnitSuccess.name), slotBattlefield.transform.position, Quaternion.identity);
            }
        }
    }

    public void SetDictBattlefield(GameObject slotBattlefield, GameObject unit)
    {
        _dict_BattlefieldSide[slotBattlefield] = unit;
    }

    public void MoveBattlefield(int viewIDParent)
    {
        transform.parent.GetComponent<PlayerPosManager>().ChangeParent(gameObject.GetPhotonView().ViewID, viewIDParent);
    }
    //----------------
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
    void RPC_SetParent(int viewID)
    {
        Transform _parent = PhotonView.Find(viewID).transform;
        transform.parent = _parent;
    }
}

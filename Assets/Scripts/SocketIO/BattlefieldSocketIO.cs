using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static UnitManagerSocketIO;
using static UnitShopSocketIO;
using System.Linq;
using static Cinemachine.CinemachineTargetGroup;
using System.ComponentModel;

[Serializable]
public class BattlefieldSocketIO
{
    private SocketManager socketManager;

    [SerializeField] private BattlefieldName _battlefieldName;

    public BattlefieldName battlefieldName => _battlefieldName;
    public void BattlefieldSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<string, string>("remove-unit-on-battlefield", (data1, data2) => {
            On_RemoveUnitOnBattlefield(data1, data2);
        });
        socketManager.Socket.On<string>("set-battlefield", (data) => {
            On_SetBattlefield(data);
        });
        socketManager.Socket.On<string>("arrival-to-opponent", (username) => {
            On_ArrivalToOpponent(username);
        });
        socketManager.Socket.On("back-to-home", () => {
            On_BackToHome();
        });
        socketManager.Socket.On<bool>("reset-battlefield", (data) => {
            On_ResetBattlefield(data);
        });
    }

    #region Listening to events
    void On_RemoveUnitOnBattlefield(string _slot, string _unitInfo)
    {
        Debug.Log("On_RemoveUnitOnBench: " + _slot + " - " + _unitInfo);
        UnitInfo unitInfo = JsonConvert.DeserializeObject<UnitInfo>(_unitInfo);
        BattlefieldSideManager.instance.RemoveUnit(_slot, unitInfo);
    }

    private void On_SetBattlefield(string data)
    {
        switch (data)
        {
            case "Battlefield_0":
                _battlefieldName = BattlefieldName.Battlefield_0;
                break;
            case "Battlefield_1":
                _battlefieldName = BattlefieldName.Battlefield_1;
                break;
            case "Battlefield_2":
                _battlefieldName = BattlefieldName.Battlefield_2;
                break;
            case "Battlefield_3":
                _battlefieldName = BattlefieldName.Battlefield_3;
                break;
            case "Battlefield_4":
                _battlefieldName = BattlefieldName.Battlefield_4;
                break;
            case "Battlefield_5":
                _battlefieldName = BattlefieldName.Battlefield_5;
                break;
            case "Battlefield_6":
                _battlefieldName = BattlefieldName.Battlefield_6;
                break;
            case "Battlefield_7":
                _battlefieldName = BattlefieldName.Battlefield_7;
                break;
        }
    }
    private void On_ArrivalToOpponent(string username)
    {
        Debug.Log("On_ArrivalToOpponent: " + username);
        GameObject away = GameObject.Find("Player_" + username);
        BenchManager.instance.MoveBench(away.GetPhotonView().ViewID);
        BenchManager.instance.SetLocalPosition(new Vector3(0, 0, -3.1f));
        BenchManager.instance.SetLocalRotation(new Vector3(0, 180, 0));
        BattlefieldSideManager.instance.MoveBattlefield(away.GetPhotonView().ViewID);
        BattlefieldSideManager.instance.SetLocalPosition(new Vector3(6.23f, 0, 4.55f));
        BattlefieldSideManager.instance.SetLocalRotation(new Vector3(0, 180, 0));
        ItemsDropManager.instance.MoveItemsDropManager(away.GetPhotonView().ViewID);
        ItemsDropManager.instance.SetLocalPosition(new Vector3(0, 0, 13));
        ItemsDropManager.instance.SetLocalRotation(new Vector3(0, 180, 0));
        RoomManager.instance.myTactician.GetComponent<PetManager>().MoveTactician(away.GetPhotonView().ViewID);
        RoomManager.instance.myTactician.GetComponent<PetManager>().SetLocalPosition(new Vector3(9, 2, 6));
        RoomManager.instance.myTactician.GetComponent<PetManager>().SetLocalRotation(new Vector3(0, 0, 0));
        CameraController.instance.SetCameraAway(away.transform.position);
        //BattlefieldSideManager.instance.SetOpponent(data);
    }

    private void On_BackToHome()
    {
        Debug.Log("On_BackTohome: ");
        GameObject home = RoomManager.instance.playerPos;
        BenchManager.instance.MoveBench(home.GetPhotonView().ViewID);
        BenchManager.instance.SetLocalPosition(Vector3.zero);
        BenchManager.instance.SetLocalRotation(new Vector3(0, 0, 0));
        BattlefieldSideManager.instance.MoveBattlefield(home.GetPhotonView().ViewID);
        BattlefieldSideManager.instance.SetLocalPosition(new Vector3(-6.36f, 0, -7.7f));
        BattlefieldSideManager.instance.SetLocalRotation(new Vector3(0, 0, 0));
        ItemsDropManager.instance.MoveItemsDropManager(home.GetPhotonView().ViewID);
        ItemsDropManager.instance.SetLocalPosition(new Vector3(0, 0, -15.8f));
        ItemsDropManager.instance.SetLocalRotation(new Vector3(0, 0, 0));
        RoomManager.instance.myTactician.GetComponent<PetManager>().MoveTactician(home.GetPhotonView().ViewID);
        RoomManager.instance.myTactician.GetComponent<PetManager>().SetLocalPosition(new Vector3(-9, 2, -9));
        RoomManager.instance.myTactician.GetComponent<PetManager>().SetLocalRotation(new Vector3(0, 0, 0));
        CameraController.instance.SetCameraHome(home.transform.position);
        //BattlefieldSideManager.instance.SetOpponent(data);
    }
    private void On_ResetBattlefield(bool data)
    {
        if (data)
        {
            RoomManager.instance.ClearMonster();
            Dictionary<int[], UnitInfo> fomation = new Dictionary<int[], UnitInfo>();
            try
            {
                fomation = SocketIO.instance.playerDataInBattleSocketIO.playerData._battlefield.Fomation();
            }
            catch
            {
            
            }
            if(fomation.Count > 0)
            {
                foreach(var i in fomation)
                {
                    GameObject champion = BattlefieldSideManager.instance.lstCurrentFormation.FirstOrDefault(x => x != null && x.GetComponent<ChampionInfo1>().info.chStat._id == i.Value._id);
                    if (champion != null)
                    {
                        ChampionInfo1 chInfo = champion.GetComponent<ChampionInfo1>();
                        if(chInfo != null)
                        {
                            Debug.Log("On_ResetBattlefield: " + chInfo.name);
                            GameObject currentCell = BattlefieldSideManager.instance.dict_BattlefieldSide.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._node.SequenceEqual(chInfo.moveManager.positionNode)).Key;
                            GameObject newCell = BattlefieldSideManager.instance.dict_BattlefieldSide.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._node.SequenceEqual(i.Key)).Key;
                            BattlefieldSideManager.instance.dict_BattlefieldSide[currentCell] = null;
                            BattlefieldSideManager.instance.dict_BattlefieldSide[newCell] = champion;
                            chInfo.SetParent(newCell.gameObject.GetPhotonView().ViewID);
                            champion.transform.position = newCell.transform.position;
                            champion.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
                            chInfo.dragDrop.currentParent = newCell.transform;
                            chInfo.currentState.InitState();
                            chInfo.stateCtrl.InitStateCtrl();
                            chInfo.skills.currentCasting = null;
                            chInfo.moveManager.positionNode = newCell.GetComponent<SlotManager>()._node;
                            chInfo.moveManager.moveState = MoveManager.State.StandStill;
                            chInfo.moveManager.locked = false;
                            chInfo.items.OnReset();
                            chInfo.SetActive(true);
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region Emitting events

    #endregion

    [Serializable]
    public class StageJSON
    {
        public string username;
        public string profileImage;
        public string rank;
        public int points;
        public string tacticianEquip;
    }
}
public enum BattlefieldName
{
    Battlefield_0 = 0,
    Battlefield_1 = 1,
    Battlefield_2 = 2,
    Battlefield_3 = 3,
    Battlefield_4 = 4,
    Battlefield_5 = 5,
    Battlefield_6 = 6,
    Battlefield_7 = 7,
}
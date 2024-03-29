using BestHTTP.SocketIO3;
using ExitGames.Client.Photon.StructWrapping;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnitManagerSocketIO;

[Serializable]
public class StageBattleSocketIO
{
    private SocketManager socketManager;

    [SerializeField] private string _opponent;
    [SerializeField] private List<UnitInfo> monsterList = new List<UnitInfo>();
    [SerializeField] private List<Vector2> monsterPositionList = new List<Vector2>();

    public string opponent
    {
        get { return _opponent; }
        set { _opponent = value; }
    }

    public void StageBattleSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<int[]>("countdownTimer", (data) => {
            On_CountdownTimer(data[0], data[1]);
        });
        socketManager.Socket.On<int[]>("new-round", (data) => {
            On_NewRound(data);
        });
        socketManager.Socket.On<string[][]>("round-PvE", (data) => {
            On_RoundPvE(data);
        });
        socketManager.Socket.On<string>("change-phase", (data) => {
            On_ChangePhase(data);
        });
        socketManager.Socket.On<string>("set-opponent", (data) => {
            On_SetOpponent(data);
        });
        socketManager.Socket.On("set-start-combat", () => {
            On_StartCombat();
        });
        socketManager.Socket.On("set-end-combat", () => {
            On_EndCombat();
        });
        socketManager.Socket.On<string>("battle-result", (data) => {
            On_BattleResult(data);
        });
    }

    #region Listening to events
    private void On_NewRound(int[] data)
    {
        StageTrackerManager.instance.SetTextStage(data[0] + "-" + data[1]);
    }
    private void On_RoundPvE(string[][] data)
    {
        monsterList.Clear();
        monsterPositionList.Clear();
        for(int i = 0; i< data[0].Length; i++)
        {
            var monster = JsonConvert.DeserializeObject<UnitInfo>(data[0][i]);
            monsterList.Add(monster);
            string[] arrListStr = data[1][i].Split(new char[] { ',' });
            int[] monsterPosition = { int.Parse(arrListStr[0]), int.Parse(arrListStr[1]) };
            Item itemDrop = data[2][i] != null ? JsonConvert.DeserializeObject<Item>(data[2][i]) : null;
            string coinDrop = data[3][i] != null ? "coin" : null;
            //Debug.Log("monsterPosition: " + monsterPosition[0] + ","+ monsterPosition[1]);
            Debug.Log("itemDrop: " + data[2][i]);
            RoomManager.instance.InstantiateMonster(monster, monsterPosition, itemDrop, coinDrop);
        }
    }
    private void On_CountdownTimer(int data1, int data2)
    {
        StageTrackerManager.instance.SetTextCountdownTimer(data1);
        StageTrackerManager.instance.SetSliderCountdownTimer(data1, data2);
    }
    private void On_ChangePhase(string data)
    {
        Debug.Log("On_ChangePhase: " + data);
        string newPhase = "";
        switch (data)
        {
            case "PlanningPhase":
                newPhase = "Dàn Trận"; break;
            case "CombatPhase":
                newPhase = "Giao Chiến"; break;
        }
        StageTrackerManager.instance.SetNewPhase(newPhase);
    }
    private void On_SetOpponent(string data)
    {
        Debug.Log("On_SetOpponent: " + data);
        opponent = data;
    }
    private void On_StartCombat()
    {
        //Debug.Log("On_StartCombat: ");
        try
        {
            foreach(var monster in RoomManager.instance.monstersLst)
            {
                monster.GetComponent<ChampionInfo1>().info.stateCtrl.inCombat = true;
            }
            foreach(var unit in BattlefieldSideManager.instance.dict_BattlefieldSide.Values)
            {
                if(unit != null)
                    unit.GetComponent<ChampionInfo1>().info.stateCtrl.inCombat = true;
            }
        }
        catch
        {
            Debug.Log("Error On_StartCombat on monster");
        }
    }
    private void On_EndCombat()
    {
        Debug.Log("On_EndCombat: ");
        try
        {
            foreach (var monster in RoomManager.instance.monstersLst)
            {
                monster.GetComponent<PhotonView>().Synchronization = ViewSynchronization.Off;
                ChampionInfo1 monsterInfo = monster.GetComponent<ChampionInfo1>();
                if (monsterInfo)
                {
                    if (monsterInfo.skills)
                    {
                        monsterInfo.skills.target = null;
                        monsterInfo.skills.ForceInterrupt();
                    }
                }
                if (monsterInfo.moveManager)
                {
                    monsterInfo.moveManager.locked = true;
                    monsterInfo.moveManager.SetNearestTarget(null);
                }
                if (monsterInfo.stateCtrl)
                {
                    monsterInfo.stateCtrl.inCombat = false;
                }
            }
            foreach (var unit in BattlefieldSideManager.instance.dict_BattlefieldSide.Values)
            {
                if (unit != null)
                {
                    ChampionInfo1 chInfo = unit.GetComponent<ChampionInfo1>();
                    if (chInfo)
                    {
                        if (chInfo.stateCtrl)
                        {
                            chInfo.stateCtrl.inCombat = false;
                        }
                        if (chInfo.skills != null)
                        {
                            chInfo.skills.target = null;
                            //chInfo.skills.ForceInterrupt();
                        }
                        if (chInfo.moveManager != null)
                        {
                            chInfo.moveManager.locked = true;
                            chInfo.moveManager.SetNearestTarget(null);
                        }
                        if (chInfo.buffs)
                        {
                            Transform buffManager = chInfo.info.buffs.transform;
                            //for (int i = 0; i < buffManager.childCount - 1; i++)
                            //{
                            //    GameObject buff = buffManager.GetChild(i).gameObject;
                            //    //buff.SetActive(false);
                            //    if (buff.GetComponent<PhotonView>().IsMine)
                            //    {
                            //        PhotonNetwork.Destroy(buff.GetComponent<PhotonView>());
                            //    }
                            //}
                            chInfo.buffs.DestoryAllBuffs();
                        }
                        if (chInfo.info.weakness)
                        {
                            Transform weakness = chInfo.info.weakness.transform;
                            for (int i = 0; i < weakness.childCount - 1; i++)
                            {
                                GameObject obj = weakness.GetChild(i).gameObject;
                                if (!obj.GetComponent<FloatingTextPopupManager>())
                                {
                                    if (obj.GetPhotonView().IsMine)
                                    {
                                        PhotonNetwork.Destroy(obj.GetComponent<PhotonView>());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            Debug.Log("Error On_EndCombat on monster");
        }
    }
    private void On_BattleResult(string data)
    {
        Debug.Log("On_BattleResult: " + data);
    }
    #endregion

    #region Emitting event
    public void Emit_GetNearestEnemy(string opponent)
    {
        socketManager.Socket.Emit("loading-complete");
    }
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

public enum Phase
{
    PlanningPhase = 0,
    CombatPhase = 1,
}

using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitManagerSocketIO;
using Unity.VisualScripting;
using System.Linq;

[Serializable]
public class PlayerDataInBattleSocketIO
{
    private SocketManager socketManager;
    [SerializeField] private PlayerDataJSON _playerData;
    public PlayerDataJSON playerData => _playerData;

    public void PlayerDataInBattleSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<string>("update-player-stat", (playerStat) => {
            Debug.Log("update-player-stat");
            Debug.Log(playerStat);
            On_UpdatePlayerData(playerStat);
        });
        socketManager.Socket.On<string>("get-player-data-success", (playerData) => {
            On_GetPlayerData(playerData);
        });
        socketManager.Socket.On<int>("update-level", (level) => {
            On_UpdateLevel(level);
        });
        socketManager.Socket.On<string[]>("deal-damage-to-opponent", (target) => {
            On_DealDamageToOpponent(target);
        });
        socketManager.Socket.On<int>("take-damage-from-opponent", (damage) => {
            On_TakeDamageFromOpponent(damage);
        });
    }

    #region Listening to events
    private void On_UpdatePlayerData(string playerData)
    {
        var player = JsonConvert.DeserializeObject<PlayerDataJSON>(playerData);
        _playerData = player;
        //player.GetAll();
    }

    private void On_GetPlayerData(string playerData)
    {
        Debug.Log("get-player-stat-success");
        var player = JsonConvert.DeserializeObject<PlayerDataJSON>(playerData);
        _playerData = player;
    }
    private void On_UpdateLevel(int level)
    {
        Debug.Log("On_UpdateLevel: " + level.ToString());
        RoomManager.instance.myTactician.GetComponent<PetManager>().SetLevel(level);
    }

    private void On_DealDamageToOpponent(string[] data)
    {
        Debug.Log("On_DealDamageToOpponent: " + data[0] + " - " + data[1]);
        GameObject targetTactician = GameObject.FindGameObjectsWithTag("Tactician").FirstOrDefault(x => x.GetComponent<PetManager>().owner == data[0]);
        if(targetTactician != null)
        {
            RoomManager.instance.myTactician.GetComponent<PetManager>().DealDamage(targetTactician, int.Parse(data[1]));
        }
        //RoomManager.instance.myTactician.GetComponent<PetManager>().DealDamage();
    }

    private void On_TakeDamageFromOpponent(int damage)
    {
        Debug.Log("On_TakeDamageFromOpponent: " + damage.ToString());
        RoomManager.instance.myTactician.GetComponent<PetManager>().TakeDamage(damage);
    }
    #endregion

    #region Emitting events
    public void Emit_GetPlayerData()
    {
        socketManager.Socket.Emit("get-player-data");
    }
    #endregion

    [Serializable]
    public class PlayerDataJSON
    {
        public string _username;
        public string _profileImage;
        public string _socketid;
        public int _hp;
        public int _maxhp;
        public int _level;
        public int _gold;
        public int _place;
        public RollingChances _rerollProbability;
        public UnitShop _unitShop;
        public int _goldRefreshUnitShop;
        public bool _lockedUnitShop;
        public Bench _bench;
        public Battlefield _battlefield;
        public int _maxUnitInBench;
        public int _maxUnitInBattlefield;
        public string _phase;

        [Serializable]
        public class UnitShop
        {
            public UnitInfo slot0;
            public UnitInfo slot1;
            public UnitInfo slot2;
            public UnitInfo slot3;
            public UnitInfo slot4;
        }

        [Serializable]
        public class Bench
        {
            public UnitInfo slot0_0;
            public UnitInfo slot0_1;
            public UnitInfo slot0_2;
            public UnitInfo slot0_3;
            public UnitInfo slot0_4;
            public UnitInfo slot0_5;
            public UnitInfo slot0_6;
            public UnitInfo slot0_7;
            public UnitInfo slot0_8;
            public UnitInfo slot0_9;
        }

        [Serializable]
        public class Battlefield
        {
            public UnitInfo slot0_0;
            public UnitInfo slot0_1;
            public UnitInfo slot0_2;
            public UnitInfo slot0_3;
            public UnitInfo slot0_4;
            public UnitInfo slot0_5;
            public UnitInfo slot1_0;
            public UnitInfo slot1_1;
            public UnitInfo slot1_2;
            public UnitInfo slot1_3;
            public UnitInfo slot1_4;
            public UnitInfo slot1_5;
            public UnitInfo slot2_0;
            public UnitInfo slot2_1;
            public UnitInfo slot2_2;
            public UnitInfo slot2_3;
            public UnitInfo slot2_4;
            public UnitInfo slot2_5;

            public Dictionary<int[], UnitInfo> Fomation()
            {
                Dictionary<int[], UnitInfo> fomation = new Dictionary<int[], UnitInfo>();
                if (slot0_0 != null) { fomation.Add(new int[2] { 0, 0 }, slot0_0); }
                if (slot0_1 != null) { fomation.Add(new int[2] { 0, 1 }, slot0_1); }
                if (slot0_2 != null) { fomation.Add(new int[2] { 0, 2 }, slot0_2); }
                if (slot0_3 != null) { fomation.Add(new int[2] { 0, 3 }, slot0_3); }
                if (slot0_4 != null) { fomation.Add(new int[2] { 0, 4 }, slot0_4); }
                if (slot0_5 != null) { fomation.Add(new int[2] { 0, 5 }, slot0_5); }
                if (slot1_0 != null) { fomation.Add(new int[2] { 1, 0 }, slot1_0); }
                if (slot1_1 != null) { fomation.Add(new int[2] { 1, 1 }, slot1_1); }
                if (slot1_2 != null) { fomation.Add(new int[2] { 1, 2 }, slot1_2); }
                if (slot1_3 != null) { fomation.Add(new int[2] { 1, 3 }, slot1_3); }
                if (slot1_4 != null) { fomation.Add(new int[2] { 1, 4 }, slot1_4); }
                if (slot1_5 != null) { fomation.Add(new int[2] { 1, 5 }, slot1_5); }
                if (slot2_0 != null) { fomation.Add(new int[2] { 2, 0 }, slot2_0); }
                if (slot2_1 != null) { fomation.Add(new int[2] { 2, 1 }, slot2_1); }
                if (slot2_2 != null) { fomation.Add(new int[2] { 2, 2 }, slot2_2); }
                if (slot2_3 != null) { fomation.Add(new int[2] { 2, 3 }, slot2_3); }
                if (slot2_4 != null) { fomation.Add(new int[2] { 2, 4 }, slot2_4); }
                if (slot2_5 != null) { fomation.Add(new int[2] { 2, 5 }, slot2_5); }
                return fomation;
            }
        }

        [Serializable]
        public class RollingChances
        {
            public int tier1;
            public int tier2;
            public int tier3;
            public int tier4;
            public int tier5;
        }

        public Dictionary<string, UnitInfo> GetUnitShop()
        {
            Dictionary<string, UnitInfo> kvp = new Dictionary<string, UnitInfo>();
            kvp.Add("slot0", _unitShop.slot0);
            kvp.Add("slot1", _unitShop.slot1);
            kvp.Add("slot2", _unitShop.slot2);
            kvp.Add("slot3", _unitShop.slot3);
            kvp.Add("slot4", _unitShop.slot4);
            return kvp;
        }

        public bool BenchContains(UnitInfo unit)
        {
            if(_bench.slot0_0 == unit 
                || _bench.slot0_1 == unit
                || _bench.slot0_2 == unit
                || _bench.slot0_3 == unit
                || _bench.slot0_4 == unit
                || _bench.slot0_5 == unit
                || _bench.slot0_6 == unit
                || _bench.slot0_7 == unit
                || _bench.slot0_8 == unit
                || _bench.slot0_9 == unit)
            { return true; }
            return false;
        }

        public bool BattlefieldContains(UnitInfo unit)
        {
            if (_bench.slot0_0 == unit
                || _bench.slot0_1 == unit
                || _bench.slot0_2 == unit
                || _bench.slot0_3 == unit
                || _bench.slot0_4 == unit
                || _bench.slot0_5 == unit
                || _bench.slot0_6 == unit
                || _bench.slot0_7 == unit
                || _bench.slot0_8 == unit
                || _bench.slot0_9 == unit)
            { return true; }
            return false;
        }
    }
}

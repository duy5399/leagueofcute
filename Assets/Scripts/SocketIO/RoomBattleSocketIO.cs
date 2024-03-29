using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitManagerSocketIO;

[Serializable]
public class RoomBattleSocketIO
{
    private SocketManager socketManager;
    [SerializeField] private string _room;
    public string room => _room;
    public void RoomBattleSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<string, string>("join-room-success", (roomName, allPlayerData) => {
            On_JoinRoomSuccess(roomName, allPlayerData);
        });
        socketManager.Socket.On<string>("join-room-fail", (data) => {
            On_JoinRoomFail(data);
        });
        socketManager.Socket.On("loading-complete-success", () => {
            On_LoadingComplete();
        });
    }

    #region Listening to events
    private void On_JoinRoomSuccess(string roomName, string allPlayerData)
    {
        if (allPlayerData != null)
        {
            _room = roomName;
            Debug.Log(allPlayerData);
            var playerDataLoading = JsonConvert.DeserializeObject<PlayerDataLoadingJSON[]>(allPlayerData);
            LoadingSceneManager.instance.LoadSceneBattleAsync(2, playerDataLoading);
        }
    }
    private void On_JoinRoomFail(string data)
    {
        Debug.Log(data);
    }

    private void On_LoadingComplete()
    {
        LoadingSceneManager.instance.SetLoadingBattle(false);
    }
    #endregion

    #region Emitting events
    public void Emit_LoadingComplete(bool isloadingComplete)
    {
        socketManager.Socket.Emit("loading-complete", isloadingComplete);
    }
    public void Emit_LeaveRoom()
    {
        socketManager.Socket.Emit("leave-room", room);
    }
    #endregion

    [Serializable]
    public class PlayerDataLoadingJSON
    {
        public string username;
        public string profileImage;
        public string rank;
        public int points;
        public string tacticianEquip;
    }
}

using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MatchmakingSocketIO
{
    private SocketManager socketManager;
    public void MatchmakingSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On("match-found", () => {
            On_MatchFound();
        });

        socketManager.Socket.On<string>("accept-match-found-success", (data) => {
            Emit_JoinRoom(data);
        });
    }

    #region Listening to events
    private void On_MatchFound()
    {
        FindMatchManager.instance.MatchFound();
    }

    #endregion

    #region Emitting events
    public void Emit_StartMatchmaking()
    {
        socketManager.Socket.Emit("start-matchmaking");
    }

    public void Emit_StopMatchmaking()
    {
        socketManager.Socket.Emit("stop-matchmaking");
    }

    public void Emit_AcceptMatchFound(bool isAccept)
    {
        socketManager.Socket.Emit("accept-match-found", isAccept);
    }

    public void Emit_JoinRoom(string data)
    {
        socketManager.Socket.Emit("join-room", data);
    }
    #endregion
}

using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnitManagerSocketIO;

[Serializable]
public class TraitsSocketIO
{
    private SocketManager socketManager;
    public void TraitsSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<string>("get-traits-success", (data) => {
            On_GetTraitsSuccess(data);
            //Debug.Log("get-traits: " + data);
        });
        socketManager.Socket.On<bool>("active-traits-success", (data) => {
            On_ActiveTraitsSuccess(data);
            //Debug.Log("active-traits-success: " + data);
        });
    }
    #region Listening to events
    private void On_GetTraitsSuccess(string traitsJSON)
    {
        var traits = JsonConvert.DeserializeObject<AllTraitsBase>(traitsJSON);
        TraitsManager.instance.allTraitsBase = traits;
    }
    private void On_ActiveTraitsSuccess(bool isActive)
    {
        foreach (var i in TraitsManager.instance.dictTraits)
        {
            i.Key.GetComponent<ChTraits>().isActive = isActive;
        }
    }
    #endregion
    #region Emitting events
    public void Emit_GetTrait()
    {
        socketManager.Socket.Emit("get-traits");
    }
    #endregion
}

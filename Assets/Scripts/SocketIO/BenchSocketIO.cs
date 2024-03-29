using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitManagerSocketIO;
using static UnitShopSocketIO;

public class BenchSocketIO
{
    private SocketManager socketManager;
    public void BenchSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<string, string>("remove-unit-on-bench", (data1, data2) => {
            On_RemoveUnitOnBench(data1, data2);
        });
    }
    #region Listening to events
    void On_RemoveUnitOnBench(string _slot, string _unitInfo)
    {
        Debug.Log("On_RemoveUnitOnBench: " + _slot + " - " + _unitInfo);
        UnitInfo unitInfo = JsonConvert.DeserializeObject<UnitInfo>(_unitInfo);
        BenchManager.instance.RemoveUnit(_slot, unitInfo);
    }
    #endregion

    #region Emitting event

    #endregion
}

using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerDataInBattleSocketIO;

[Serializable]
public class ScoreboardBattleSocketIO
{
    private SocketManager socketManager;
    public void ScoreboardBattleSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<string>("update-scoreboard", (scoreboardJSON) => {
            On_UpdateScoreboard(scoreboardJSON);
        });
        socketManager.Socket.On("on-dead", () => {
            On_Dead();
        });
        socketManager.Socket.On<string[]>("end-game", (data) => {
            On_EndGame(data);
        });
    }

    #region Listening to events
    private void On_UpdateScoreboard(string scoreboardJSON)
    {
        var playersList = JsonConvert.DeserializeObject<PlayerDataJSON[]>(scoreboardJSON);
        ScoreboardManager.instance.UpdateLeaderboard(playersList);
    }
    private void On_Dead()
    {
        Debug.Log("On_Dead: ");
        BattleResultManager.instance.gameObject.SetActive(true);
        BattleResultManager.instance.OnDisableScreen();
    }
    private void On_EndGame(string[] data)
    {
        int place = Int32.Parse(data[0]);
        string rank = data[1];
        int points = Int32.Parse(data[2]);
        int addPoints = Int32.Parse(data[3]);
        PlayerDataJSON[] playerData = JsonConvert.DeserializeObject<PlayerDataJSON[]>(data[4]);
        
        Debug.Log("On_EndGame: " + place + " - " + rank + " - " + points + " - " + addPoints + " - " + data[4]);

        BattleResultManager.instance.gameObject.SetActive(true);
        BattleResultManager.instance.OnSetBattleResult(place);
        BattleResultManager.instance.btnConfirm.onClick.AddListener(() =>
        {
            LoadingSceneManager.instance.LoadLeaderboardAsync(1, place, rank, points, addPoints, playerData);
            Debug.Log("BattleResultManager.instance.btnConfirm.onClick.AddListener");
            BattleResultManager.instance.btnConfirm.onClick.RemoveAllListeners();
        });
    }
    #endregion

    #region Emitting events
    public void Emit_UpdateLeaderboard(string room)
    {
        socketManager.Socket.Emit("update-leaderboard", room);
    }
    #endregion
}

using BestHTTP.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerDataInBattleSocketIO;

public class ScoreboardManager : MonoBehaviour
{
    public static ScoreboardManager instance { get; private set; }

    [SerializeField] private Transform tf_Leaderboard;
    [SerializeField] private GameObject go_Prefab_PlayerInfo;

    [SerializeField] private Dictionary<string, GameObject> dict_Leaderboard = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        //UpdateLeaderboard();
        //SocketIO.instance.Emit_UpdateLeaderboard();
    }

    public void UpdateLeaderboard(PlayerDataJSON[] players)
    {
        //SocketIO.instance.Emit_UpdateLeaderboard();
        foreach (var p in players)
        {
            if (!dict_Leaderboard.ContainsKey(p._username))
            {
                GameObject obj = Instantiate(go_Prefab_PlayerInfo, tf_Leaderboard);
                obj.GetComponent<ScoreboardPlayerInfoManager>().SetPlayerName(p._username);
                obj.GetComponent<ScoreboardPlayerInfoManager>().SetAvatar(p._profileImage);
                obj.GetComponent<ScoreboardPlayerInfoManager>().SetHP(p._hp, p._maxhp);
                dict_Leaderboard.Add(p._username, obj);
            }
            else
            {
                dict_Leaderboard[p._username].GetComponent<ScoreboardPlayerInfoManager>().SetHP(p._hp, p._maxhp);
                dict_Leaderboard[p._username].transform.SetSiblingIndex(p._place);
            }
        }
    }
}

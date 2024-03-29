using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerDataInBattleSocketIO;
using static RoomBattleSocketIO;

public class LoadingSceneManager : MonoBehaviourPun
{
    public static LoadingSceneManager instance { get; private set; }

    [Header("Loading Data")]
    [SerializeField] private Transform tf_LoadingScreen;


    [Header("Loading Battle")]
    [SerializeField] private Transform tf_LoadingScreenBattle;
    [SerializeField] private Slider slider_LoadingProcess;
    [SerializeField] private TextMeshProUGUI text_LoadingProcess;
    [SerializeField] private Transform tf_PlayersList;
    [SerializeField] private GameObject go_Prefab_PlayerInfo;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        //LoadSceneAsync(1);
        DontDestroyOnLoad(gameObject);
        SetLoadingData(false);
        SetLoadingBattle(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #region Loading Data
    public void SetLoadingData(bool boolean)
    {
        tf_LoadingScreen.gameObject.SetActive(boolean);
    }
    #endregion

    #region Loading Battle
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 1)
        {
            SetLoadingData(false);
        }
        else if (scene.buildIndex == 2)
        {
            RoomManager.instance.CreateOrJoinRoom(SocketIO.instance.roomBattleSocketIO.room);
        }
    }

    public void SetLoadingBattle(bool boolean)
    {
        tf_LoadingScreenBattle.gameObject.SetActive(boolean);
    }
    public void LoadSceneBattleAsync(int sceneName, PlayerDataLoadingJSON[] playersList)
    {
        StartCoroutine(Coroutine_LoadSceneBattleAsync(sceneName, playersList));
    }

    IEnumerator Coroutine_LoadSceneBattleAsync(int sceneName, PlayerDataLoadingJSON[] playersList)
    {
        SetLoadingBattle(true);
        for(int i = 0; i < tf_PlayersList.childCount; i++)
        {
            Destroy(tf_PlayersList.GetChild(i).gameObject);
        }
        foreach (var player in playersList)
        {
            Debug.Log("Coroutine_LoadSceneBattleAsync: " + player.username);
            GameObject obj = Instantiate(go_Prefab_PlayerInfo, tf_PlayersList);
            obj.GetComponent<InfoPlayerLoadingManager>().SetInfoPlayer(player);
        }
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!loadOperation.isDone)
        {
            slider_LoadingProcess.value = 1 - loadOperation.progress;
            text_LoadingProcess.text = string.Format("{0:0}", loadOperation.progress * 100) + "%";
            yield return null;
        }
    }

    public void LoadingPlayerInfo(PlayerDataJSON[] players)
    {
        foreach(PlayerDataJSON playerStat in players)
        {
            GameObject obj = Instantiate(go_Prefab_PlayerInfo, tf_PlayersList);
            obj.GetComponent<LoadingPlayerInfoManager>().SetPlayerInfo(playerStat._socketid, playerStat._socketid);
        }
    }
    #endregion
    public void LoadLeaderboardAsync(int sceneName, int standing, string rank, int points, int addPoints, PlayerDataJSON[] playersList)
    {
        StartCoroutine(Coroutine_LoadLeaderboardAsync(sceneName, standing, rank, points, addPoints, playersList));
    }
    IEnumerator Coroutine_LoadLeaderboardAsync(int sceneName, int standing, string rank, int points, int addPoints, PlayerDataJSON[] playersList)
    {
        try
        {
            if (PhotonNetwork.IsMasterClient/* && PhotonNetwork.PlayerList.Length > 0*/)
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.MasterClient.GetNext());
                Debug.Log("PhotonNetwork.SetMasterClient success");
            }
        }
        catch
        {
            Debug.Log("PhotonNetwork.SetMasterClient error");
        }
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!loadOperation.isDone)
        {
            yield return null;
        }
        PhotonNetwork.LeaveRoom();
        Leaderboard_TheEndGame_Manager.instance.gameObject.SetActive(true);
        Leaderboard_TheEndGame_Manager.instance.SetTxtStanding(standing);
        Leaderboard_TheEndGame_Manager.instance.SetCurrentRanking(rank);
        Leaderboard_TheEndGame_Manager.instance.SetCurrentPoint(points, addPoints);
        Leaderboard_TheEndGame_Manager.instance.SetLeaderboad(playersList);
        //foreach (var player in playersList)
        //{
        //    GameObject obj = Instantiate(go_Prefab_PlayerInfo, tf_PlayersList.transform);
        //    obj.GetComponent<InfoPlayerLoadingManager>().SetInfoPlayer(player);
        //}
    }
}

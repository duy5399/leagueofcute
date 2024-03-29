using Photon.Pun;
using Photon.Realtime;
using PlayFab.EconomyModels;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using static StoreClientSocketIO.ItemInStoreJSON;
using static UnitManagerSocketIO;

public class RoomManager : MonoBehaviourPun
{
    public static RoomManager instance { get; private set; }

    [SerializeField] private GameObject _myTactician;
    [SerializeField] private Vector3 v3_TacticianPos;
    [SerializeField] private List<Vector3> list_v3_PlayerPos;
    [SerializeField] private GameObject _playerPos;
    [SerializeField] private List<GameObject> _monstersLst;

    public GameObject playerPos
    {
        get { return _playerPos; }
        set { _playerPos = value; }
    }

    public GameObject myTactician
    {
        get { return _myTactician; }
        set { _myTactician = value;}
    }
    public List<GameObject> monstersLst
    {
        get { return _monstersLst; }
        set { _monstersLst = value; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        _monstersLst = new List<GameObject>();
        list_v3_PlayerPos = new List<Vector3>() { new Vector3(0, 0, 0), new Vector3(46, 0, 0), new Vector3(0, 0, -40), new Vector3(46, 0, -40) };
        v3_TacticianPos = new Vector3(-9, 2, -9);
    }

    public void CreateOrJoinRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = false,
            IsOpen = true,
            MaxPlayers = (byte)2,
        };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
    }

    public void InstantiatePlayerPos(string username, int place)
    {
        var prefab_playerPos = Resources.Load<GameObject>("prefabs/fight/arenas/PlayerPos");
        GameObject playerPosInstance = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/arenas", prefab_playerPos.name), prefab_playerPos.transform.position, prefab_playerPos.transform.rotation);
        playerPosInstance.GetComponent<PlayerPosManager>().ChangeName("Player_" + username);
        playerPosInstance.transform.position = list_v3_PlayerPos[place];
        CameraController.instance.SetCameraHome(playerPosInstance.transform.position);
        playerPos = playerPosInstance;
    }

    public void InstantiateArena(string arenaSkinName, int place)
    {
        var prefab_arena = Resources.Load<GameObject>("prefabs/fight/arenas/Mainboard");
        GameObject arena = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/arenas", prefab_arena.name), prefab_arena.transform.position, prefab_arena.transform.rotation);
        arena.GetComponent<ArenaSkinsManager>().ChangeArenaSkin(arenaSkinName);
        arena.GetComponent<ArenaSkinsManager>().ChangeParent(playerPos.GetPhotonView().ViewID);
        arena.transform.localPosition = Vector3.zero;
    }

    public void InstantiateBench()
    {
        var prefab_bench = Resources.Load<GameObject>("prefabs/fight/arenas/Bench");
        GameObject bench = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/arenas", prefab_bench.name), prefab_bench.transform.position, prefab_bench.transform.rotation);
        bench.GetComponent<BenchManager>().SetParent(playerPos.GetPhotonView().ViewID);
        bench.transform.localPosition = Vector3.zero;
    }

    public void InstantiateBattlefieldSide()
    {
        var prefab_battlefieldSide = Resources.Load<GameObject>("prefabs/fight/arenas/BattlefieldSide");
        GameObject battlefieldSide = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/arenas", prefab_battlefieldSide.name), prefab_battlefieldSide.transform.position, prefab_battlefieldSide.transform.rotation);
        battlefieldSide.GetComponent<BattlefieldSideManager>().SetParent(playerPos.GetPhotonView().ViewID);
        battlefieldSide.transform.localPosition = prefab_battlefieldSide.transform.position;
        //6.23,0,4.55
    }

    public void InstantiateItemsManager()
    {
        var prefab_itemsDropStorage = Resources.Load<GameObject>("prefabs/fight/arenas/ItemsDropStorage");
        GameObject itemsDropStorage = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/arenas", prefab_itemsDropStorage.name), prefab_itemsDropStorage.transform.position, prefab_itemsDropStorage.transform.rotation);
        itemsDropStorage.GetComponent<ItemsDropManager>().SetParent(playerPos.GetPhotonView().ViewID);
        itemsDropStorage.transform.localPosition = prefab_itemsDropStorage.transform.position;
    }

    public void InstantiateTactician(string tacticianName, int place)
    {
        var prefab_tactician = Resources.Load<GameObject>("prefabs/fight/tacticians/" + tacticianName);
        GameObject tactician = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/tacticians", prefab_tactician.name), list_v3_PlayerPos[place], prefab_tactician.transform.rotation);
        tactician.GetComponent<PetManager>().owner = SocketIO.instance.playerDataInBattleSocketIO.playerData._username;
        tactician.GetComponent<PetManager>().ChangeParent(playerPos.GetPhotonView().ViewID);
        tactician.transform.localPosition = v3_TacticianPos;
        myTactician = tactician;
    }

    public void InstantiateBoom(string boomName)
    {
        var prefab_boom = Resources.Load<GameObject>("prefabs/fight/booms/" + boomName);
        myTactician.GetComponent<PetManager>().myBoom = prefab_boom;
    }

    public void InstantiateMonster(UnitInfo unitJSON, int[] node, Item item, string coin)
    {
        var prefab_monster = Resources.Load<GameObject>("prefabs/fight/units/" + unitJSON.championName);
        GameObject monster = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/units", prefab_monster.name), playerPos.transform.position, prefab_monster.transform.rotation);
        monster.GetComponent<ChampionInfo1>().chStat = unitJSON;
        monster.GetComponent<ChampionInfo1>().chCategory = ChampionInfo1.Categories.Monster;
        monster.GetComponent<ChampionInfo1>().moveManager.positionNode = node;
        monster.GetComponent<ChampionInfo1>().SetName(unitJSON.championName + "_" + unitJSON._id);
        monster.GetComponent<ChampionInfo1>().currentState.InitState();
        monster.GetComponent<ChampionDrop>().AddItemDrop(item);
        monster.GetComponent<ChampionDrop>().AddCoinDrop(coin);
        GameObject parent = BattlefieldSideManager.instance.dict_BattlefieldSide.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._node.SequenceEqual(node)).Key;
        BattlefieldSideManager.instance.SetDictBattlefield(parent, monster);
        monster.GetComponent<ChampionBase>().SetParent(parent.GetPhotonView().ViewID);
        monster.transform.localPosition = Vector3.zero;
        monstersLst.Add(monster);
        var prefab_SummonMonsterSuccess = Resources.Load<GameObject>("prefabs/vfx/vfx_MagicAbility_ArcaneCircle");
        GameObject obj = PhotonNetwork.Instantiate(Path.Combine("prefabs/vfx", prefab_SummonMonsterSuccess.name), parent.transform.position, Quaternion.identity);
        StartCoroutine(Coroutine_Destroy(obj, 3f));
    }

    public void ClearMonster()
    {
        foreach (var monster in monstersLst)
        {
            if(monster != null)
            {
                Debug.Log("RoomManager.instance.lstMonsters: " + monster.name + " - " + monster.GetPhotonView().ViewID);
                if (monster.GetComponent<PhotonView>().IsMine)
                {
                    monster.GetComponent<PhotonView>().Synchronization = ViewSynchronization.Off;
                    Debug.Log("RoomManager.instance.lstMonsters: IsMine " + monster.name + " - " + monster.GetPhotonView().ViewID);
                    PhotonNetwork.Destroy(monster.GetComponent<PhotonView>());
                }
            }
        }
        monstersLst.Clear();
    }

    IEnumerator Coroutine_Destroy(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        if (obj.GetComponent<PhotonView>().IsMine)
        {
            PhotonNetwork.Destroy(obj.GetComponent<PhotonView>());
        }
    }
}

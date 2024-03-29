using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO3;
using System;
using System.Web;
using Newtonsoft.Json;
using System.Linq;
using static AuthenticationSocketIO;
using static UnitManagerSocketIO;

public class SocketIO : MonoBehaviour
{
    public static SocketIO instance { get; private set; }
    public SocketManager socketManager { get; private set; }


    [Header("Client")]
    [SerializeField] private AuthenticationSocketIO authenticationSocketIO = new AuthenticationSocketIO();
    [SerializeField] private StoreClientSocketIO storeClientSocketIO = new StoreClientSocketIO();
    [SerializeField] private InventoryClientSocketIO inventoryClientSocketIO = new InventoryClientSocketIO();
    [SerializeField] private FriendSocketIO friendSocketIO = new FriendSocketIO();
    [SerializeField] private RankedSocketIO _rankedSocketIO = new RankedSocketIO();

    [Header("Battle")]
    [SerializeField] private MatchmakingSocketIO matchmakingSocketIO = new MatchmakingSocketIO();
    [SerializeField] private RoomBattleSocketIO _roomBattleSocketIO = new RoomBattleSocketIO();
    [SerializeField] private PlayerDataInBattleSocketIO _playerDataInBattleSocketIO = new PlayerDataInBattleSocketIO();
    [SerializeField] private UnitShopSocketIO unitShopSocketIO = new UnitShopSocketIO();
    [SerializeField] private BenchSocketIO benchSocketIO = new BenchSocketIO();
    [SerializeField] private BattlefieldSocketIO battlefieldSocketIO = new BattlefieldSocketIO();
    [SerializeField] private ScoreboardBattleSocketIO scoreboardBattleSocketIO = new ScoreboardBattleSocketIO();
    [SerializeField] private UnitManagerSocketIO unitManagerSocketIO = new UnitManagerSocketIO();
    [SerializeField] private StageBattleSocketIO stageBattleSocketIO = new StageBattleSocketIO();
    [SerializeField] private TraitsSocketIO _traitsSocketIO = new TraitsSocketIO();
    [SerializeField] private ItemsSocketIO _itemsSocketIO = new ItemsSocketIO();

    public AuthenticationSocketIO _authenticationSocketIO => authenticationSocketIO;
    public StoreClientSocketIO _storeClientSocketIO => storeClientSocketIO;
    public InventoryClientSocketIO _inventoryClientSocketIO => inventoryClientSocketIO;
    public FriendSocketIO _friendSocketIO => friendSocketIO;
    public RankedSocketIO rankedSocketIO => _rankedSocketIO;
    public MatchmakingSocketIO _matchmakingSocketIO => matchmakingSocketIO;
    public RoomBattleSocketIO roomBattleSocketIO => _roomBattleSocketIO;
    public PlayerDataInBattleSocketIO playerDataInBattleSocketIO => _playerDataInBattleSocketIO;
    public UnitShopSocketIO _unitShopSocketIO => unitShopSocketIO;
    public BenchSocketIO _benchSocketIO => benchSocketIO;
    public BattlefieldSocketIO _battlefieldSocketIO => battlefieldSocketIO;
    public ScoreboardBattleSocketIO _scoreboardBattleSocketIO => scoreboardBattleSocketIO;
    public UnitManagerSocketIO _unitManagerSocketIO => unitManagerSocketIO;
    public StageBattleSocketIO _stageBattleSocketIO => stageBattleSocketIO;
    public TraitsSocketIO traitsSocketIO => _traitsSocketIO;
    public ItemsSocketIO itemsSocketIO => _itemsSocketIO;

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        socketManager = new SocketManager(new Uri("http://localhost:3000"));
        socketManager.Socket.On("connection", () => Debug.Log(socketManager.Socket.Id));

        #region Client
        authenticationSocketIO.AuthenticationSocketIOStart(socketManager);
        storeClientSocketIO.StoreClientSocketIOStart(socketManager);
        inventoryClientSocketIO.InventoryClientSocketIOStart(socketManager);
        friendSocketIO.FriendSocketIOStart(socketManager);
        rankedSocketIO.RankedSocketIOStart(socketManager);
        #endregion

        #region Battle
        matchmakingSocketIO.MatchmakingSocketIOStart(socketManager);
        roomBattleSocketIO.RoomBattleSocketIOStart(socketManager);
        playerDataInBattleSocketIO.PlayerDataInBattleSocketIOStart(socketManager);
        unitShopSocketIO.UnitShopSocketIOStart(socketManager);
        benchSocketIO.BenchSocketIOStart(socketManager);
        battlefieldSocketIO.BattlefieldSocketIOStart(socketManager);
        scoreboardBattleSocketIO.ScoreboardBattleSocketIOStart(socketManager);
        unitManagerSocketIO.UnitSocketIOStart(socketManager);
        stageBattleSocketIO.StageBattleSocketIOStart(socketManager);
        _traitsSocketIO.TraitsSocketIOStart(socketManager);
        _itemsSocketIO.ItemsSocketIOStart(socketManager);
        #endregion
    }

    private void Update()
    {
        try{
            playerDataInBattleSocketIO.Emit_GetPlayerData();
        }
        catch {
            Debug.Log("Emit_GetPlayerStat error");
        }
    }
}

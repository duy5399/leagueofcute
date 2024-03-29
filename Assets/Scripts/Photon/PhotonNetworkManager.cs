using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        ConnectToPhotonPUN();
    }

    public void ConnectToPhotonPUN()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("ConnectToPhotonPUN");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        ConnectToPhotonPUN();
    }

    public override void OnJoinedLobby() 
    {
        LoadingSceneManager.instance.SetLoadingData(false);
    }

    public override void OnCreatedRoom()
    {
        
    }

    public override void OnJoinedRoom()
    {
        RoomManager.instance.InstantiatePlayerPos(SocketIO.instance.playerDataInBattleSocketIO.playerData._username , SocketIO.instance.playerDataInBattleSocketIO.playerData._place);
        RoomManager.instance.InstantiateArena(SocketIO.instance._inventoryClientSocketIO._userInventory.arenaSkinEquip, SocketIO.instance.playerDataInBattleSocketIO.playerData._place);
        RoomManager.instance.InstantiateBench();
        RoomManager.instance.InstantiateBattlefieldSide();
        RoomManager.instance.InstantiateItemsManager();
        RoomManager.instance.InstantiateTactician("petpengu Variant object", SocketIO.instance.playerDataInBattleSocketIO.playerData._place);
        RoomManager.instance.InstantiateBoom("boomArcadeBomb");
        SocketIO.instance.traitsSocketIO.Emit_GetTrait();
        SocketIO.instance.roomBattleSocketIO.Emit_LoadingComplete(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomManager.instance.CreateOrJoinRoom(SocketIO.instance.roomBattleSocketIO.room);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        RoomManager.instance.CreateOrJoinRoom(SocketIO.instance.roomBattleSocketIO.room);
    }
}

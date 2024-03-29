using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AuthenticationSocketIO;

[Serializable]
public class FriendSocketIO
{
    private SocketManager socketManager;
    [SerializeField] private List<UserInfoJSON> friendsAccepted;
    [SerializeField] private List<UserInfoJSON> friendsSendRequest;
    [SerializeField] private List<UserInfoJSON> friendsWaitingResponse;
    public List<UserInfoJSON> _friendsAccepted => friendsAccepted;
    public List<UserInfoJSON> _friendsSendRequest => friendsSendRequest;
    public List<UserInfoJSON> _friendsWaitingResponse => friendsWaitingResponse;

    public void FriendSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<string, string, string>("get-friend-list", (data1, data2, data3) => {
            On_GetFriendList(data1, data2, data3);
        });

        socketManager.Socket.On<string>("send-friend-request-success", (data) => {
            On_SendFriendRequestSuccess(data);
        });

        socketManager.Socket.On<string>("send-friend-request-error", (data) => {
            On_SendFriendRequestError(data);
        });

        socketManager.Socket.On<string>("new-request-waiting-response", (data) => {
            On_NewRequestWaitingResponse(data);
        });

        socketManager.Socket.On<string>("accept-friend-request-success", (data) => {
            On_AcceptFriendRequestSuccess(data);
        });

        socketManager.Socket.On<string>("cancel-friend-request-success", (data) => {
            On_CancelFriendRequestSuccess(data);
        });

        socketManager.Socket.On<string>("cancel-friend-request-error", (data) => {
            On_CancelFriendRequestError(data);
        });

        socketManager.Socket.On<string>("cancel-request-waiting-response", (data) => {
            On_CancelRequestWaitingResponse(data);
        });

        socketManager.Socket.On<string>("accept-request-waiting-response-success", (data) => {
            On_AcceptRequestWaitingResponseSuccess(data);
        });

        socketManager.Socket.On<string>("decline-request-waiting-response-success", (data) => {
            On_DeclineRequestWaitingResponseSuccess(data);
        });
    }

    #region Listening to events
    private void On_GetFriendList(string data1, string data2, string data3)
    {
        friendsAccepted = JsonConvert.DeserializeObject<List<UserInfoJSON>>(data1);
        friendsSendRequest = JsonConvert.DeserializeObject<List<UserInfoJSON>>(data2);
        friendsWaitingResponse = JsonConvert.DeserializeObject<List<UserInfoJSON>>(data3);
        try
        {
            FriendListManager.instance.SetFriendList(friendsAccepted);
            AddFriendManager.instance.SetSendFriendRequest(friendsSendRequest);
            WaitingResponseManager.instance.SetWaitingResponseRequest(friendsWaitingResponse);
        }
        catch
        {
            Debug.Log("SetFriendList error");
        }
    }

    private void On_SendFriendRequestSuccess(string data)
    {
        var userData = JsonConvert.DeserializeObject<UserInfoJSON>(data);
        friendsSendRequest.Add(userData);
        AddFriendManager.instance.SetSendFriendRequest(friendsSendRequest);
    }

    private void On_SendFriendRequestError(string data)
    {
        Debug.Log(data);
    }

    private void On_AcceptFriendRequestSuccess(string data)
    {
        var userData = JsonConvert.DeserializeObject<UserInfoJSON>(data);
        var request = friendsSendRequest.First(x => x.username == userData.username);
        try
        {
            if (request != null)
            {
                friendsSendRequest.Remove(request);
                AddFriendManager.instance.CancelFriendRequest(request);
            }
            friendsAccepted.Add(userData);
            FriendListManager.instance.SetFriendList(friendsAccepted);
        }
        catch
        {
            Debug.Log("Hiện đang trong trận, không thể hiển thị");
        }
    }

    private void On_CancelFriendRequestSuccess(string data)
    {
        var request = friendsSendRequest.First(x => x.username == data);
        try
        {
            if (request != null)
            {
                friendsSendRequest.Remove(request);
                AddFriendManager.instance.CancelFriendRequest(request);
            }
        }
        catch
        {
            Debug.Log("Hiện đang trong trận, không thể hiển thị");
        }
    }

    private void On_CancelFriendRequestError(string data)
    {
        Debug.Log(data);
    }

    private void On_NewRequestWaitingResponse(string data)
    {
        var userData = JsonConvert.DeserializeObject<UserInfoJSON>(data);
        friendsWaitingResponse.Add(userData);
        WaitingResponseManager.instance.SetWaitingResponseRequest(friendsWaitingResponse);
    }

    private void On_CancelRequestWaitingResponse(string data)
    {
        var request = friendsWaitingResponse.First(x => x.username == data);
        if (request != null)
        {
            friendsWaitingResponse.Remove(request);
            WaitingResponseManager.instance.CancelFriendRequest(request);
        }
    }

    private void On_AcceptRequestWaitingResponseSuccess(string data)
    {
        var userData = JsonConvert.DeserializeObject<UserInfoJSON>(data);
        var request = friendsWaitingResponse.First(x => x.username == userData.username);
        try
        {
            if (request != null)
            {
                friendsWaitingResponse.Remove(request);
                WaitingResponseManager.instance.CancelFriendRequest(request);
            }
            friendsAccepted.Add(userData);
            FriendListManager.instance.SetFriendList(friendsAccepted);
        }
        catch
        {
            Debug.Log("Hiện đang trong trận, không thể hiển thị");
        }
    }

    private void On_DeclineRequestWaitingResponseSuccess(string data)
    {
        var request = friendsWaitingResponse.First(x => x.username == data);
        if (request != null)
        {
            friendsWaitingResponse.Remove(request);
            WaitingResponseManager.instance.CancelFriendRequest(request);
        }
    }
    #endregion

    #region Emitting events
    public void Emit_SendFriendRequest(string username)
    {
        socketManager.Socket.Emit("send-friend-request", username);
    }

    public void Emit_CancelFriendRequest(string username)
    {
        socketManager.Socket.Emit("cancel-friend-request", username);
    }

    public void Emit_AccpectRequestWaitingResponse(UserInfoJSON userInfo)
    {
        socketManager.Socket.Emit("accept-request-waiting-response", userInfo);
    }

    public void Emit_DeclineRequestWaitingResponse(string username)
    {
        socketManager.Socket.Emit("decline-request-waiting-response", username);
    }
    #endregion
}

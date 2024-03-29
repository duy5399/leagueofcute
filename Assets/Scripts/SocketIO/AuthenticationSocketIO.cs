using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AuthenticationSocketIO
{
    private SocketManager socketManager;
    [SerializeField] private UserInfoJSON userInfo;
    public UserInfoJSON _userInfo => userInfo;

    public void AuthenticationSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<string, string>("authorized", (data1, data2) => {
            Debug.Log(data1);
            Debug.Log(data2);
            On_AuthenticationSuccess(data1, data2);
        });

        socketManager.Socket.On<string>("authentication-error", (data) => {
            On_AuthenticationError(data);
        });

        socketManager.Socket.On<string>("register-success", (data) => {
            On_RegisterSuccess(data);
        });

        socketManager.Socket.On<string>("register-error", (data) => {
            On_RegisterError(data);
        });
    }

    #region Listening to events
    private void On_AuthenticationSuccess(string success, string data2)
    {
        userInfo = JsonConvert.DeserializeObject<UserInfoJSON>(data2);
        LoginManager.instance.LoginSuccess(success);
    }

    private void On_AuthenticationError(string error)
    {
        LoginManager.instance.LoginError(error);
    }

    private void On_RegisterSuccess(string success)
    {
        RegisterManager.instance.RegisterSuccess(success);
    }

    private void On_RegisterError(string error)
    {
        RegisterManager.instance.RegisterError(error);
    }
    #endregion

    #region Emitting events
    public void Emit_Login(string username, string password)
    {
        socketManager.Socket.Emit("authenticate", username, password);
    }

    public void Emit_Register(string username, string password, string email)
    {
        socketManager.Socket.Emit("register", username, password, email);
    }
    #endregion
    [Serializable]
    public class UserInfoJSON
    {
        public string username;
        public string profileImage;
        public int level;
        public string rank;
        public int points;
        public bool online;
        public string status;
        public string lastLogin;
    }
}

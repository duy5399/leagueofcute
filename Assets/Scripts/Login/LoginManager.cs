using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Net.Mail;
using TMPro.Examples;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Numerics;
using Photon.Pun;

public class LoginManager : MonoBehaviour
{
    public static LoginManager instance { get; private set; }

    [SerializeField] protected TMP_InputField inputField_Username, inputField_Password;
    [SerializeField] protected Button button_Login;

    [SerializeField] protected ValidateManager validateManager = new ValidateManager();
    [SerializeField] protected AlertManager alertManager;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    public void Login()
    {
        if (!IsValidLogin())
        {
            return;
        }
        LoadingSceneManager.instance.SetLoadingData(true);
        SocketIO.instance._authenticationSocketIO.Emit_Login(inputField_Username.text, inputField_Password.text);
    }

    public void LoginSuccess(string alert)
    {
        StartCoroutine(Coroutine_CheckConnectPhotonServer(alert));
    }

    public void LoginError(string error)
    {
        LoadingSceneManager.instance.SetLoadingData(false);
        alertManager.DisplayAlertPopup(error, new Color32(255, 0, 0, 255));
    }


    private bool IsValidLogin()
    {
        if (!validateManager.IsValidUsername(inputField_Username.text) || !validateManager.IsValidPassword(inputField_Password.text))
        {
            alertManager.DisplayAlertPopup("Invalid username or password", new Color32(255, 0, 0, 255));
            return false;
        }
        else
            return true;
    }

    IEnumerator Coroutine_CheckConnectPhotonServer(string alert)
    {
        while (!PhotonNetwork.IsConnected)
            yield return null;
        alertManager.DisplayAlertPopup(alert, new Color32(0, 255, 0, 255));
        SceneManager.LoadScene("Client");
    }
}
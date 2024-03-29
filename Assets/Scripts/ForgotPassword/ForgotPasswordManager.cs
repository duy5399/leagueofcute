using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ForgotPasswordManager : MonoBehaviour
{
    [SerializeField] protected TMP_InputField inputField_Email;
    [SerializeField] protected Button button_SendEmail;

    [SerializeField] protected ValidateManager validateManager = new ValidateManager();
    [SerializeField] protected AlertManager alertManager;
    public void SendAccountRecoveryEmail()
    {
        if (!IsValidForgotPassword())
        {
            return;
        }
        LoadingSceneManager.instance.SetLoadingData(true);
        var requestResetPassword = new SendAccountRecoveryEmailRequest { Email = inputField_Email.text, TitleId = PlayFabSettings.TitleId };
        PlayFabClientAPI.SendAccountRecoveryEmail(requestResetPassword, SendAccountRecoveryEmailSuccess, SendAccountRecoveryEmailError);
    }

    private void SendAccountRecoveryEmailSuccess(SendAccountRecoveryEmailResult obj)
    {
        LoadingSceneManager.instance.SetLoadingData(false);
        alertManager.DisplayAlertPopup("Password reset request was sent successfully. Please check your email to reset your password", new Color32(0, 255, 0, 255));
    }

    private void SendAccountRecoveryEmailError(PlayFabError obj)
    {
        LoadingSceneManager.instance.SetLoadingData(false);
        alertManager.DisplayAlertPopup("Something went wrong, please try again later", new Color32(0, 127, 255, 255));
        Debug.Log(obj.Error);
    }

    private bool IsValidForgotPassword()
    {
        if (!validateManager.IsValidEmail(inputField_Email.text))
        {
            alertManager.DisplayAlertPopup("Invalid email address", new Color32(255, 0, 0, 255));
            return false;
        }
        return true;
    }
}

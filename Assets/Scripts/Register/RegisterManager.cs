using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterManager : MonoBehaviour
{
    public static RegisterManager instance { get; private set; }

    [SerializeField] protected TMP_InputField inputField_Username, inputField_Password, inputField_ConfirmPassword, inputField_Email;
    [SerializeField] protected Toggle toggle_PrivacyPolicy;
    [SerializeField] protected Button button_Register;

    [SerializeField] protected ValidateManager validateManager = new ValidateManager();
    [SerializeField] protected AlertManager alertManager;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    public void Register()
    {
        if (!IsValidRegister())
        {
            return;
        }
        LoadingSceneManager.instance.SetLoadingData(true);
        SocketIO.instance._authenticationSocketIO.Emit_Register(inputField_Username.text, inputField_Password.text, inputField_Email.text);
    }

    public void RegisterSuccess(string success)
    {
        LoadingSceneManager.instance.SetLoadingData(false);
        alertManager.DisplayAlertPopup(success, new Color32(0, 255, 0, 255));
    }

    public void RegisterError(string error)
    {
        LoadingSceneManager.instance.SetLoadingData(false);
        alertManager.DisplayAlertPopup(error, new Color32(255, 0, 0, 255));
    }

    private bool IsValidRegister()
    {
        if (!validateManager.IsValidUsername(inputField_Username.text))
        {
            alertManager.DisplayAlertPopup("Username must be between 3 to 10 characters", new Color32(255, 0, 0, 255));
            return false;
        }
        else if (!validateManager.IsValidPassword(inputField_Password.text))
        {
            alertManager.DisplayAlertPopup("Password must be at least 6 characters", new Color32(255, 0, 0, 255));
            return false;
        }
        else if (!validateManager.IsValidConfirmPassword(inputField_Password.text, inputField_ConfirmPassword.text))
        {
            alertManager.DisplayAlertPopup("Password confirmation does not match", new Color32(255, 0, 0, 255));
            return false;
        }
        else if (!validateManager.IsValidEmail(inputField_Email.text))
        {
            alertManager.DisplayAlertPopup("Invalid email address", new Color32(255, 0, 0, 255));
            return false;
        }
        else if (!toggle_PrivacyPolicy.isOn)
        {
            alertManager.DisplayAlertPopup("Please indicate that you have read and agree to the Terms of Service and Privacy Policy", new Color32(255, 0, 0, 255));
            return false;
        }
        else
            return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class ValidateManager
{
    #region Validate

    public bool IsValidUsername(string username)
    {
        if(username.Length < 3 || username.Length > 10)
            return false;
        return true;
    }

    public bool IsValidPassword(string password)
    {
        if (password.Length < 6)
            return false;
        return true;
    }

    public bool IsValidConfirmPassword(string password1, string password2)
    {
        if (password1 != password2)
            return false;
        return true;
    }

    public bool IsValidEmail(string email)
    {
        try
        {
            MailAddress m = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }

    }

    //validate------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //private bool IsValidSignIn()
    //{
    //    txtAlertSignIn.color = new Color32(255, 0, 0, 255);
    //    if (usernameSignIn.text.Length <= 0)
    //    {
    //        txtAlertSignIn.text = "Vui lòng nhập tài khoản!";
    //        return false;
    //    }
    //    else if (passwordSignIn.text.Length <= 0)
    //    {
    //        txtAlertSignIn.text = "Vui lòng nhập mật khẩu!";
    //        return false;
    //    }
    //    return true;
    //}

    //private bool IsValidSignUp()
    //{
    //    txtAlertSignUp.color = new Color32(255, 0, 0, 255);
    //    if (usernameSignUp.text.Length <= 0)
    //    {
    //        txtAlertSignUp.text = "Vui lòng nhập tài khoản!";
    //        return false;
    //    }
    //    else if (passwordSignUp.text.Length <= 0)
    //    {
    //        txtAlertSignUp.text = "Vui lòng nhập mật khẩu!";
    //        return false;
    //    }
    //    else if (confirmPasswordSignUp.text.Length <= 0)
    //    {
    //        txtAlertSignUp.text = "Vui lòng nhập xác nhận mật khẩu!";
    //        return false;
    //    }
    //    else if (emailSignUp.text.Length <= 0)
    //    {
    //        txtAlertSignUp.text = "Vui lòng nhập email!";
    //        return false;
    //    }
    //    else if (!IsValidUsername(usernameSignUp.text))
    //    {
    //        txtAlertSignUp.text = "Tài khoản phải có độ dài từ 3 - 20 ký tự";
    //        return false;
    //    }
    //    else if (!IsValidPassword(passwordSignUp.text))
    //    {
    //        txtAlertSignUp.text = "Mật khẩu phải có độ dài từ 6 - 100 ký tự";
    //        return false;
    //    }
    //    else if (!IsValidPassword2(passwordSignUp.text, confirmPasswordSignUp.text))
    //    {
    //        txtAlertSignUp.text = "Nhập lại Mật khẩu không trùng khớp";
    //        return false;
    //    }
    //    else if (!IsValidEmail(emailSignUp.text))
    //    {
    //        txtAlertSignUp.text = "Email không đúng định dạng";
    //        return false;
    //    }
    //    else
    //    {
    //        return true;
    //    }
    //}

    //private bool IsValidResetPassword()
    //{
    //    txtAlertResetPW.color = new Color32(255, 0, 0, 255);
    //    if (emailResetPW.text.Length <= 0)
    //    {
    //        txtAlertResetPW.text = "Vui lòng nhập email!";
    //        return false;
    //    }
    //    else if (!IsValidEmail(emailResetPW.text))
    //    {
    //        txtAlertResetPW.text = "Email không đúng định dạng";
    //        return false;
    //    }
    //    return true;
    //}


    //private bool IsValidUsername(string username)
    //{
    //    bool isValid = false;
    //    if (username.Length >= 3 && username.Length <= 20)
    //    {
    //        isValid = true;
    //    }
    //    return isValid;
    //}

    //private bool IsValidPassword(string password)
    //{
    //    bool isValid = false;
    //    if (password.Length >= 6)
    //    {
    //        isValid = true;
    //    }
    //    return isValid;
    //}

    //private bool IsValidPassword2(string password1, string password2)
    //{
    //    bool isValid = false;
    //    if (password1 == password2)
    //    {
    //        isValid = true;
    //    }
    //    return isValid;
    //}

    //private bool IsValidEmail(string email)
    //{
    //    bool isValid = false;
    //    try
    //    {
    //        MailAddress m = new MailAddress(email);
    //        isValid = true;
    //    }
    //    catch
    //    {
    //        isValid = false;
    //    }
    //    return isValid;
    //}

    //private bool IsValidNameInput()
    //{
    //    txtAlertSetName.color = new Color32(255, 0, 0, 255);
    //    if (nameInput.text.Length < 5 || nameInput.text.Length > 10)
    //    {
    //        txtAlertSetName.text = "Tên nhân vật phải có độ dài từ 5-10 ký tự!";
    //        return false;
    //    }
    //    return true;
    //}
    #endregion
}

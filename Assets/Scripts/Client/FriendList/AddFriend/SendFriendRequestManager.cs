using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AuthenticationSocketIO;
using static StoreClientSocketIO;

public class SendFriendRequestManager : MonoBehaviour
{
    [SerializeField] private UserInfoJSON userInfo;
    [SerializeField] private Image image_ProfileImage;
    [SerializeField] private TextMeshProUGUI text_Username;
    [SerializeField] private TextMeshProUGUI text_Status;
    [SerializeField] private Button button_CancelRequest;

    private void OnEnable()
    {
        button_CancelRequest.onClick.AddListener(OnClick_CancelRequest);
    }

    private void OnDisable()
    {
        button_CancelRequest.onClick.RemoveListener(OnClick_CancelRequest);
    }

    public void SetUserInfo(UserInfoJSON userInfo)
    {
        this.userInfo = userInfo;
        SetProfileImage(userInfo.profileImage);
        SetUsername(userInfo.username);
        SetStatus(userInfo.status);
    }

    public void SetProfileImage(string profileImage)
    {
        image_ProfileImage.sprite = Resources.Load<Sprite>("textures/profile-image/" + profileImage);
    }
    public void SetUsername(string username)
    {
        text_Username.text = username;
    }

    public void SetStatus(string status)
    {
        text_Status.text = status;
    }

    public void OnClick_CancelRequest()
    {
        SocketIO.instance._friendSocketIO.Emit_CancelFriendRequest(userInfo.username);
    }
}

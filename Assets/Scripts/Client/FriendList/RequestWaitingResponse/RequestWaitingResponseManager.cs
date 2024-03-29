using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SocketIO;
using static AuthenticationSocketIO;

public class RequestWaitingResponseManager : MonoBehaviour
{
    [SerializeField] private UserInfoJSON userInfo;
    [SerializeField] private Image image_ProfileImage;
    [SerializeField] private TextMeshProUGUI text_Username;
    [SerializeField] private TextMeshProUGUI text_Status;
    [SerializeField] private Button button_AcceptRequest;
    [SerializeField] private Button button_DeclineRequest;

    private void OnEnable()
    {
        button_AcceptRequest.onClick.AddListener(OnClick_AcceptRequestWaitingResponse);
        button_DeclineRequest.onClick.AddListener (OnClick_DeclineRequestWaitingResponse);
    }

    private void OnDisable()
    {
        button_AcceptRequest.onClick.RemoveListener(OnClick_AcceptRequestWaitingResponse);
        button_DeclineRequest.onClick.RemoveListener(OnClick_DeclineRequestWaitingResponse);
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

    public void OnClick_AcceptRequestWaitingResponse()
    {
        SocketIO.instance._friendSocketIO.Emit_AccpectRequestWaitingResponse(userInfo);
    }

    public void OnClick_DeclineRequestWaitingResponse()
    {
        SocketIO.instance._friendSocketIO.Emit_DeclineRequestWaitingResponse(userInfo.username);
    }
}

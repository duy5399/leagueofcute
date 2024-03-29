using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SocketIO;
using static AuthenticationSocketIO;

public class FriendSlotManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UserInfoJSON userInfo;
    [SerializeField] private Image image_ProfileImage;
    [SerializeField] private TextMeshProUGUI text_Username;
    [SerializeField] private TextMeshProUGUI text_Status;
    [SerializeField] private Image image_Active;

    public void SetUserInfo(UserInfoJSON userInfo)
    {
        this.userInfo = userInfo;
        SetProfileImage(userInfo.profileImage);
        SetUsername(userInfo.username);
        SetStatus(userInfo.status);
        SetImageStatus(userInfo.online);
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

    public void SetImageStatus(bool isOnline)
    {
        if (isOnline)
            image_Active.color = new Color(2, 158, 61);
        else 
            image_Active.color = new Color(161, 155, 141);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InfoFriendManager.instance.ActiveInfoFriend(true, userInfo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InfoFriendManager.instance.ActiveInfoFriend(false, null);
    }
}

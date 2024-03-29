using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SocketIO;
using static AuthenticationSocketIO;

public class InfoFriendManager : MonoBehaviour
{
    public static InfoFriendManager instance { get; private set; }

    [SerializeField] private UserInfoJSON userInfo;
    [SerializeField] private Image image_ProfileImage;
    [SerializeField] private Image image_Rank;
    [SerializeField] private TextMeshProUGUI text_Username;
    [SerializeField] private TextMeshProUGUI text_Rank;
    [SerializeField] private TextMeshProUGUI text_Status;
    [SerializeField] private TextMeshProUGUI text_LastLogin;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        ActiveInfoFriend(false, null);
    }

    public void ActiveInfoFriend(bool isActive, UserInfoJSON userInfo)
    {
        if(isActive)
        {
            this.userInfo = userInfo;
            SetProfileImage(userInfo.profileImage);
            SetUsername(userInfo.username);
            SetRank(userInfo.rank, userInfo.points);
            SetStatus(userInfo.status);
            SetLastLogin(userInfo.lastLogin);

            transform.localPosition = new Vector2 (415f, Input.mousePosition.y - 150f);
        }
        gameObject.SetActive(isActive);
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

    public void SetRank(string rank, int points)
    {
        text_Rank.text = rank + " (" + points.ToString()+")";
        image_Rank.sprite = Resources.Load<Sprite>("textures/rank-icon/" + rank);
    }

    public void SetLastLogin(string lastLogin)
    {
        text_LastLogin.text = lastLogin;
    }
}

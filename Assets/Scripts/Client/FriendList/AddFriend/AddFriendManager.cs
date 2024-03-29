using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AuthenticationSocketIO;

public class AddFriendManager : MonoBehaviour
{
    public static AddFriendManager instance { get; private set; }

    [SerializeField] private TMP_InputField inputField_FriendName;
    [SerializeField] private Button button_AddFriend;
    [SerializeField] private Transform tf_ContentSendFriendRequest;
    [SerializeField] private GameObject go_Prefab_SendFriendRequest;
    [SerializedDictionary("Friend Request", "GameObject")]
    public SerializedDictionary<UserInfoJSON, GameObject> dict_SendFriendRequest;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        SetSendFriendRequest(SocketIO.instance._friendSocketIO._friendsSendRequest);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        button_AddFriend.onClick.AddListener(OnClick_AddFriend);
    }

    private void OnDisable()
    {
        button_AddFriend.onClick.RemoveListener(OnClick_AddFriend);
    }

    public void SetSendFriendRequest(List<UserInfoJSON> userInfo)
    {
        foreach(var kvp in dict_SendFriendRequest)
        {
            if (!userInfo.Contains(kvp.Key))
            {
                Destroy(kvp.Value);
                dict_SendFriendRequest.Remove(kvp.Key);
            }
        }
        foreach(var user in userInfo)
        {
            if (!dict_SendFriendRequest.ContainsKey(user))
            {
                GameObject obj = Instantiate(go_Prefab_SendFriendRequest, tf_ContentSendFriendRequest);
                obj.GetComponent<SendFriendRequestManager>().SetUserInfo(user);
                dict_SendFriendRequest.Add(user, obj);
            }
        }
    }

    public void CancelFriendRequest(UserInfoJSON request)
    {
        if (request != null)
        {
            Destroy(dict_SendFriendRequest[request]);
            dict_SendFriendRequest.Remove(request);
        }
    }

    private void OnClick_AddFriend()
    {
        if (inputField_FriendName.text.Length > 0)
            SocketIO.instance._friendSocketIO.Emit_SendFriendRequest(inputField_FriendName.text);
    }
}

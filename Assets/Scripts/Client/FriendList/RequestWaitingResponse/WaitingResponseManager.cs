using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SocketIO;
using static AuthenticationSocketIO;

public class WaitingResponseManager : MonoBehaviour
{
    public static WaitingResponseManager instance { get; private set; }

    [SerializeField] private Transform tf_ContentRequestWaitingResponse;
    [SerializeField] private GameObject go_Prefab_RequestWaitingResponse;
    [SerializedDictionary("Friend Request", "GameObject")]
    public SerializedDictionary<UserInfoJSON, GameObject> dict_RequestWaitingResponse;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        SetWaitingResponseRequest(SocketIO.instance._friendSocketIO._friendsWaitingResponse);
        gameObject.SetActive(false);
    }

    public void SetWaitingResponseRequest(List<UserInfoJSON> userInfo)
    {
        foreach (var kvp in dict_RequestWaitingResponse.ToList())
        {
            if (!userInfo.Contains(kvp.Key))
            {
                Destroy(kvp.Value);
                dict_RequestWaitingResponse.Remove(kvp.Key);
            }
        }
        foreach (var user in userInfo)
        {
            if (!dict_RequestWaitingResponse.ContainsKey(user))
            {
                GameObject obj = Instantiate(go_Prefab_RequestWaitingResponse, tf_ContentRequestWaitingResponse);
                obj.GetComponent<RequestWaitingResponseManager>().SetUserInfo(user);
                dict_RequestWaitingResponse.Add(user, obj);
            }
        }
    }

    public void CancelFriendRequest(UserInfoJSON request)
    {
        if (request != null)
        {
            Destroy(dict_RequestWaitingResponse[request]);
            dict_RequestWaitingResponse.Remove(request);
        }
    }
}

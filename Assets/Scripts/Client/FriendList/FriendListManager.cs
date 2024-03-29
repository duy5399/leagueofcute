using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SocketIO;
using static AuthenticationSocketIO;

public class FriendListManager : MonoBehaviour
{
    public static FriendListManager instance { get; private set; }

    [SerializeField] private Transform tf_ContentFriendList;
    [SerializeField] private GameObject go_Prefab_FriendSlot;
    [SerializedDictionary("Friend Request", "GameObject")]
    public SerializedDictionary<UserInfoJSON, GameObject> dict_FriendList;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        SetFriendList(SocketIO.instance._friendSocketIO._friendsAccepted);
    }

    public void SetFriendList(List<UserInfoJSON> userInfo)
    {
        foreach (var kvp in dict_FriendList.ToList())
        {
            if (!userInfo.Contains(kvp.Key))
            {
                Destroy(kvp.Value);
                dict_FriendList.Remove(kvp.Key);
            }
        }
        foreach (var user in userInfo)
        {
            if (!dict_FriendList.ContainsKey(user))
            {
                GameObject obj = Instantiate(go_Prefab_FriendSlot, tf_ContentFriendList);
                obj.GetComponent<FriendSlotManager>().SetUserInfo(user);
                dict_FriendList.Add(user, obj);
            }
        }
    }
}

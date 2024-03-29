using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RoomBattleSocketIO;
using static StoreClientSocketIO;

public class InfoPlayerLoadingManager : MonoBehaviour
{
    [SerializeField] private Image image_Champion;
    [SerializeField] private Image image_ProfileImage;
    [SerializeField] private Image image_Rank;
    [SerializeField] private Image image_Border;
    [SerializeField] private TextMeshProUGUI text_PlayerName;
    [SerializeField] private TextMeshProUGUI text_Points;

    public void SetInfoPlayer(PlayerDataLoadingJSON playerData)
    {
        SetTextPlayerName(playerData.username);
        SetImageChampion(playerData.tacticianEquip);
        SetImageProfileImage(playerData.profileImage);
        SetImagerank(playerData.rank);
        SetTextPoints(playerData.points);
    }

    private void SetTextPlayerName(string username)
    {
        text_PlayerName.text = username;
        if (username == SocketIO.instance._authenticationSocketIO._userInfo.username)
            text_PlayerName.color = new Color(235, 171, 8);
        else
            text_PlayerName.color = new Color(231, 221, 202);
    }

    private void SetTextPoints(int point)
    {
        text_Points.text = point.ToString();
    }

    private void SetImageChampion(string champion)
    {
        ItemInStoreJSON tacticianEquip = SocketIO.instance._storeClientSocketIO._tacticians.FirstOrDefault(x => x.itemID == champion);
        Sprite sprite = Resources.Load<Sprite>("textures/tacticians/" + tacticianEquip.displayImage);
        image_Champion.sprite = sprite;
    }

    private void SetImageProfileImage(string profileImage)
    {
        Sprite sprite = Resources.Load<Sprite>("textures/profile-image/" + profileImage);
        image_ProfileImage.sprite = sprite;
    }

    private void SetImagerank(string rank)
    {
        Sprite spriteRank = Resources.Load<Sprite>("textures/rank-icon/" + rank);
        image_Rank.sprite = spriteRank;
        Sprite spriteBorder = Resources.Load<Sprite>("textures/rank-border/" + rank);
        image_Border.sprite = spriteBorder;
    }
}

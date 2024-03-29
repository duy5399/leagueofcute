using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardPlayerInfoManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txt_PlayerName;
    [SerializeField] private TextMeshProUGUI txt_HP;
    [SerializeField] private Image img_HP;
    [SerializeField] private Image img_Avatar;

    public void SetPlayerName(string playerName)
    {
        txt_PlayerName.text = playerName;
    }

    public void SetHP(int hp, int maxhp)
    {
        txt_HP.text = hp.ToString();
        img_HP.fillAmount = hp / maxhp;
    }

    public void SetAvatar(string profileImage)
    {
        var iconAvatar = Resources.Load<Sprite>("textures/profile-image/" + profileImage);
        img_Avatar.sprite = iconAvatar;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPlayerInfoManager : MonoBehaviour
{
    [SerializeField] private Image image_Champion;
    [SerializeField] private TextMeshProUGUI text_PlayerName;

    public void SetPlayerInfo(string playerName, string champion)
    {
        SetImageChampion(champion);
        SetTextPlayerName(playerName);
    }

    private void SetImageChampion(string champion)
    {
        var sprite = Resources.Load<Sprite>("textures/tacticians/" + champion);
        image_Champion.sprite = sprite;
    }

    private void SetTextPlayerName(string palyerName)
    {
        text_PlayerName.text = palyerName;
    }
}

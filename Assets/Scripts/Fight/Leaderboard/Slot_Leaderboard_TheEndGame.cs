using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnitManagerSocketIO;

public class Slot_Leaderboard_TheEndGame : MonoBehaviour
{
    [SerializeField] private Image imgStanding;
    [SerializeField] private TextMeshProUGUI txtStanding;
    [SerializeField] private Image imgAvatar;
    [SerializeField] private TextMeshProUGUI txtPlayerName;
    [SerializeField] List<GameObject> lstArmy = new List<GameObject>();

    private void Awake()
    {
        
    }

    public void SetImgStanding(int standing)
    {
        switch (standing)
        {
            case 0:
                imgStanding.color = new Color(0.6f, 0.49f, 0.27f, 1);
                break;
            case 1:
                imgStanding.color = new Color(0.44f, 0.47f, 0.51f, 1);
                break;
            case 2:
                imgStanding.color = new Color(0.54f, 0.33f, 0.16f, 1);
                break;
            default:
                imgStanding.color = new Color(0, 0, 0, 0.78f);
                break;
        }
    }

    public void SetTxtStanding(int standing, bool isMine)
    {
        if (isMine)
        {
            txtStanding.text = "<color=#FAD10A>" + standing.ToString() + "</color>";
        }
        else
        {
            txtStanding.text = "<color=#F0E6D2>" + standing.ToString() + "</color>";
        }
    }

    public void SetImgAvatar(string profileImage)
    {
        var iconAvatar = Resources.Load<Sprite>("textures/profile-image/" + profileImage);
        imgAvatar.sprite = iconAvatar;
    }

    public void SetTxtPlayerName(string playerName, bool isMine)
    {
        if (isMine)
        {
            txtPlayerName.text = "<color=#FAD10A>" + playerName + "</color>";
        }
        else
        {
            txtPlayerName.text = "<color=#F0E6D2>" + playerName + "</color>";
        }
    }

    public void SetArmy(List<UnitInfo> army)
    {
        for(int i = 0; i <lstArmy.Count; i++)
        {
            if (army != null && army.ElementAtOrDefault(i) != null)
            {
                lstArmy[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("textures/avatar/avatar_" + army[i].championName);
                lstArmy[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                switch (army[i].currentLevel.star)
                {
                    case 1:
                        lstArmy[i].transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("textures/unit-star/bronzestar");
                        break;
                    case 2:
                        lstArmy[i].transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("textures/unit-star/silverstar");
                        break;
                    case 3:
                        lstArmy[i].transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("textures/unit-star/goldstar");
                        break;
                }
                lstArmy[i].transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);

            }
            else
            {
                lstArmy[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                lstArmy[i].transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0.44f);
                lstArmy[i].transform.GetChild(1).GetComponent<Image>().sprite = null;
                lstArmy[i].transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
        }
    }
}

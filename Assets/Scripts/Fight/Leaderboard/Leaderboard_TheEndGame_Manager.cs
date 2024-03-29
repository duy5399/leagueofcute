using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerDataInBattleSocketIO;
using static UnitManagerSocketIO;

public class Leaderboard_TheEndGame_Manager : MonoBehaviour
{
    public static Leaderboard_TheEndGame_Manager instance { get; private set; }

    [SerializeField] private TextMeshProUGUI txtStanding;
    [SerializeField] private Image imgCurrentRanking;
    [SerializeField] private TextMeshProUGUI txtCurrentPoint;
    [SerializeField] private Transform tfBoard;
    [SerializeField] private GameObject prefabSlotPlayerData;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        this.gameObject.SetActive(false);
    }

    public void SetLeaderboad(PlayerDataJSON[] playerData)
    {
        foreach(PlayerDataJSON i in playerData)
        {
            GameObject obj = Instantiate(prefabSlotPlayerData, tfBoard);
            bool isMine = SocketIO.instance.playerDataInBattleSocketIO.playerData._username == i._username ? true : false;
            obj.GetComponent<Slot_Leaderboard_TheEndGame>().SetImgStanding(i._place);
            obj.GetComponent<Slot_Leaderboard_TheEndGame>().SetTxtStanding(i._place+1, isMine);
            obj.GetComponent<Slot_Leaderboard_TheEndGame>().SetImgAvatar(i._profileImage);
            obj.GetComponent<Slot_Leaderboard_TheEndGame>().SetTxtPlayerName(i._username, isMine);
            List<UnitInfo> unitInfos = new List<UnitInfo>();
            try
            {
                unitInfos = i._battlefield.Fomation().Values.OrderBy(x => x.currentLevel.star).ToList();
            }
            catch
            {
                unitInfos = null;
            }
            obj.GetComponent<Slot_Leaderboard_TheEndGame>().SetArmy(unitInfos);
        }
    }

    public void SetTxtStanding(int standing)
    {
        switch (standing)
        {
            case 0:
                txtStanding.text = "HẠNG NHẤT";
                break;
            case 1:
                txtStanding.text = "HẠNG NHÌ";
                break;
            case 2:
                txtStanding.text = "HẠNG BA";
                break;
            case 3:
                txtStanding.text = "HẠNG TƯ";
                break;
            case 4:
                txtStanding.text = "HẠNG NĂM";
                break;
            case 5:
                txtStanding.text = "HẠNG SÁU";
                break;
            case 6:
                txtStanding.text = "HẠNG BẢY";
                break;
            case 7:
                txtStanding.text = "HẠNG TÁM";
                break;
        }
    }

    public void SetCurrentRanking(string currentRanking)
    {
        switch(currentRanking)
        {
            case "Bronze":
                imgCurrentRanking.sprite = Resources.Load<Sprite>("textures/rank-icon/Season_2023_-_Bronze");
                break;
            case "Silver":
                imgCurrentRanking.sprite = Resources.Load<Sprite>("textures/rank-icon/Season_2023_-_Silver");
                break;
            case "Gold":
                imgCurrentRanking.sprite = Resources.Load<Sprite>("textures/rank-icon/Season_2023_-_Gold");
                break;
            case "Platinum":
                imgCurrentRanking.sprite = Resources.Load<Sprite>("textures/rank-icon/Season_2023_-_Platinum");
                break;
            case "Diamond":
                imgCurrentRanking.sprite = Resources.Load<Sprite>("textures/rank-icon/Season_2023_-_Diamond");
                break;
            case "Master":
                imgCurrentRanking.sprite = Resources.Load<Sprite>("textures/rank-icon/Season_2023_-_Master");
                break;
            case "Grandmaster":
                imgCurrentRanking.sprite = Resources.Load<Sprite>("textures/rank-icon/Season_2023_-_Grandmaster");
                break;
            case "Challenger":
                imgCurrentRanking.sprite = Resources.Load<Sprite>("textures/rank-icon/Season_2023_-_Challenger");
                break;
        }
    }

    public void SetCurrentPoint(int currentPoint, int addPoint)
    {
        if(addPoint > 0)
        {
            txtCurrentPoint.text = currentPoint.ToString() + "<color=green> (+" + addPoint.ToString() + ")</color>";
        }
        else
        {
            txtCurrentPoint.text = currentPoint.ToString() + "<color=red> (-" + addPoint.ToString() + ")</color>";
        }
    }

    public void OnClick_CloseLeaderboard()
    {
        SocketIO.instance.roomBattleSocketIO.Emit_LeaveRoom();
        this.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleResultManager : MonoBehaviour
{
    public static BattleResultManager instance { get; private set; }
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtStanding;
    public Button btnConfirm;

    private void Awake()
    {
        if (instance == null && instance != this)
            instance = this;
        else
            Destroy(this);
        transform.GetComponent<Image>().color = new Color(1, 1, 1, 0.1f);
        this.gameObject.SetActive(false);
    }

    public void OnDisableScreen()
    {
        txtTitle.enabled = false;
        btnConfirm.gameObject.SetActive(false);
        txtStanding.text = "ĐANG CHỜ GIAO TRANH";
        transform.GetComponent<Image>().color = new Color(0, 0, 0, 0.3f);
    }

    public void OnSetBattleResult(int standing)
    {
        txtTitle.enabled = true;
        btnConfirm.gameObject.SetActive(true);
        SetTxtStanding(standing);
        transform.GetComponent<Image>().color = new Color(1, 1, 1, 0.1f);
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

    public void OnClick_GoToLeaderboard()
    {
        //LoadingSceneManager.instance.LoadLeaderboardAsync(1, );
    }
}

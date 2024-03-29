using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindMatchManager : MonoBehaviour
{
    public static FindMatchManager instance { get; private set; }

    [SerializeField] private bool bool_isMatchmaking;
    [SerializeField] private Transform panel_FindingMatch;
    [SerializeField] private TextMeshProUGUI text_FindingTime;

    [SerializeField] private float float_FindingTime;
    [SerializeField] private bool bool_FindingMatch;

    [SerializeField] private Transform panel_MatchFound;
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        ResetMatchmaking();
    }

    private void OnEnable()
    {
        bool_isMatchmaking = false;
    }

    void FixedUpdate()
    {
        CountUpTime();
    }

    public void OnClick_StartMatchmaking()
    {
        if (!bool_isMatchmaking)
        {
            SocketIO.instance._matchmakingSocketIO.Emit_StartMatchmaking();
            bool_FindingMatch = true;
            bool_isMatchmaking = true;
            panel_FindingMatch.gameObject.SetActive(true);
        }
    }

    public void OnClick_StopMatchmaking()
    {
        SocketIO.instance._matchmakingSocketIO.Emit_StopMatchmaking();
        ResetMatchmaking();
        bool_isMatchmaking = false;
        panel_FindingMatch.gameObject.SetActive(false);
    }

    private void ResetMatchmaking()
    {
        float_FindingTime = 0f;
        bool_FindingMatch = false;
    }

    private void CountUpTime()
    {
        if (bool_FindingMatch)
        {
            float_FindingTime += Time.fixedDeltaTime;
            DisplayTime(float_FindingTime);
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        int minutes = (int)timeToDisplay / 60;
        int seconds = (int)timeToDisplay % 60;
        text_FindingTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void MatchFound()
    {
        panel_MatchFound.gameObject.SetActive(true);
    }
}

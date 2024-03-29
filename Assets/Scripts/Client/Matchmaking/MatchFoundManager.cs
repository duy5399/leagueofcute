using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchFoundManager : MonoBehaviour
{
    [SerializeField] private Button button_Accept;
    [SerializeField] private Button button_Decline;
    [SerializeField] private Image image_CooldownTime;
    [SerializeField] private TextMeshProUGUI text_CooldownTime;


    [SerializeField] private float float_TimeToAccept;
    [SerializeField] private float float_RemainingTime;
    [SerializeField] private bool bool_isResponse;

    private void OnEnable()
    {
        bool_isResponse = false;
        float_TimeToAccept = 10f;
        float_RemainingTime = 10f;
        image_CooldownTime.fillAmount = 1f;
        button_Accept.interactable = true;
        button_Decline.interactable = true;
        button_Accept.onClick.AddListener(OnClick_AcceptMatchFound);
        button_Decline.onClick.AddListener(OnClick_DeclineMatchFound);
    }

    private void OnDisable()
    {
        button_Accept.onClick.RemoveListener(OnClick_AcceptMatchFound);
        button_Decline.onClick.RemoveListener(OnClick_DeclineMatchFound);
    }

    void FixedUpdate()
    {
        CountdownTime();
    }

    private void CountdownTime()
    {
        if (float_RemainingTime >= 0)
        {
            float_RemainingTime -= Time.fixedDeltaTime;
            DisplayTime(float_RemainingTime);
        }
        else
        {
            if (bool_isResponse == false)
                OnClick_DeclineMatchFound();
            gameObject.SetActive(false);
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        image_CooldownTime.fillAmount = timeToDisplay / float_TimeToAccept;
        text_CooldownTime.text = ((int)timeToDisplay).ToString();
    }

    private void OnClick_AcceptMatchFound()
    {
        SocketIO.instance._matchmakingSocketIO.Emit_AcceptMatchFound(true);
        bool_isResponse = true;
        button_Accept.interactable = false;
        button_Decline.interactable = false;
    }

    private void OnClick_DeclineMatchFound()
    {
        FindMatchManager.instance.OnClick_StopMatchmaking();
        button_Accept.interactable = false;
        button_Decline.interactable = false;
    }

}

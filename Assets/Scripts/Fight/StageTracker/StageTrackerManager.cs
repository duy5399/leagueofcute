using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageTrackerManager : MonoBehaviour
{
    public static StageTrackerManager instance { get; private set; }

    [SerializeField] private TextMeshProUGUI txt_Stage;
    [SerializeField] private TextMeshProUGUI txt_CountdownTimer;
    [SerializeField] private Slider slider_CountdownTimer;
    [SerializeField] private Image img_ChangePhase;
    [SerializeField] private TextMeshProUGUI txt_Phase;

    private void Awake()
    {
        if (instance == null && instance != this)
            instance = this;
        else
            Destroy(this);

        txt_Stage = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        txt_CountdownTimer = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        slider_CountdownTimer = transform.GetChild(3).GetComponentInChildren<Slider>();
        img_ChangePhase = transform.GetChild(4).GetComponent<Image>();
        txt_Phase = transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        img_ChangePhase.gameObject.SetActive(false);
    }

    public void SetTextStage(string stage)
    {
        txt_Stage.text = stage;
    }

    public void SetTextCountdownTimer(int countdownTimer)
    {
        txt_CountdownTimer.text = countdownTimer.ToString();
    }

    public void SetSliderCountdownTimer(int countdownTimer, int time)
    {
        slider_CountdownTimer.value = (float)countdownTimer / time;
    }

    public void SetNewPhase(string newPhase)
    {
        StartCoroutine(Coroutine_SetNewPhase(newPhase));
    }

    IEnumerator Coroutine_SetNewPhase(string newPhase)
    {
        img_ChangePhase.gameObject.SetActive(true);
        txt_Phase.text = newPhase;
        yield return new WaitForSeconds(3);
        img_ChangePhase.gameObject.SetActive(false);
    }
}

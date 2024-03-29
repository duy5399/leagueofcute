using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Drawing;

public class AlertManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text_Alert;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2 vector3_startPosition = new Vector2(0,-200);
    [SerializeField] private Vector2 vector3_endPosition = new Vector2(0, 0);

    public void DisplayAlertPopup(string text, Color32 color)
    {
        gameObject.SetActive(true);
        AlertText(text);
        AlertColor(color);
        StartCoroutine(DisplayAlertPopup());
    }

    private void AlertText(string text)
    {
        text_Alert.text = text;
    }

    private void AlertColor(Color32 color32)
    {
        text_Alert.color = color32;
    }

    IEnumerator DisplayAlertPopup()
    {
        rectTransform.DOAnchorPos(vector3_endPosition, 0.75f, false).SetEase(Ease.OutExpo);
        transform.GetComponent<Image>().DOFade(1, 0.75f);
        yield return new WaitForSeconds(1f);
        transform.GetComponent<Image>().DOFade(0, 0.75f);
        rectTransform.DOAnchorPos(vector3_startPosition, 0f, false);
        gameObject.SetActive(false);
    }
}

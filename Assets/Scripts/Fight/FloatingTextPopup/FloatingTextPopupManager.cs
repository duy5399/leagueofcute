using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static HealthbarManager;

public class FloatingTextPopupManager : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI txtFloatingText;
    [SerializeField] private Image imageCritical;

    private void Awake()
    {
        txtFloatingText = GetComponentInChildren<TextMeshProUGUI>();
        imageCritical = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    public void SetContentText(string content)
    {
        photonView.RPC(nameof(RPC_SetContentText), RpcTarget.AllBuffered, content);
    }

    [PunRPC]
    public void RPC_SetContentText(string content)
    {
        txtFloatingText.text = content;
    }

    public void SetColorText(int colorStyle)
    {
        photonView.RPC(nameof(RPC_SetColorText), RpcTarget.AllBuffered, colorStyle);
    }

    [PunRPC]
    public void RPC_SetColorText(int colorStyle)
    {
        switch ((ColorStyle)colorStyle)
        {
            case ColorStyle.PhysicalDamage:
            case ColorStyle.CriticalDamage:
                txtFloatingText.color = new Color32(250, 139, 67, 255);
                break;
            case ColorStyle.MagicalDamage:
                txtFloatingText.color = new Color32(12, 169, 242, 255);
                break;
            case ColorStyle.TrueDamage:
                txtFloatingText.color = new Color32(242, 242, 242, 255);
                break;
            case ColorStyle.Heal:
                txtFloatingText.color = new Color32(107, 236, 144, 255);
                break;
        }
    }

    public void SetCritical(bool isActive)
    {
        photonView.RPC(nameof(RPC_SetCritical), RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    public void RPC_SetCritical(bool isActive)
    {
        imageCritical.enabled = isActive;
    }

    public void FloatingTextJumpDown(string content, ColorStyle colorStyle)
    {
        StartCoroutine(Coroutine_FloatingTextJumpDown(content, colorStyle));
    }
    public void FloatingTextDiagonalUp(string content, ColorStyle colorStyle)
    {
        StartCoroutine(Coroutine_FloatingTextDiagonalUp(content, colorStyle));
    }
    public void FloatingTextUp(string content, ColorStyle colorStyle)
    {
        StartCoroutine(Coroutine_FloatingTextUp(content, colorStyle));
    }

    IEnumerator Coroutine_FloatingTextJumpDown(string content, ColorStyle colorStyle)
    {
        //Debug.Log("Coroutine_FloatingTextJumpDown");
        SetContentText(content);
        SetColorText((int)colorStyle);
        SetCritical(false);
        transform.localPosition = Vector3.zero;
        transform.DOScale(1, 0);
        bool isLeft = Random.value > 0.5f;
        Vector3 endValue = isLeft == true ? new Vector3(-0.9f, -0.3f, -1.2f) : new Vector3(1.5f, -0.3f, 0.2f);
        float jumpPower = 1f;
        int numJumps = 1;
        float duration = 1f;
        transform.DOLocalJump(endValue, jumpPower, numJumps, duration).OnPlay(() => transform.DOScale(0.3f, duration)).OnComplete(() => SetActive(false));
        yield return null;
    }

    IEnumerator Coroutine_FloatingTextDiagonalUp(string content, ColorStyle colorStyle)
    {
        //Debug.Log("Coroutine_FloatingTextDiagonalUp");
        SetContentText(content);
        SetColorText((int)colorStyle);
        if(colorStyle == ColorStyle.CriticalDamage)
        {
            SetCritical(true);
        }
        else
        {
            SetCritical(false);
        }
        transform.localPosition = Vector3.zero;
        transform.DOScale(1, 0);
        bool isLeft = Random.value > 0.5f;
        Vector3 endValue = isLeft == true ? new Vector3(-1.3f, 0.2f, 0.3f) : new Vector3(-1f, 0.2f, -0.9f);
        float duration = 1f;
        transform.DOLocalMove(endValue, duration).OnPlay(() => transform.DOScale(0.3f, duration)).OnComplete(() => SetActive(false));
        yield return null;
    }

    IEnumerator Coroutine_FloatingTextUp(string content, ColorStyle colorStyle)
    {
        //Debug.Log("Coroutine_FloatingTextUp: " + content);
        SetContentText(content);
        SetColorText((int)colorStyle);
        SetCritical(false);
        transform.localPosition = Vector3.zero;
        transform.DOScale(1, 0);
        Vector3 endValue = new Vector3(0.5f, 0.6f, -0.9f);
        float duration = 1f;
        transform.DOLocalMove(endValue, duration).OnPlay(() => transform.DOScale(0.3f, duration)).OnComplete(() => SetActive(false));
        yield return null;
    }

    public void SetActive(bool isActive)
    {
        photonView.RPC(nameof(RPC_SetActive), RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    public void RPC_SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SetParent(int photonviewParent, string pathParent = null)
    {
        photonView.RPC(nameof(RPC_SetParent), RpcTarget.AllBuffered, photonviewParent, pathParent);
    }

    [PunRPC]
    void RPC_SetParent(int viewID, string pathParent)
    {
        Transform _parent = PhotonView.Find(viewID).transform;
        if (pathParent == null)
        {
            transform.parent = _parent;
        }
        else
        {
            Transform _pathParent = _parent.Find(pathParent);
            transform.parent = _pathParent;
        }
    }
}

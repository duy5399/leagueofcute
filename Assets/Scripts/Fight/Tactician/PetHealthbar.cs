using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static HealthbarManager;

public class PetHealthbar : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Slider slider_Health;
    [SerializeField] private TextMeshProUGUI txt_Level;
    [SerializeField] private TextMeshProUGUI txt_Username;
    [SerializeField] private List<GameObject> lst_FloatingTextPopup = new List<GameObject>();

    [SerializeField] private string other_username;
    [SerializeField] private float other_slider_Health_Value;
    private void Awake()
    {
        slider_Health = GetComponentInChildren<Slider>();
        txt_Level = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        txt_Username = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        if (photonView.IsMine)
        {
            txt_Username.text = SocketIO.instance.playerDataInBattleSocketIO.playerData._username;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            SetSliderHealth(SocketIO.instance.playerDataInBattleSocketIO.playerData._hp, SocketIO.instance.playerDataInBattleSocketIO.playerData._maxhp);
        }
        else
        {
            txt_Username.text = other_username;
            slider_Health.value = other_slider_Health_Value;
        }
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    public void SetUsername(string username)
    {
        txt_Username.text = username;
    }

    public void SetLevel(int level)
    {
        txt_Level.text = level.ToString();
    }

    public void SetSliderHealth(int currentHealth, int maxHealth)
    {
        slider_Health.value = (float)currentHealth / maxHealth;
    }

    public void SetDamagePopup(int damage)
    {
        if(photonView.IsMine)
        {
            GameObject floatingText = lst_FloatingTextPopup.FirstOrDefault(x => x.activeSelf == false);
            if (floatingText != null)
            {
                floatingText.SetActive(true);
                floatingText.GetComponent<FloatingTextPopupManager>().FloatingTextUp(damage.ToString(), ColorStyle.TrueDamage);
            }
            else
            {
                var prefab_FloatingText = Resources.Load<GameObject>("prefabs/fight/floating-text/DamagePopup");
                GameObject newFloatingText = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/floating-text", prefab_FloatingText.name), transform.position, prefab_FloatingText.transform.rotation);
                newFloatingText.GetComponent<FloatingTextPopupManager>().SetParent(GetComponentInParent<PhotonView>().ViewID);
                newFloatingText.transform.localScale = Vector3.one;
                newFloatingText.GetComponent<FloatingTextPopupManager>().FloatingTextUp(damage.ToString(), ColorStyle.TrueDamage);
                lst_FloatingTextPopup.Add(newFloatingText);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)       //gửi dữ liệu
        {
            stream.SendNext(GetComponentInParent<PetManager>().owner);
            stream.SendNext(slider_Health.value);
            stream.SendNext(SocketIO.instance.playerDataInBattleSocketIO.playerData._username);
        }
        else if (stream.IsReading)  //nhận dữ liệu
        {
            GetComponentInParent<PetManager>().owner = (string)stream.ReceiveNext();
            other_slider_Health_Value = (float)stream.ReceiveNext();
            other_username = (string)stream.ReceiveNext();
        }
    }
}

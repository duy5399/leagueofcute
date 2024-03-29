using Animancer;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarManager : ChampionBase
{
    public enum Mode
    {
        Hero = 0,
        Soldier = 1,
        Monster = 2,
        Boss = 3
    }

    public enum ColorStyle
    {
        PhysicalDamage = 0,
        MagicalDamage = 1,
        CriticalDamage = 2,
        TrueDamage = 3,
        Heal = 4,
        Miss = 5
    }
    public PhotonView pv;
    [SerializeField] private Image image_Star;
    [SerializeField] private Slider slider_HP;
    [SerializeField] private Slider slider_Mana;
    [SerializeField] private Slider slider_Shield;

    [SerializeField] private List<GameObject> lst_FloatingTextPopup = new List<GameObject>();

    [SerializeField] private Transform healthbar;
    [SerializeField] private RectTransform rectHealthbar;
    [SerializeField] private RectTransform rectHP;
    [SerializeField] private RectTransform rectShield;

    [SerializeField] private bool other_rectShield_active;
    public Slider _slider_HP
    {
        get { return slider_HP; } 
        set { slider_HP = value; }
    }
    public Slider _slider_Mana
    {
        get { return slider_Mana; }
        set { slider_Mana = value; }
    }
    protected override void Awake()
    {
        base.Awake();
        pv = GetComponentInParent<PhotonView>();
        image_Star = GetComponentInChildren<Image>();
        healthbar = transform.GetChild(0).GetChild(0);
        slider_HP = healthbar.GetChild(0).GetComponent<Slider>();
        slider_Shield = healthbar.GetChild(1).GetComponent<Slider>();
        slider_Mana = transform.GetChild(0).GetChild(1).GetComponent<Slider>();
        rectHealthbar = (RectTransform)healthbar;
        rectHP = (RectTransform)slider_HP.transform;
        rectShield = (RectTransform)slider_Shield.transform;
    }

    private void Start()
    {

    }


    private void Update()
    {
        if (photonView.IsMine)
        {
            SetSliderHealth(base.currentState.hp, base.currentState.maxHP);
            SetSliderMana(base.currentState.mana, base.currentState.maxMana);
            Shield();
        }
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    public void Shield()
    {
        if (base.currentState.shield <= 0 && !slider_Shield.gameObject.activeSelf)
        {
            return;
        }
        //RectTransform rectHealthbar = (RectTransform)healthbar;
        float healthBarWidth = rectHealthbar.rect.width;
        //float offset = base.currentState.hp + base.currentState.shield <= base.currentState.maxHP ? healthBarWidth : base.currentState.maxHP / (base.currentState.maxHP + base.currentState.shield) * healthBarWidth;
        //float offset3 = base.currentState.hp + base.currentState.shield <= base.currentState.maxHP ? 0 : (base.currentState.maxHP - base.currentState.hp) / base.currentState.hp * offset;
        //rectHP.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, offset + offset3);
        //RectTransform rectShield = (RectTransform)slider_Shield.transform;
        //float offset2 = base.currentState.hp + base.currentState.shield <= base.currentState.maxHP ? base.currentState.shield / base.currentState.maxHP * healthBarWidth : base.currentState.shield / (base.currentState.maxHP + base.currentState.shield) * healthBarWidth;
        //rectShield.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, offset2);            
        float lossHP = base.currentState.maxHP - base.currentState.hp;

        //RectTransform rectShield = (RectTransform)slider_Shield.transform;
        float offset2 = base.currentState.shield <= lossHP ? base.currentState.shield / base.currentState.maxHP * healthBarWidth : base.currentState.shield / (base.currentState.hp + base.currentState.shield) * healthBarWidth;
        rectShield.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, offset2);
        float offset = base.currentState.shield <= lossHP ? healthBarWidth : (healthBarWidth - offset2) / (base.currentState.hp / base.currentState.maxHP);
        rectHP.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, offset);
        float pixelsPerHP = rectHP.rect.width / base.currentState.maxHP;
        rectShield.anchoredPosition3D = new Vector3(pixelsPerHP * base.currentState.hp, 0, 0);
        if (base.currentState.shield == 0)
        {
            slider_Shield.gameObject.SetActive(false);
        }
        else
        {
            slider_Shield.gameObject.SetActive(true);
        }
    }

    public void SetSliderHealth(float currentHealth, float maxHealth)
    {
        slider_HP.value = currentHealth / maxHealth;
    }

    public void SetSliderMana(float currentMana, float maxMana)
    {
        slider_Mana.value = currentMana / maxMana;
    }

    public void SetImageStar1(int star)
    {
        switch (star)
        {
            case 1:
                image_Star.sprite = Resources.Load<Sprite>("textures/healthbar/healthbar-1-star");
                break;
            case 2:
                image_Star.sprite = Resources.Load<Sprite>("textures/healthbar/healthbar-2-star");
                break;
            case 3:
                image_Star.sprite = Resources.Load<Sprite>("textures/healthbar/healthbar-3-star");
                break;
        }
    }

    public void SetDamagePopup(float value, ColorStyle valueStyle)
    {
        try
        {
            GameObject floatingText = lst_FloatingTextPopup.FirstOrDefault(x => x.activeSelf == false);
            if (floatingText != null)
            {
                floatingText.GetComponent<FloatingTextPopupManager>().SetActive(true);
                floatingText.GetComponent<FloatingTextPopupManager>().SetParent(GetComponentInParent<PhotonView>().ViewID, "Weakness");
                floatingText.transform.localScale = Vector3.one;
                switch (valueStyle)
                {
                    case ColorStyle.PhysicalDamage:
                    case ColorStyle.MagicalDamage:
                        floatingText.GetComponent<FloatingTextPopupManager>().FloatingTextJumpDown(Math.Round(value).ToString(), valueStyle);
                        break;
                    case ColorStyle.CriticalDamage:
                    case ColorStyle.TrueDamage:
                        floatingText.GetComponent<FloatingTextPopupManager>().FloatingTextDiagonalUp(Math.Round(value).ToString(), valueStyle);
                        break;
                    case ColorStyle.Heal:
                        floatingText.GetComponent<FloatingTextPopupManager>().FloatingTextUp("+" + Math.Round(value).ToString(), valueStyle);
                        break;
                }
            }
            else
            {
                var prefab_FloatingText = Resources.Load<GameObject>("prefabs/fight/floating-text/DamagePopup");
                GameObject newFloatingText = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/floating-text", prefab_FloatingText.name), transform.position, prefab_FloatingText.transform.rotation);
                newFloatingText.name = "DamagePopup" + base.info.chStat._id;
                newFloatingText.GetComponent<FloatingTextPopupManager>().SetParent(GetComponentInParent<PhotonView>().ViewID, "Weakness");
                newFloatingText.transform.localScale = Vector3.one;
                switch (valueStyle)
                {
                    case ColorStyle.PhysicalDamage:
                    case ColorStyle.MagicalDamage:
                        newFloatingText.GetComponent<FloatingTextPopupManager>().FloatingTextJumpDown(Math.Round(value).ToString(), valueStyle);
                        break;
                    case ColorStyle.CriticalDamage:
                    case ColorStyle.TrueDamage:
                        newFloatingText.GetComponent<FloatingTextPopupManager>().FloatingTextDiagonalUp(Math.Round(value).ToString(), valueStyle);
                        break;
                    case ColorStyle.Heal:
                        newFloatingText.GetComponent<FloatingTextPopupManager>().FloatingTextUp(Math.Round(value).ToString(), valueStyle);
                        break;
                }
                lst_FloatingTextPopup.Add(newFloatingText);
            }
        }
        catch
        {

        }
    }

    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)       //gửi dữ liệu
        {
            stream.SendNext(_slider_HP.value);
            stream.SendNext(_slider_Mana.value);
            float healthBarWidth = rectHealthbar.rect.width;
            float lossHP = base.currentState.maxHP - base.currentState.hp;
            float offset2 = base.currentState.shield <= lossHP ? base.currentState.shield / base.currentState.maxHP * healthBarWidth : base.currentState.shield / (base.currentState.hp + base.currentState.shield) * healthBarWidth;
            float offset = base.currentState.shield <= lossHP ? healthBarWidth : (healthBarWidth - offset2) / (base.currentState.hp / base.currentState.maxHP);
            float pixelsPerHP = rectHP.rect.width / base.currentState.maxHP;
            
            stream.SendNext(offset2);
            stream.SendNext(offset);
            stream.SendNext(new Vector3(pixelsPerHP * base.currentState.hp, 0, 0));
            stream.SendNext(rectShield.gameObject.activeSelf);
        }
        else if (stream.IsReading)  //nhận dữ liệu
        {
            _slider_HP.value = (float)stream.ReceiveNext();
            _slider_Mana.value = (float)stream.ReceiveNext();
            rectShield.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)stream.ReceiveNext());
            rectHP.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)stream.ReceiveNext());
            rectShield.anchoredPosition3D = (Vector3)stream.ReceiveNext();
            rectShield.gameObject.SetActive((bool)stream.ReceiveNext());
            //slider_HP.value = (float)stream.ReceiveNext();
            //slider_Mana.value = (float)stream.ReceiveNext();
            //rectHealthbar.localPosition = (Vector3)stream.ReceiveNext();
            //rectShield.localPosition = (Vector3)stream.ReceiveNext();
            //other_rectShield_active = (bool)stream.ReceiveNext();
        }
    }

    //private void DivisionHP()
    //{
    //    RectTransform rectHP = (RectTransform)slider_HP.transform;
    //    float hpBarWidth = rectHP.rect.width;
    //    float pixelsPerHP = hpBarWidth / base.currentState.maxHP;
    //    var corners = new Vector3[4];
    //    rectHP.GetWorldCorners(corners);
    //    for (int i = 1; i <= hpBarWidth/(pixelsPerHP*300); i++)
    //    {
    //        GameObject startpint = new GameObject("startpint");
    //        startpint.transform.position = corners[1];
    //        GameObject endpint = new GameObject("endpint");
    //        endpint.transform.position = corners[0];
    //        Vector3 startPoint = new Vector3(corners[1].x + (pixelsPerHP * 300), corners[1].y, corners[1].z);
    //        Vector3 endPoint = new Vector3(corners[0].x + (pixelsPerHP * 300), corners[0].y, corners[0].z);
    //        DrawLine(startPoint, endPoint);
    //    }
    //    DrawTriangle(corners[1], corners[0]);
    //    Debug.Log("DivisionHP: hpBarWidth " + hpBarWidth + " pixelsPerHP: " + pixelsPerHP);
    //}

    //public Material mat;
    //private void DrawLine(Vector3 startPoint, Vector3 endPoint)
    //{
    //    if (!mat)
    //    {
    //        Debug.Log("Please Assign a material on the inspector");
    //        return;
    //    }
    //    mat.SetPass(0);
    //    GL.Begin(GL.QUADS);
    //    GL.Color(Color.black);
    //    GL.Vertex(new Vector3(startPoint.x - 0.1f, startPoint.y, startPoint.z));
    //    GL.Vertex(new Vector3(startPoint.x + 0.1f, startPoint.y, startPoint.z));
    //    GL.Vertex(new Vector3(endPoint.x - 0.1f, endPoint.y, endPoint.z));
    //    GL.Vertex(new Vector3(endPoint.x + 0.1f, endPoint.y, endPoint.z));
    //    GL.End();
    //}
    //void DrawTriangle(Vector3 startPoint, Vector3 endPoint)
    //{

    //    lineRenderer.positionCount = 2;
    //    lineRenderer.SetPosition(0, startPoint);
    //    lineRenderer.SetPosition(1, endPoint);
    //}
}
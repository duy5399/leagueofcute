using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnitManagerSocketIO;

public class UnitDescriptionManager : MonoBehaviour
{
    public static UnitDescriptionManager instance { get; private set; }

    [SerializeField] private Transform _unitDescription;
    [SerializeField] private Image image_Card;
    [SerializeField] private Image image_Star;
    [SerializeField] private Image image_SkillIcon;
    [SerializeField] private Image image_Item1;
    [SerializeField] private Image image_Item2;
    [SerializeField] private Image image_Item3;
    [SerializeField] private Slider slider_Health;
    [SerializeField] private Slider slider_Mana;
    [SerializeField] private TextMeshProUGUI text_Health;
    [SerializeField] private TextMeshProUGUI text_Mana;
    [SerializeField] private TextMeshProUGUI text_ChampionName;
    [SerializeField] private Image imgClassIcon;
    [SerializeField] private TextMeshProUGUI txtClassName;
    [SerializeField] private Image imgOriginIcon;
    [SerializeField] private TextMeshProUGUI txtOriginName;
    [SerializeField] private TextMeshProUGUI txtStatAD;
    [SerializeField] private TextMeshProUGUI txtStatAP;
    [SerializeField] private TextMeshProUGUI txtStatAR;
    [SerializeField] private TextMeshProUGUI txtStatMR;
    [SerializeField] private TextMeshProUGUI txtStatAS;
    [SerializeField] private TextMeshProUGUI txtStatAttackRange;
    [SerializeField] private TextMeshProUGUI txtStatCritChance;
    [SerializeField] private TextMeshProUGUI txtStatCritDmg;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        _unitDescription = transform.GetChild(0);
    }

    public Transform unitDescription
    {
        get { return _unitDescription; }
        set { _unitDescription = value; }
    }

    private void Start()
    {
        _unitDescription.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void ActiveUnitDescription(bool isActive, UnitInfo unitStatJSON)
    {
        if (isActive)
        {
            SetImageCard(unitStatJSON.background);
            SetImageStar(unitStatJSON.currentLevel.star);
            SetImageSkillIcon(unitStatJSON.abilityIcon);
            SetTextChampionName(unitStatJSON.championName);
            SetTextHealth((int)unitStatJSON.currentLevel.hp, (int)unitStatJSON.currentLevel.hp);
            SetTextMana((int)unitStatJSON.startMana, (int)unitStatJSON.maxMana);
            SetSliderHealth((int)unitStatJSON.currentLevel.hp, (int)unitStatJSON.currentLevel.hp);
            SetSliderMana((int)unitStatJSON.startMana, (int)unitStatJSON.maxMana);
        }
        gameObject.SetActive(isActive);
    }


    public void SetImageCard(string card)
    {
        var sprite = Resources.Load<Sprite>("textures/card-big/" + card);
        image_Card.sprite = sprite;
    }

    public void SetImageStar(int star)
    {
        var sprite = Resources.Load<Sprite>("textures/hub/unit-des-" + star + "-star");
        image_Star.sprite = sprite;
    }

    public void SetImageSkillIcon(string skillIcon)
    {
        var sprite = Resources.Load<Sprite>("textures/hero-skill/" + skillIcon);
        image_SkillIcon.sprite = sprite;
    }

    public void SetClassIcon(ClassBase.IdClass classIcon)
    {
        switch (classIcon)
        {
            case ClassBase.IdClass.Ranger:
                imgClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_ranger");
                break;
            case ClassBase.IdClass.Assassin:
                imgClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_assassin");
                break;
            case ClassBase.IdClass.Brawler:
                imgClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_brawler");
                break;
            case ClassBase.IdClass.Mystic:
                imgClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_mystic");
                break;
            case ClassBase.IdClass.Defender:
                imgClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_defender");
                break;
            case ClassBase.IdClass.Sorcerer:
                imgClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_sorcerer");
                break;
            case ClassBase.IdClass.Skirmisher:
                imgClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_skirmisher");
                break;
        }
    }

    public void SetClassName(ClassBase.IdClass className)
    {
        switch (className)
        {
            case ClassBase.IdClass.Ranger:
                txtClassName.text = "Cung Thủ";
                break;
            case ClassBase.IdClass.Assassin:
                txtClassName.text = "Sát Thủ";
                break;
            case ClassBase.IdClass.Brawler:
                txtClassName.text = "Đấu Sĩ";
                break;
            case ClassBase.IdClass.Mystic:
                txtClassName.text = "Bí Ẩn";
                break;
            case ClassBase.IdClass.Defender:
                txtClassName.text = "Hộ Vệ";
                break;
            case ClassBase.IdClass.Sorcerer:
                txtClassName.text = "Pháp Sư";
                break;
            case ClassBase.IdClass.Skirmisher:
                txtClassName.text = "Chiến Binh";
                break;
        }
    }

    public void SetOriginIcon(OriginBase.IdOrigin originIcon)
    {
        switch (originIcon)
        {
            case OriginBase.IdOrigin.Mascot:
                imgOriginIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_mascot");
                break;
            case OriginBase.IdOrigin.Hextech:
                imgOriginIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_hextech");
                break;
            case OriginBase.IdOrigin.Yordle:
                imgOriginIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_yordle");
                break;
            case OriginBase.IdOrigin.Nightbringer:
                imgOriginIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_nightbringer");
                break;
            case OriginBase.IdOrigin.Dawnbringer:
                imgOriginIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_dawnbringer");
                break;
            case OriginBase.IdOrigin.Duelist:
                imgOriginIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_duelist");
                break;
        }
    }

    public void SetOriginName(OriginBase.IdOrigin originName)
    {
        switch (originName)
        {
            case OriginBase.IdOrigin.Mascot:
                txtOriginName.text = "Linh Vật";
                break;
            case OriginBase.IdOrigin.Hextech:
                txtOriginName.text = "Công Nghệ";
                break;
            case OriginBase.IdOrigin.Yordle:
                txtOriginName.text = "Yordle";
                break;
            case OriginBase.IdOrigin.Nightbringer:
                txtOriginName.text = "Ma Sứ";
                break;
            case OriginBase.IdOrigin.Dawnbringer:
                txtOriginName.text = "Thần Sứ";
                break;
            case OriginBase.IdOrigin.Duelist:
                txtOriginName.text = "Song Đấu";
                break;
        }
    }

    public void SetImageItem1(string item = null)
    {
        if(item == null)
        {
            image_Item1.sprite = null;
            image_Item1.color = new Color32(118, 118, 118, 255);
        }
        else
        {
            var sprite = Resources.Load<Sprite>("textures/items/" + item);
            image_Item1.sprite = sprite;
            image_Item1.color = new Color32(255, 255, 255, 255);
        }
    }

    public void SetImageItem2(string item = null)
    {
        if (item == null)
        {
            image_Item2.sprite = null;
            image_Item2.color = new Color32(118, 118, 118, 255);
        }
        else
        {
            var sprite = Resources.Load<Sprite>("textures/items/" + item);
            image_Item2.sprite = sprite;
            image_Item2.color = new Color32(255, 255, 255, 255);
        }
    }
    public void SetImageItem3(string item = null)
    {
        if (item == null)
        {
            image_Item3.sprite = null;
            image_Item3.color = new Color32(118, 118, 118, 255);
        }
        else
        {
            var sprite = Resources.Load<Sprite>("textures/items/" + item);
            image_Item3.sprite = sprite;
            image_Item3.color = new Color32(255, 255, 255, 255);
        }
    }

    public void SetTextChampionName(string championName)
    {
        text_ChampionName.text = championName;
    }

    public void SetTextHealth(int currentHealth, int maxHealth)
    {
        text_Health.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    public void SetTextMana(int currentMana, int maxMana)
    {
        text_Mana.text = currentMana.ToString() + "/" + maxMana.ToString();
    }

    public void SetSliderHealth(int currentHealth, int maxHealth)
    {
        slider_Health.value = (float)currentHealth / maxHealth;
    }

    public void SetSliderMana(int currentMana, int maxMana)
    {
        slider_Mana.value = (float)currentMana / maxMana;
    }

    public void SetStatAD(float attackDamage)
    {
        txtStatAD.text = Math.Round(attackDamage).ToString();
    }

    public void SetStatAP(float abilityPower)
    {
        txtStatAP.text = Math.Round(abilityPower).ToString();
    }

    public void SetStatAR(float armor)
    {
        txtStatAR.text = Math.Round(armor).ToString();
    }

    public void SetStatMR(float magicResistance)
    {
        txtStatMR.text = Math.Round(magicResistance).ToString();
    }

    public void SetStatAS(float attackSpeed)
    {
        txtStatAS.text = attackSpeed.ToString();
    }

    public void SetStatAttackRange(int attackRange)
    {
        txtStatAttackRange.text = attackRange.ToString();
    }

    public void SetStatCritChance(float critChance)
    {
        txtStatCritChance.text = (critChance*100).ToString() + "%";
    }

    public void SetStatCritDmg(float critDmg)
    {
        txtStatCritDmg.text = (critDmg*100).ToString() + "%";
    }
}

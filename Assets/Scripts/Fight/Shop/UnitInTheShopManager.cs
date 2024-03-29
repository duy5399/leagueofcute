using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SocketIO;
using static UnityEngine.UI.CanvasScaler;
using static UnitManagerSocketIO;
using static UnitShopManager;

public class UnitInTheShopManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SlotUnitShop slotUnitShop;
    [SerializeField] private UnitInfo unitStatJSON;
    [SerializeField] private Image image_Background;
    [SerializeField] private Image image_Border;
    [SerializeField] private TextMeshProUGUI text_ChampionName;
    [SerializeField] private TextMeshProUGUI text_Cost;
    [SerializeField] private Image image_ClassIcon;
    [SerializeField] private Image image_OriginIcon;
    [SerializeField] private TextMeshProUGUI text_ClassName;
    [SerializeField] private TextMeshProUGUI text_OriginName;

    private void OnEnable()
    {
        transform.GetComponent<Button>().onClick.AddListener(OnClick_BuyUnit);
    }

    private void OnDisable()
    {
        transform.GetComponent<Button>().onClick.RemoveListener(OnClick_BuyUnit);
    }

    public void LoadData(string slotUnitShop, UnitInfo unitStatJSON)
    {
        switch (slotUnitShop)
        {
            case "slot0":
                this.slotUnitShop = SlotUnitShop.slot0; break;
            case "slot1":
                this.slotUnitShop = SlotUnitShop.slot1; break;
            case "slot2":
                this.slotUnitShop = SlotUnitShop.slot2; break;
            case "slot3":
                this.slotUnitShop = SlotUnitShop.slot3; break;
            case "slot4":
                this.slotUnitShop = SlotUnitShop.slot4; break;
        }
        this.unitStatJSON = unitStatJSON;
        SetBorder(unitStatJSON.border);
        SetBackground(unitStatJSON.background);
        SetChampionName(unitStatJSON.championName);
        SetCost(unitStatJSON.buyPrice);
        SetClassIcon(unitStatJSON.idClass);
        SetClassName(unitStatJSON.idClass);
        SetOriginIcon(unitStatJSON.idOrigin);
        SetOriginName(unitStatJSON.idOrigin);
    }

    public void SetBackground(string background)
    {
        var sprite = Resources.Load<Sprite>("textures/card-big/" + background);
        image_Background.sprite = sprite;
    }

    public void SetBorder(string border)
    {
        var sprite = Resources.Load<Sprite>("textures/border-unit/" + border);
        image_Border.sprite = sprite;
    }

    public void SetChampionName(string championName)
    {
        text_ChampionName.text = championName;
    }

    public void SetCost(int cost) 
    {
        text_Cost.text = cost.ToString();
    }

    public void SetClassIcon(ClassBase.IdClass classIcon)
    {
        switch (classIcon)
        {
            case ClassBase.IdClass.Ranger:
                image_ClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_ranger");
                break;
            case ClassBase.IdClass.Assassin:
                image_ClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_assassin");
                break;
            case ClassBase.IdClass.Brawler:
                image_ClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_brawler");
                break;
            case ClassBase.IdClass.Mystic:
                image_ClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_mystic");
                break;
            case ClassBase.IdClass.Defender:
                image_ClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_defender");
                break;
            case ClassBase.IdClass.Sorcerer:
                image_ClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_sorcerer");
                break;
            case ClassBase.IdClass.Skirmisher:
                image_ClassIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_skirmisher");
                break;
        }
    }

    public void SetClassName(ClassBase.IdClass className)
    {
        switch (className)
        {
            case ClassBase.IdClass.Ranger:
                text_ClassName.text = "Cung Thủ";
                break;
            case ClassBase.IdClass.Assassin:
                text_ClassName.text = "Sát Thủ";
                break;
            case ClassBase.IdClass.Brawler:
                text_ClassName.text = "Đấu Sĩ";
                break;
            case ClassBase.IdClass.Mystic:
                text_ClassName.text = "Bí Ẩn";
                break;
            case ClassBase.IdClass.Defender:
                text_ClassName.text = "Hộ Vệ";
                break;
            case ClassBase.IdClass.Sorcerer:
                text_ClassName.text = "Pháp Sư";
                break;
            case ClassBase.IdClass.Skirmisher:
                text_ClassName.text = "Chiến Binh";
                break;
        }
    }

    public void SetOriginIcon(OriginBase.IdOrigin originIcon)
    {
        switch (originIcon)
        {
            case OriginBase.IdOrigin.Mascot:
                image_OriginIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_mascot");
                break;
            case OriginBase.IdOrigin.Hextech:
                image_OriginIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_hextech");
                break;
            case OriginBase.IdOrigin.Yordle:
                image_OriginIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_yordle");
                break;
            case OriginBase.IdOrigin.Nightbringer:
                image_OriginIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_nightbringer");
                break;
            case OriginBase.IdOrigin.Dawnbringer:
                image_OriginIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_dawnbringer");
                break;
            case OriginBase.IdOrigin.Duelist:
                image_OriginIcon.sprite = Resources.Load<Sprite>("textures/traits/trait_icon_duelist");
                break;
        }
    }

    public void SetOriginName(OriginBase.IdOrigin originName)
    {
        switch (originName)
        {
            case OriginBase.IdOrigin.Mascot:
                text_OriginName.text = "Linh Vật";
                break;
            case OriginBase.IdOrigin.Hextech:
                text_OriginName.text = "Công Nghệ";
                break;
            case OriginBase.IdOrigin.Yordle:
                text_OriginName.text = "Yordle";
                break;
            case OriginBase.IdOrigin.Nightbringer:
                text_OriginName.text = "Ma Sứ";
                break;
            case OriginBase.IdOrigin.Dawnbringer:
                text_OriginName.text = "Thần Sứ";
                break;
            case OriginBase.IdOrigin.Duelist:
                text_OriginName.text = "Song Đấu";
                break;
        }
    }

    public void OnClick_BuyUnit()
    {
        if(SocketIO.instance.playerDataInBattleSocketIO.playerData._gold > 0 && SocketIO.instance.playerDataInBattleSocketIO.playerData._gold >= unitStatJSON.buyPrice)
        {
            SocketIO.instance._unitShopSocketIO.Emit_BuyUnit(slotUnitShop);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var abilityIcon = Resources.Load<Sprite>("textures/hero-skill/" + unitStatJSON.abilityIcon);
        string abilityDescription = unitStatJSON.abilityDescription;
        //hitDamage
        if (abilityDescription.Contains("{hitDamage}"))
        {
            float ad = unitStatJSON.ability.hitDamage.ad[0];
            float ap = unitStatJSON.ability.hitDamage.ap[0];
            float trueDmg = unitStatJSON.ability.hitDamage.trueDmg[0];
            float adMultiplier = unitStatJSON.ability.hitDamage.adMultiplier[0];
            float apMultiplier = unitStatJSON.ability.hitDamage.apMultiplier[0];
            int damage = (int)(ad + ap + trueDmg + (unitStatJSON.level[0].attackDamage * adMultiplier) + (unitStatJSON.abilityPower * apMultiplier));
            abilityDescription = abilityDescription.Replace("{hitDamage}", damage.ToString());
        }
        if (abilityDescription.Contains("{ad}"))
        {
            int ad = (int)unitStatJSON.ability.hitDamage.ad[0];
            abilityDescription = abilityDescription.Replace("{ad}", ad.ToString());
        }
        if (abilityDescription.Contains("{ap}"))
        {
            int ap = (int)unitStatJSON.ability.hitDamage.ap[0];
            abilityDescription = abilityDescription.Replace("{ap}", ap.ToString());
        }
        if (abilityDescription.Contains("{adMultiplier}"))
        {
            float adMultiplier = unitStatJSON.ability.hitDamage.adMultiplier[0];
            abilityDescription = abilityDescription.Replace("{adMultiplier}", (adMultiplier*100).ToString() + "%");
        }
        if (abilityDescription.Contains("{apMultiplier}"))
        {
            float apMultiplier = unitStatJSON.ability.hitDamage.apMultiplier[0];
            abilityDescription = abilityDescription.Replace("{apMultiplier}", (apMultiplier * 100).ToString() + "%");
        }
        //aoe
        if (abilityDescription.Contains("{aoe_maxHitNum}"))
        {
            float maxHitNum = unitStatJSON.ability.aoe.maxHitNum;
            abilityDescription = abilityDescription.Replace("{aoe_maxHitNum}", maxHitNum.ToString());
        }
        //bounce
        if (abilityDescription.Contains("{maxBounces}"))
        {
            float maxBounces = unitStatJSON.ability.bounce.maxBounces[0];
            abilityDescription = abilityDescription.Replace("{maxBounces}", maxBounces.ToString());
        }
        //channelling
        if (abilityDescription.Contains("{timeChanneling}"))
        {
            float timeChanneling = unitStatJSON.ability.channelling.timeChanneling;
            abilityDescription = abilityDescription.Replace("{timeChanneling}", timeChanneling.ToString());
        }
        if (abilityDescription.Contains("{tickInterval}"))
        {
            float tickInterval = unitStatJSON.ability.channelling.tickInterval;
            abilityDescription = abilityDescription.Replace("{tickInterval}", tickInterval.ToString());
        }
        if (abilityDescription.Contains("{tickTimes}"))
        {
            float tickTimes = unitStatJSON.ability.channelling.tickTimes;
            abilityDescription = abilityDescription.Replace("{tickTimes}", tickTimes.ToString());
        }
        //crownd control
        if (abilityDescription.Contains("{cc_lifetime}"))
        {
            float lifetime = unitStatJSON.ability.crowdControl.lifeTime[0];
            abilityDescription = abilityDescription.Replace("{cc_lifetime}", lifetime.ToString());
        }
        //heal
        if (abilityDescription.Contains("{heal_healing}"))
        {
            int healing = (int)unitStatJSON.ability.heal.healing[0];
            abilityDescription = abilityDescription.Replace("{heal_healing}", healing.ToString());
        }
        if (abilityDescription.Contains("{heal_MaxHpPercentage}"))
        {
            float maxHpPercentage = unitStatJSON.ability.heal.extraMaxHpPercentage[0];
            abilityDescription = abilityDescription.Replace("{heal_MaxHpPercentage}", (maxHpPercentage*100).ToString() + "%");
        }
        //shield
        if (abilityDescription.Contains("{shield}"))
        {
            float shield_amount = unitStatJSON.ability.shield.shieldAmount[0];
            float maxHpPercentage = unitStatJSON.ability.shield.extraMaxHpPercentage[0];
            float currentHpPercentage = unitStatJSON.ability.shield.extraCurrentHpPercentage[0];
            float missingHpPercentage = unitStatJSON.ability.shield.extraMissingHpPercentage[0];
            int shield = (int)(shield_amount + (maxHpPercentage * unitStatJSON.level[0].hp));
            abilityDescription = abilityDescription.Replace("{shield}", shield.ToString());
        }
        if (abilityDescription.Contains("{shield_amount}"))
        {
            int shield_amount = (int)unitStatJSON.ability.shield.shieldAmount[0];
            abilityDescription = abilityDescription.Replace("{shield_amount}", shield_amount.ToString());
        }
        if (abilityDescription.Contains("{shield_MaxHpPercentage}"))
        {
            float shield_MaxHpPercentage = unitStatJSON.ability.shield.extraMaxHpPercentage[0];
            abilityDescription = abilityDescription.Replace("{shield_MaxHpPercentage}", (shield_MaxHpPercentage*100).ToString() + "%");
        }
        if (abilityDescription.Contains("{shield_ADPercentage}"))
        {
            float shield_ADPercentage = unitStatJSON.ability.shield.extraADPercentage[0];
            abilityDescription = abilityDescription.Replace("{shield_ADPercentage}", (shield_ADPercentage * 100).ToString() + "%");
        }
        if (abilityDescription.Contains("{shield_maxHitNum}"))
        {
            int shield_maxHitNum = unitStatJSON.ability.shield.maxHit[0];
            abilityDescription = abilityDescription.Replace("{shield_maxHitNum}", (shield_maxHitNum - 1).ToString());
        }
        if (abilityDescription.Contains("{shield_lifetime}"))
        {
            float shield_lifetime = unitStatJSON.ability.shield.lifeTime[0];
            abilityDescription = abilityDescription.Replace("{shield_lifetime}", shield_lifetime.ToString());
        }
        //increaseCritChance
        if (abilityDescription.Contains("{increaseCritChance_mult}"))
        {
            float increaseCritChance_mult = unitStatJSON.ability.increaseCritChance.criticalChanceAdd[0];
            abilityDescription = abilityDescription.Replace("{increaseCritChance_mult}", (increaseCritChance_mult * 100).ToString() + "%");
        }
        if (abilityDescription.Contains("{increaseCritChance_lifetime}"))
        {
            float increaseCritChance_lifetime = unitStatJSON.ability.increaseCritChance.lifeTime[0];
            abilityDescription = abilityDescription.Replace("{increaseCritChance_lifetime}", increaseCritChance_lifetime.ToString());
        }
        //increaseCritDMG
        if (abilityDescription.Contains("{increaseCritDMG_mult}"))
        {
            float increaseCritDMG_mult = unitStatJSON.ability.increaseCritDMG.criticalDamageAdd[0];
            abilityDescription = abilityDescription.Replace("{increaseCritDMG_mult}", (increaseCritDMG_mult * 100).ToString() + "%");
        }
        //increaseAD
        if (abilityDescription.Contains("{increaseAD_add}"))
        {
            float increaseAD_add = unitStatJSON.ability.increaseAD.attackDamageAdd[0];
            abilityDescription = abilityDescription.Replace("{increaseAD_add}", increaseAD_add.ToString());
        }
        if (abilityDescription.Contains("{increaseAD_mult}"))
        {
            float increaseAD_mult = unitStatJSON.ability.increaseAD.attackDamageMult[0];
            abilityDescription = abilityDescription.Replace("{increaseAD_mult}", (increaseAD_mult * 100).ToString() + "%");
        }
        if (abilityDescription.Contains("{increaseAD_lifetime}"))
        {
            float increaseAD_lifetime = unitStatJSON.ability.increaseAD.lifeTime[0];
            abilityDescription = abilityDescription.Replace("{increaseAD_lifetime}", increaseAD_lifetime.ToString());
        }
        //increaseAS
        if (abilityDescription.Contains("{increaseAS_mult}"))
        {
            float increaseAS_mult = unitStatJSON.ability.increaseAS.attackSpeedMult[0];
            abilityDescription = abilityDescription.Replace("{increaseAS_mult}", (increaseAS_mult * 100).ToString() + "%");
        }
        if (abilityDescription.Contains("{increaseAS_lifetime}"))
        {
            float increaseAS_lifetime = unitStatJSON.ability.increaseAS.lifeTime[0];
            abilityDescription = abilityDescription.Replace("{increaseAS_lifetime}", increaseAS_lifetime.ToString());
        }
        //increaseAR
        if (abilityDescription.Contains("{increaseAR_add}"))
        {
            int increaseAR_add = (int)unitStatJSON.ability.increaseAR.armorAdd[0];
            abilityDescription = abilityDescription.Replace("{increaseAR_add}", increaseAR_add.ToString());
        }
        if (abilityDescription.Contains("{increaseAR_lifetime}"))
        {
            float increaseAR_lifetime = unitStatJSON.ability.increaseAR.lifeTime[0];
            abilityDescription = abilityDescription.Replace("{increaseAR_lifetime}", increaseAR_lifetime.ToString());
        }
        //increaseMR
        if (abilityDescription.Contains("{increaseMR_add}"))
        {
            int increaseMR_add = (int)unitStatJSON.ability.increaseMR.magicResistanceAdd[0];
            abilityDescription = abilityDescription.Replace("{increaseMR_add}", increaseMR_add.ToString());
        }
        if (abilityDescription.Contains("{increaseMR_lifetime}"))
        {
            float increaseMR_lifetime = unitStatJSON.ability.increaseMR.lifeTime[0];
            abilityDescription = abilityDescription.Replace("{increaseMR_lifetime}", increaseMR_lifetime.ToString());
        }
        //decreaseAS
        if (abilityDescription.Contains("{decreaseAS_mult}"))
        {
            float decreaseAS_mult = unitStatJSON.ability.decreaseAS.attackSpeedMult[0];
            abilityDescription = abilityDescription.Replace("{decreaseAS_mult}", (decreaseAS_mult * 100).ToString() + "%");
        }
        if (abilityDescription.Contains("{decreaseAS_lifetime}"))
        {
            float decreaseAS_lifetime = unitStatJSON.ability.decreaseAS.lifeTime[0];
            abilityDescription = abilityDescription.Replace("{decreaseAS_lifetime}", decreaseAS_lifetime.ToString());
        }
        //decreaseAR
        if (abilityDescription.Contains("{decreaseAR_mult}"))
        {
            float decreaseAR_mult = unitStatJSON.ability.decreaseAR.armorMult[0];
            abilityDescription = abilityDescription.Replace("{decreaseAR_mult}", (decreaseAR_mult * 100).ToString() + "%");
        }
        if (abilityDescription.Contains("{decreaseAR_lifetime}"))
        {
            float decreaseAR_lifetime = unitStatJSON.ability.decreaseAR.lifeTime[0];
            abilityDescription = abilityDescription.Replace("{decreaseAR_lifetime}", decreaseAR_lifetime.ToString());
        }
        //decreaseMR
        if (abilityDescription.Contains("{decreaseMR_mult}"))
        {
            float decreaseMR_mult = unitStatJSON.ability.decreaseMR.magicResistanceMult[0];
            abilityDescription = abilityDescription.Replace("{decreaseMR_mult}", (decreaseMR_mult*100).ToString() + "%");
        }
        if (abilityDescription.Contains("{decreaseMR_lifetime}"))
        {
            float decreaseMR_lifetime = unitStatJSON.ability.decreaseMR.lifeTime[0];
            abilityDescription = abilityDescription.Replace("{decreaseMR_lifetime}", decreaseMR_lifetime.ToString());
        }
        UnitShopManager.instance.ActiveSkillDescription(true, abilityIcon, unitStatJSON.abilityName, abilityDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnitShopManager.instance.ActiveSkillDescription(false, null, null, null);
    }
}

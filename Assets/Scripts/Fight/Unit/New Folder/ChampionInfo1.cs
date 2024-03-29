using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChampionInfo1;
using static UnitManagerSocketIO;

public class ChampionInfo1 : ChampionBase
{
    public enum Categories
    {
        None = 0,
        Hero = 1,
        Monster = 2
    }

    [SerializeField] private Categories _chCategory = Categories.Hero;
    [SerializeField] private UnitInfo _chStat;
    [SerializeField] private string _abilityDescription = string.Empty;


    public Categories chCategory
    {
        get { return _chCategory; }
        set { _chCategory = value; }
    }

    public UnitInfo chStat
    {
        get { return _chStat; }
        set { _chStat = value; }
    }

    public string abilityDescription
    {
        get
        {
            _abilityDescription = _chStat.abilityDescription;
            if (_chStat.abilityDescription.Contains("{hitDamage}"))
            {
                float ad = _chStat.ability.hitDamage.adCanChange == true ? _chStat.ability.hitDamage.ad[_chStat.currentLevel.star - 1] : _chStat.ability.hitDamage.ad[0];
                float ap = _chStat.ability.hitDamage.apCanChange == true ? _chStat.ability.hitDamage.ap[_chStat.currentLevel.star - 1] : _chStat.ability.hitDamage.ap[0];
                float trueDmg = _chStat.ability.hitDamage.trueDmgCanChange == true ? _chStat.ability.hitDamage.trueDmg[_chStat.currentLevel.star - 1] : _chStat.ability.hitDamage.trueDmg[0];
                float adMultiplier = _chStat.ability.hitDamage.adMultiplierCanChange == true ? _chStat.ability.hitDamage.adMultiplier[_chStat.currentLevel.star - 1] : _chStat.ability.hitDamage.adMultiplier[0];
                float apMultiplier = _chStat.ability.hitDamage.apMultiplierCanChange == true ? _chStat.ability.hitDamage.apMultiplier[_chStat.currentLevel.star - 1] : _chStat.ability.hitDamage.apMultiplier[0];
                int damage = (int)(ad + ap + trueDmg + (base.currentState.attackDamage * adMultiplier) + (base.currentState.abilityPower * apMultiplier));
                _abilityDescription.Replace("{hitDamage}", damage.ToString());
            }
            if (_chStat.abilityDescription.Contains("{stun}"))
            {
                float lifetime = _chStat.ability.crowdControl.lifeTimeCanChange == true ? _chStat.ability.crowdControl.lifeTime[_chStat.currentLevel.star - 1] : _chStat.ability.crowdControl.lifeTime[0];
                _abilityDescription.Replace("{stun}", lifetime.ToString());
            }
            return _abilityDescription;
        }
    }

    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)       //gửi dữ liệu
        {
            if (chStat != null)
            {
                stream.SendNext(chStat._id);
                stream.SendNext(chStat.championName);
                stream.SendNext(chStat.background);
                stream.SendNext(chStat.abilityIcon);
                stream.SendNext((int)chStat.idClass);
                stream.SendNext((int)chStat.idOrigin);
                stream.SendNext(chStat.owner);
                stream.SendNext(chStat.currentLevel.star);
                stream.SendNext(chStat.currentLevel.attackDamage);

                stream.SendNext(chStat.attackRange);
                stream.SendNext(chStat.attackSpeed);
                stream.SendNext(chStat.startMana);
                stream.SendNext(chStat.maxMana);
                stream.SendNext(chStat.moveSpeed);
                stream.SendNext(chStat.criticalStrikeChance);
                stream.SendNext(chStat.criticalStrikeDamage);
                stream.SendNext(chStat.armorPenetration);
                stream.SendNext(chStat.armorPenetrationPercentage);
                stream.SendNext(chStat.abilityPower);
                stream.SendNext(chStat.magicPenetration);
                stream.SendNext(chStat.magicPenetrationPercentage);
                stream.SendNext(chStat.armor);
                stream.SendNext(chStat.magicResistance);
                stream.SendNext(chStat.hpRegen);
                stream.SendNext(chStat.manaRegen);
                stream.SendNext(chStat.physicalVamp);
                stream.SendNext(chStat.spellVamp);
}
        }
        else if (stream.IsReading)  //nhận dữ liệu
        {
            if (this.info)
            {
                chStat._id = (string)stream.ReceiveNext();
                chStat.championName = (string)stream.ReceiveNext();
                chStat.background = (string)stream.ReceiveNext();
                chStat.abilityIcon = (string)stream.ReceiveNext();
                chStat.idClass = (ClassBase.IdClass)(int)stream.ReceiveNext();
                chStat.idOrigin = (OriginBase.IdOrigin)(int)stream.ReceiveNext();
                chStat.owner = (string)stream.ReceiveNext();
                chStat.currentLevel.star = (int)stream.ReceiveNext();
                chStat.currentLevel.attackDamage = (float)stream.ReceiveNext();

                chStat.attackRange = (int)stream.ReceiveNext();
                chStat.attackSpeed = (float)stream.ReceiveNext();
                chStat.startMana = (float)stream.ReceiveNext();
                chStat.maxMana = (float)stream.ReceiveNext();
                chStat.moveSpeed = (float)stream.ReceiveNext();
                chStat.criticalStrikeChance = (float)stream.ReceiveNext();
                chStat.criticalStrikeDamage = (float)stream.ReceiveNext();
                chStat.armorPenetration = (float)stream.ReceiveNext();
                chStat.armorPenetrationPercentage = (float)stream.ReceiveNext();
                chStat.abilityPower = (float)stream.ReceiveNext();
                chStat.magicPenetration = (float)stream.ReceiveNext();
                chStat.magicPenetrationPercentage = (float)stream.ReceiveNext();
                chStat.armor = (float)stream.ReceiveNext();
                chStat.magicResistance = (float)stream.ReceiveNext();
                chStat.hpRegen = (float)stream.ReceiveNext();
                chStat.manaRegen = (float)stream.ReceiveNext();
                chStat.physicalVamp = (float)stream.ReceiveNext();
                chStat.spellVamp = (float)stream.ReceiveNext();
            }
        }
    }
}

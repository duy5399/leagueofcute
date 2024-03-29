using BestHTTP.SocketIO3;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnitManagerSocketIO;
using static UnitShopSocketIO;

[Serializable]
public class UnitManagerSocketIO
{
    private SocketManager socketManager;
    public void UnitSocketIOStart(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<string, string, string, string, string>("drag-drop-unit-success", (data1, data2, data3, data4, data5) => {
            On_DragDropUnitSuccess(data1, data2, data3, data4, data5);
        });
        socketManager.Socket.On<string, string>("get-nearest-enemy-success", (unit, nearestEnemy) => {
            On_GetNearestEnemySuccess(unit, nearestEnemy);
        });
        socketManager.Socket.On<string>("get-nearest-enemy-fail", (data) => {
            On_GetNearestEnemyFail(data);
        });
        socketManager.Socket.On<string, int[]>("unit-movement-success", (data1, data2) => {
            On_UnitMovementSuccess(data1, data2);
        });
        socketManager.Socket.On<string>("unit-movement-fail", (data) => {
            On_UnitMovementFail(data);
        });
    }
    #region Listening to events
    private void On_DragDropUnitSuccess(string unitJSON, string previousNode, string previousNodeTag, string selectedNode, string selectedNodeTag)
    {
        Debug.Log("On_DragDropUnitSuccess: " + previousNode + " - " + previousNodeTag + " - " + selectedNode + " - " + selectedNodeTag);
        var unit = JsonConvert.DeserializeObject<UnitInfo>(unitJSON);
        GameObject currentSlot = null;
        GameObject unitDragDrop = null;
        GameObject selectedSlot = null;
        GameObject unitInSelectedSlot = null;
        //thông tin vị trí cũ của unit
        if (previousNodeTag == "Bench")
        {
            currentSlot = BenchManager.instance._dict_Bench.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._nodeName == previousNode).Key;
            unitDragDrop = BenchManager.instance._dict_Bench[currentSlot];
        }
        else
        {
            currentSlot = BattlefieldSideManager.instance.dict_BattlefieldSide.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._nodeName == previousNode).Key;
            unitDragDrop = BattlefieldSideManager.instance.dict_BattlefieldSide[currentSlot];
        }
        //thông tin vị trí mới mà unit chọn
        if (selectedNodeTag == "Bench")
        {
            selectedSlot = BenchManager.instance._dict_Bench.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._nodeName == selectedNode).Key;
            unitInSelectedSlot = BenchManager.instance._dict_Bench[selectedSlot];
            //set unit đến vị trí mới => thay đổi parent và position
            BenchManager.instance.SetDictBench(selectedSlot, unitDragDrop);
        }
        else
        {
            selectedSlot = BattlefieldSideManager.instance.dict_BattlefieldSide.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._nodeName == selectedNode).Key;
            unitInSelectedSlot = BattlefieldSideManager.instance.dict_BattlefieldSide[selectedSlot];
            //set unit cũ cho vị trí mới => thay đổi parent và position
            BattlefieldSideManager.instance.SetDictBattlefield(selectedSlot, unitDragDrop);
        }
        //nếu vị trí mới có đã có unit => chuyển unit mới sang vị trí cũ
        if (selectedSlot != null)
        {
            unitDragDrop.GetComponent<ChampionBase>().SetParent(selectedSlot.GetPhotonView().ViewID);
            unitDragDrop.GetComponent<ChampionBase>().moveManager.positionNode = selectedSlot.GetComponent<SlotManager>()._node;
            unitDragDrop.transform.localPosition = new Vector3(0f, 0f, 0f);
            if (previousNodeTag == "Bench")
            {
                BenchManager.instance.SetDictBench(currentSlot, unitInSelectedSlot);
            }
            else
            {
                BattlefieldSideManager.instance.SetDictBattlefield(currentSlot, unitInSelectedSlot);
            }
            try
            {
                unitInSelectedSlot.GetComponent<ChampionBase>().SetParent(currentSlot.GetPhotonView().ViewID);
                unitInSelectedSlot.transform.localPosition = new Vector3(0f, 0f, 0f);
            }
            catch
            {
                Debug.Log("Drag drop error");
            }
        }
        if(previousNodeTag == "Bench" && selectedNodeTag == "Bench")
        {
            return;
        }
        else if (previousNodeTag == "Battlefield" && selectedNodeTag == "Battlefield")
        {
            return;
        }
        else if(previousNodeTag == "Bench" && selectedNodeTag == "Battlefield")
        {
            if (unitInSelectedSlot != null)
            {
                BattlefieldSideManager.instance.lstCurrentFormation.Remove(unitInSelectedSlot);
                TraitsManager.instance.RemoveClass(unitInSelectedSlot.GetComponent<ChampionInfo1>().info.chStat.idClass, unitInSelectedSlot);
                TraitsManager.instance.RemoveOrigin(unitInSelectedSlot.GetComponent<ChampionInfo1>().info.chStat.idOrigin, unitInSelectedSlot);
            }
            BattlefieldSideManager.instance.lstCurrentFormation.Add(unitDragDrop);
            TraitsManager.instance.AddClass(unit.idClass, unitDragDrop);
            TraitsManager.instance.AddOrigin(unit.idOrigin, unitDragDrop);
        }
        else if(previousNodeTag == "Battlefield" && selectedNodeTag == "Bench")
        {
            BattlefieldSideManager.instance.lstCurrentFormation.Remove(unitDragDrop);
            TraitsManager.instance.RemoveClass(unit.idClass, unitDragDrop);
            TraitsManager.instance.RemoveOrigin(unit.idOrigin, unitDragDrop);
            if (unitInSelectedSlot != null)
            {
                BattlefieldSideManager.instance.lstCurrentFormation.Add(unitInSelectedSlot);
                TraitsManager.instance.AddClass(unitInSelectedSlot.GetComponent<ChampionInfo1>().info.chStat.idClass, unitInSelectedSlot);
                TraitsManager.instance.AddOrigin(unitInSelectedSlot.GetComponent<ChampionInfo1>().info.chStat.idOrigin, unitInSelectedSlot);
            }
        }
    }
    private void On_GetNearestEnemySuccess(string unitStatJSON, string enemyStat)
    {
        var unitStat = JsonConvert.DeserializeObject<UnitInfo>(unitStatJSON);
        var nearestEnemy = JsonConvert.DeserializeObject<UnitInfo>(enemyStat);
        GameObject target = GameObject.Find(nearestEnemy.championName + "_" + nearestEnemy._id);
        GameObject unit = GameObject.Find(unitStat.championName + "_" + unitStat._id);
        unit.GetComponentInChildren<MoveManager>().SetNearestTarget(target);
    }
    private void On_GetNearestEnemyFail(string unitStatJSON)
    {
        var unitStat = JsonConvert.DeserializeObject<UnitInfo>(unitStatJSON);
        foreach (var i in BattlefieldSideManager.instance.dict_BattlefieldSide.Values)
        {
            if (i != null && i.GetComponent<ChampionBase>().info.chStat._id == unitStat._id && i.GetComponent<ChampionBase>().info.chStat.championName == unitStat.championName)
            {
                i.GetComponent<ChampionInfo1>().info.stateCtrl.inCombat = false;
                break;
            }
        }
    }
    private void On_UnitMovementSuccess(string unitStatJSON, int[] node)
    {
        var unitStat = JsonConvert.DeserializeObject<UnitInfo>(unitStatJSON);
        Debug.Log("On_UnitMovementSuccess: " + unitStat.championName +"_"+ unitStat._id + " -> " + node[0] +"," + node[1]);
        foreach (var i in BattlefieldSideManager.instance.dict_BattlefieldSide.Values)
        {
            if (i != null && i.GetComponent<ChampionBase>().info.chStat._id == unitStat._id && i.GetComponent<ChampionBase>().info.chStat.championName == unitStat.championName)
            {
                i.GetComponentInChildren<MoveManager>().MovingToTarget(node);
                break;
            }
        }
    }
    private void On_UnitMovementFail(string unitStatJSON)
    {
        var unitStat = JsonConvert.DeserializeObject<UnitInfo>(unitStatJSON);
        Debug.Log("On_UnitMovementFail: " + unitStat.championName + "_" + unitStat._id );
        foreach (var i in BattlefieldSideManager.instance.dict_BattlefieldSide.Values)
        {
            if (i != null && i.GetComponent<ChampionBase>().info.chStat._id == unitStat._id && i.GetComponent<ChampionBase>().info.chStat.championName == unitStat.championName)
            {
                i.GetComponentInChildren<MoveManager>().UnitMovementFail();
                break;
            }
        }
    }
    #endregion
    #region Emitting events
    public void Emit_DragDropUnit(UnitInfo unitStatJSON, string selectedNode, string selectedNodeTag)
    {
        socketManager.Socket.Emit("drag-drop-unit", unitStatJSON, selectedNode, selectedNodeTag);
    }
    public void Emit_GetNearestEnemy(UnitInfo unit, BattlefieldName battlefieldName)
    {
        Debug.Log("Emit_GetNearestEnemy");
        socketManager.Socket.Emit("get-nearest-enemy", unit, battlefieldName.ToString());
    }
    public void Emit_UnitMovement(UnitInfo unit, BattlefieldName battlefieldName, UnitInfo target, float unitHP, float targetHP, bool isOpponent, bool isTargetMoving)
    {
        Debug.Log("Emit_UnitMovement: unit " + unit.championName+unit._id  +" target " + target.championName + " unithp: " + unitHP + " - targetHP: " + targetHP + "  - isOpponent: " + isOpponent);
        socketManager.Socket.Emit("unit-movement", unit, battlefieldName.ToString(), target, unitHP, targetHP, isOpponent, isTargetMoving);
    }
    public void Emit_RemoveUnitFromBattlefield(UnitInfo unit, BattlefieldName battlefieldName)
    {
        Debug.Log("Emit_RemoveUnitFromBattlefield");
        socketManager.Socket.Emit("remove-unit-from-battlefield", unit, battlefieldName.ToString());
    }
    #endregion

    [Serializable]
    public class UnitStatJSON
    {
        public string _id;
        public string championName;
        public int tier;
        public Level[] level;
        public int buyPrice;
        public string border;
        public string background;
        public string owner;

        public int attackRange;
        public Level currentLevel;
        public float startMana;
        public float maxMana;
        public float moveSpeed;
        public float criticalStrikeChance;
        public float criticalStrikeDamage;
        public float attackSpeed;
        public float armorPenetration;
        public float armorPenetrationPercentage;
        public float abilityPower;
        public float magicPenetration;
        public float magicPenetrationPercentage;
        public float armor;
        public float magicResistance;
        public float hpRegen;
        public float manaRegen;
        public float physicalVamp;
        public float spellVamp;

        public string abilityName;
        public string abilityIcon;
        public string abilityDescription;

        [Serializable]
        public class Level
        {
            public int star;
            public int hp;
            public int attackDamage;
            public int sellPrice;
        }
    }

    [Serializable]
    public class UnitInfo
    {
        public string _id;
        public string championName;
        public int tier;
        public int buyPrice;
        public string border;
        public string background;
        public string owner;
        public Level[] level;
        public Level currentLevel;
        public ClassBase.IdClass idClass;
        public OriginBase.IdOrigin idOrigin;
        public int attackRange;
        public float attackSpeed;
        public float startMana;
        public float maxMana;
        public float moveSpeed;
        public float criticalStrikeChance;
        public float criticalStrikeDamage;
        public float armorPenetration;
        public float armorPenetrationPercentage;
        public float abilityPower;
        public float magicPenetration;
        public float magicPenetrationPercentage;
        public float armor;
        public float magicResistance;
        public float hpRegen;
        public float manaRegen;
        public float physicalVamp;
        public float spellVamp;

        public SkillBase1.Details basicAttack;
        public SkillBase1.Details ability;

        public string abilityName;
        public string abilityIcon;
        public string abilityDescription;

        [Serializable]
        public class Level
        {
            public int star;
            public float hp;
            public float attackDamage;
            public int sellPrice;
        }
    }
}

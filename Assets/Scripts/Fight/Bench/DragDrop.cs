using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnitManagerSocketIO;

public class DragDrop : ChampionBase
{
    [SerializeField] private Transform _currentParent;
    [SerializeField] private Transform selectingDropPosition;
    [SerializeField] private float float_posY;
    [SerializeField] private Vector3 v3_offset;
    [SerializeField] private LayerMask layerMask_destinationTag;
    [SerializeField] private const string BENCH_TAG = "Bench";
    [SerializeField] private const string BATTLEFIELD_SIDE_TAG = "Battlefield";
    [SerializeField] private bool isSellUnit = false;

    public Transform currentParent
    {
        get { return _currentParent; }
        set { _currentParent = value; }
    }

    private void OnMouseDown()
    {
        if (photonView.IsMine && base.info.chCategory == ChampionInfo1.Categories.Hero)
        {
            if (base.stateCtrl.inCombat)
            {
                return;
            }
            //trong CombatPhase chỉ có thể kéo thả unit trên "Hàng chờ"
            if (SocketIO.instance.playerDataInBattleSocketIO.playerData._phase == "CombatPhase" /* && SocketIO.instance._playerDataInBattleSocketIO._playerData.BenchContains(unitManager._unitStatJSON)*/)
            {
                BattlefieldSideManager.instance.SetActiveBattlefieldSide(false);
            }
            //trong PlanningPhase có thể kéo thả unit trên "Hàng chờ" và "Sân đấu"
            if (SocketIO.instance.playerDataInBattleSocketIO.playerData._phase == "PlanningPhase")
            {
                BattlefieldSideManager.instance.SetActiveBattlefieldSide(true);
            }
            BenchManager.instance.SetActiveBench(true);
            v3_offset = Input.mousePosition - MouseWorldPosition();
            transform.GetComponent<Collider>().enabled = false;
            float_posY = transform.position.y + 2f;
            transform.position = new Vector3(transform.position.x, float_posY, transform.position.x);
        }
    }

    private void OnMouseDrag()
    {
        if (photonView.IsMine && base.info.chCategory == ChampionInfo1.Categories.Hero)
        {
            if (SocketIO.instance.playerDataInBattleSocketIO.playerData._phase == "CombatPhase"/* && SocketIO.instance._playerDataInBattleSocketIO._playerData.BenchContains(unitManager._unitStatJSON)*/)
                return;
            else
            {
                //kích hoạt chỗ bán unit
                UnitShopManager.instance.ActiveSellUnit(true, base.info.chStat);
                //di chuyển unit theo chuột
                Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition - v3_offset);
                transform.position = new Vector3(newPos.x, float_posY, newPos.z);
                //tạo 1 raycast từ camera
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 100f;
                var dir = Camera.main.ScreenToWorldPoint(mousePos) - Camera.main.transform.position;
                RaycastHit hit;
                //nếu chiếu tới các object có layer là DropArea => active slot được chiếu tới
                if (Physics.Raycast(Camera.main.transform.position, dir, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("DropArea")))
                {
                    selectingDropPosition = hit.transform;
                    if (selectingDropPosition.tag == BENCH_TAG || selectingDropPosition.tag == BATTLEFIELD_SIDE_TAG)
                    {
                        BenchManager.instance.ActiveSelectDropPosition(selectingDropPosition.gameObject);
                        BattlefieldSideManager.instance.ActiveSelectDropPosition(selectingDropPosition.gameObject);
                    }
                }
                else
                {
                    if (selectingDropPosition != null)
                    {
                        selectingDropPosition.GetComponent<SlotManager>().ActiveBorder(false);
                        selectingDropPosition = null;
                    }
                }
                Debug.DrawRay(Camera.main.transform.position, dir, Color.green);
            }
        }
    }

    private void OnMouseUp()
    {
        if (photonView.IsMine && base.info.chCategory == ChampionInfo1.Categories.Hero)
        {
            //bán unit
            var pointerEventData = new PointerEventData(null);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);
            if (raycastResults.Count > 0 && raycastResults.Exists(x => x.gameObject.tag == "SellUnit"))
                isSellUnit = true;
            else
                isSellUnit = false;
            UnitShopManager.instance.ActiveSellUnit(false, null);
            if (isSellUnit)
            {
                SellUnit();
                return;
            }
            if (selectingDropPosition != null && selectingDropPosition.gameObject.GetPhotonView().IsMine)
            {
                if ((BenchManager.instance._dict_Bench.ContainsKey(selectingDropPosition.gameObject) && BenchManager.instance._dict_Bench[selectingDropPosition.gameObject] == this.gameObject) 
                    || (BattlefieldSideManager.instance.dict_BattlefieldSide.ContainsKey(selectingDropPosition.gameObject) && BattlefieldSideManager.instance.dict_BattlefieldSide[selectingDropPosition.gameObject] == this.gameObject)
                    || (BenchManager.instance._dict_Bench.ContainsValue(this.gameObject) && selectingDropPosition.tag == BATTLEFIELD_SIDE_TAG 
                    && BattlefieldSideManager.instance.dict_BattlefieldSide[selectingDropPosition.gameObject] == null 
                    && BattlefieldSideManager.instance.dict_BattlefieldSide.Values.Where(x => x != null).ToList().Count >= SocketIO.instance.playerDataInBattleSocketIO.playerData._maxUnitInBattlefield))
                {
                    transform.localPosition = new Vector3(0f, 0f, 0f);
                }
                else
                {
                    SocketIO.instance._unitManagerSocketIO.Emit_DragDropUnit(base.info.chStat, selectingDropPosition.GetComponent<SlotManager>()._nodeName, selectingDropPosition.transform.tag);
                    selectingDropPosition = null;
                }
            }
            else
                transform.localPosition = new Vector3(0f, 0f, 0f);
            transform.GetComponent<Collider>().enabled = true;
            BenchManager.instance.SetActiveBench(false);
            BattlefieldSideManager.instance.SetActiveBattlefieldSide(false);
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            UnitDescriptionManager.instance.gameObject.SetActive(true);
            UnitDescriptionManager.instance.unitDescription.gameObject.SetActive(true);
            UnitDescriptionManager.instance.SetImageCard(base.info.chStat.background);
            UnitDescriptionManager.instance.SetImageStar(base.info.chStat.currentLevel.star);
            UnitDescriptionManager.instance.SetImageSkillIcon(base.info.chStat.abilityIcon);
            UnitDescriptionManager.instance.SetTextChampionName(base.info.chStat.championName);
            UnitDescriptionManager.instance.SetClassIcon(base.info.chStat.idClass);
            UnitDescriptionManager.instance.SetClassName(base.info.chStat.idClass);
            UnitDescriptionManager.instance.SetOriginIcon(base.info.chStat.idOrigin);
            UnitDescriptionManager.instance.SetOriginName(base.info.chStat.idOrigin);
            UnitDescriptionManager.instance.SetTextHealth((int)base.currentState.hp, (int)base.currentState.maxHP);
            UnitDescriptionManager.instance.SetTextMana((int)base.currentState.mana, (int)base.currentState.maxMana);
            UnitDescriptionManager.instance.SetSliderHealth((int)base.currentState.hp, (int)base.currentState.maxHP);
            UnitDescriptionManager.instance.SetSliderMana((int)base.currentState.mana, (int)base.currentState.maxMana);
            UnitDescriptionManager.instance.SetStatAD(base.currentState.attackDamage);
            UnitDescriptionManager.instance.SetStatAP(base.currentState.abilityPower);
            UnitDescriptionManager.instance.SetStatAR(base.currentState.armor);
            UnitDescriptionManager.instance.SetStatMR(base.currentState.magicResistance);
            UnitDescriptionManager.instance.SetStatAS(base.currentState.attackSpeed);
            UnitDescriptionManager.instance.SetStatAttackRange(base.currentState.attackRange);
            UnitDescriptionManager.instance.SetStatCritChance(base.currentState.criticalStrikeChance);
            UnitDescriptionManager.instance.SetStatCritDmg(base.currentState.criticalStrikeDamage);
            for (int i = 0; i < 3; i++)
            {
                if (i < base.items.itemLst.Count && base.items.itemLst[i])
                {
                    Debug.Log("DragDrop OnMouseOver: " + base.items.itemLst[i].GetComponent<ItemBase>().item.icon);
                    switch (i)
                    {
                        case 0:
                            UnitDescriptionManager.instance.SetImageItem1(base.items.itemLst[i].GetComponent<ItemBase>().item.icon); break;
                        case 1:
                            UnitDescriptionManager.instance.SetImageItem2(base.items.itemLst[i].GetComponent<ItemBase>().item.icon); break;
                        case 2:
                            UnitDescriptionManager.instance.SetImageItem3(base.items.itemLst[i].GetComponent<ItemBase>().item.icon); break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            UnitDescriptionManager.instance.SetImageItem1(); break;
                        case 1:
                            UnitDescriptionManager.instance.SetImageItem2(); break;
                        case 2:
                            UnitDescriptionManager.instance.SetImageItem3(); break;
                    }
                }
            }
        }
    }

    Vector3 MouseWorldPosition()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    public void SellUnit()
    {
        SocketIO.instance._unitShopSocketIO.Emit_SellUnit(base.info.chStat);
    }

}

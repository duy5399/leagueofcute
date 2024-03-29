using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WebSocketSharp;
using static BattlefieldSideManager;
using static MoveManager;
using static UnitManagerSocketIO;

public class MoveManager : ChampionBase
{
    public enum State
    {
        StandStill = 0,
        FindingNearestTarget = 1,
        MovingToTarget = 2
    }
    [SerializeField] private int[] _positionNode;
    [SerializeField] private State _moveState;
    [SerializeField] private bool _locked;
    [SerializeField] private GameObject nearestTarget;

    public State moveState
    {
        get { return _moveState; }
        set { _moveState = value; }
    }

    public bool locked
    {
        get { return _locked; }
        set { _locked = value; }
    }

    public int[] positionNode
    {
        get { return _positionNode; }
        set { _positionNode = value; }
    }

    public float moveSpeed
    {
        get { 
            return (base.currentState != null) ? base.currentState.moveSpeed : 0f;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        locked = false;
        moveState = State.StandStill;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            UpdateMove();
        }
    }

    private void UpdateMove()
    {
        if(base.currentState.hp <=0 || base.currentState.dead)
        {
            return;
        }
        if (SocketIO.instance.playerDataInBattleSocketIO.playerData._phase != "CombatPhase" || base.stateCtrl.inCombat == false || base.currentState.attackDisable == true)
        {
            StandStill();
            return;
        }
        else
        {
            if((base.skills.currentCasting != null && !base.skills.currentCasting.details.canMoveWhenCast) || base.currentState.moveDisable)
            {
                return;
            }
            if (!locked)
            {
                if (nearestTarget == null || nearestTarget.GetComponent<ChampionBase>().currentState.hp <= 0 || nearestTarget.GetComponent<ChampionBase>().currentState.dead)
                {
                    if (moveState == State.FindingNearestTarget || moveState == State.MovingToTarget)
                        {
                        Debug.Log("State.FindingNearestTarget: " + base.info.name);
                        return;
                    }
                    moveState = State.FindingNearestTarget;
                    SocketIO.instance._unitManagerSocketIO.Emit_GetNearestEnemy(base.info.chStat, SocketIO.instance._battlefieldSocketIO.battlefieldName);
                }
                if (nearestTarget != null && nearestTarget.GetComponent<ChampionBase>().currentState.hp > 0 && !nearestTarget.GetComponent<ChampionBase>().currentState.dead)
                {
                    if(moveState == State.MovingToTarget)
                    {
                        Debug.Log("State.MovingToTarget: " + base.info.name);
                        return;
                    }
                    if (moveState == State.StandStill)
                    {
                        ChampionInfo1 component = nearestTarget.GetComponent<ChampionBase>().info;
                        if (component)
                        {
                            int[] node = new int[2];
                            if (base.info.chStat.owner != "PvE" && component.chStat.owner != "PvE")
                            {
                                node[0] = 5 - component.moveManager.positionNode[0];
                                node[1] = 5 - component.moveManager.positionNode[1];
                            }
                            else
                            {
                                node = component.moveManager.positionNode;
                            }
                            Debug.Log("moveManager GetDistance: " + positionNode[0] + "_" + positionNode[1] + " - " + node[0] + "_" + node[1] + " : " + GetDistance(positionNode, node) + " > " + base.info.chStat.championName + "_" + base.info.chStat._id + " - " + base.currentState.attackRange + " _hp: " + base.currentState.hp);
                            if (GetDistance(positionNode, node) > base.currentState.attackRange)
                            {
                                Debug.Log("moveManager GetDistance >: " + node[0] + "_" + node[1]);
                                moveState = State.MovingToTarget;
                                UnitMovement();
                            }
                        }
                    }
                }
            }
            else
            {
                StandStill();
            }
        }
    }

    public void SetNearestTarget(GameObject nearestTarget)
    {
        this.nearestTarget = nearestTarget;
        base.skills.target = nearestTarget;
        moveState = State.StandStill;
    }

    public void LookAt(GameObject target)
    {
        if (target)
        {
            Vector3 direction = (target.transform.position - transform.parent.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    public void StandStill()
    {
        if (base.skills && !base.skills.currentCasting && base.anim != null)
        {
            base.anim.TriggerIdle();
        }
    }

    public void UnitMovement()
    {
        ChampionInfo1 targetInfo = nearestTarget.GetComponent<ChampionBase>().info;
        bool isOpponent = targetInfo.skills.target == base.info.gameObject ? true : false;
        bool isTargetMoving = targetInfo.moveManager.moveState == State.MovingToTarget ? true : false;
        SocketIO.instance._unitManagerSocketIO.Emit_UnitMovement(base.info.chStat, SocketIO.instance._battlefieldSocketIO.battlefieldName, targetInfo.chStat, base.info.currentState.hp, targetInfo.currentState.hp, isOpponent, isTargetMoving);
    }

    public void MovingToTarget(int[] newNode)
    {
        GameObject nextCellToMove = BattlefieldSideManager.instance.dict_BattlefieldSide.FirstOrDefault(x => x.Key.GetComponent<SlotManager>()._node.SequenceEqual(newNode)).Key;
        if (nextCellToMove != null)
        {
            GameObject currentCell = BattlefieldSideManager.instance.dict_BattlefieldSide.FirstOrDefault(x => x.Value == base.info.gameObject).Key;
            StartCoroutine(Coroutine_MoveToTarget(currentCell, nextCellToMove));
        }
    }

    IEnumerator Coroutine_MoveToTarget(GameObject currentCell, GameObject nextCellToMove)
    {
        yield return new WaitForSeconds(0.25f);
        BattlefieldSideManager.instance.dict_BattlefieldSide[currentCell] = null;
        BattlefieldSideManager.instance.dict_BattlefieldSide[nextCellToMove] = base.info.gameObject;
        base.info.SetParent(nextCellToMove.gameObject.GetPhotonView().ViewID);
        while (transform.position != nextCellToMove.transform.position && base.currentState.hp > 0 && !base.currentState.dead)
        {
            if (base.anim != null)
            {
                base.anim.TriggerRun(true);
            }
            LookAt(nextCellToMove);
            base.info.transform.position = Vector3.MoveTowards(base.info.transform.position, nextCellToMove.transform.position, moveSpeed* Time.deltaTime);
            yield return null;
        }
        positionNode = nextCellToMove.GetComponent<SlotManager>()._node;
        moveState = State.StandStill;
        StandStill();
        LookAt(nearestTarget);
        nearestTarget = null;
    }

    public void UnitMovementFail()
    {
        Debug.Log("UnitMovementFail: " + base.info.name + " - " + positionNode[0] + "_" + positionNode[1]);
        moveState = State.StandStill;
        StandStill();
        LookAt(nearestTarget);
    }

    public int GetDistance(int[] posSelf, int[] posTarget)
    {
        int x1 = posSelf[0];
        int y1 = posSelf[1];
        int x2 = posTarget[0];
        int y2 = posTarget[1];
        int dx = x2 - x1;
        int dy = y2 - y1;
        int x = Math.Abs(dx);
        int y = Math.Abs(dy);
        // special case if we start on an odd row or if we move into negative x direction 
        if ((dy < 0) ^ ((x1 & 1) == 1))
            y = Math.Max(0, y - (x / 2));
        else
            y = Math.Max(0, y - (x + 1) / 2);
        return x + y;
    }

    public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)       //gửi dữ liệu
        //{
        //    stream.SendNext(positionNode);
        //}
        //else if (stream.IsReading)  //nhận dữ liệu
        //{
        //    positionNode = (int[])stream.ReceiveNext();
        //}
    }
}

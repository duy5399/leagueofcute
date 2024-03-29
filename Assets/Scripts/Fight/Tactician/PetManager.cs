using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static HealthbarManager;

public class PetManager : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Animator animator;
    [SerializeField] private string _owner;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private PetHealthbar petHealthBar;
    [SerializeField] private GameObject _myBoom;
    [SerializeField] private GameObject posAttack;

    public Animator _animator => animator;
    public NavMeshAgent navMeshAgent {
        get { return _navMeshAgent; }
        set { _navMeshAgent = value; }
    }

    public string owner
    {
        get { return _owner; }
        set { _owner = value; }
    }

    public GameObject myBoom
    {
        get { return _myBoom; }
        set { _myBoom = value; }
    }
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        petHealthBar = GetComponentInChildren<PetHealthbar>();
        posAttack = transform.GetChild(3).gameObject;
    }
    private void Start()
    {
        //photonView.RPC(nameof(RPC_SetUsername), RpcTarget.AllBuffered, SocketIO.instance.playerDataInBattleSocketIO.playerData._username);
        //photonView.RPC(nameof(RPC_SetLevel), RpcTarget.AllBuffered, SocketIO.instance.playerDataInBattleSocketIO.playerData._level);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine && pv.gameObject.tag == "Item")
            {
                Debug.Log("OnTriggerEnter: " + pv.gameObject.name);
                ItemBase itemBase = pv.gameObject.GetComponent<ItemBase>();
                ItemDragDrop itemDragDrop = pv.gameObject.GetComponent<ItemDragDrop>();
                if (!itemBase.isEquipped && itemDragDrop.currentParent == null)
                {
                    itemDragDrop.MoveToStorage();
                }
            }
            else
            {
                Debug.Log("!OnTriggerEnter");
            }
        }
    }

    public void DealDamage(GameObject target, int damage)
    {
        Debug.Log("DealDamage: ");
        GameObject boom = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/booms", myBoom.name), posAttack.transform.position, posAttack.transform.rotation);
        boom.GetComponent<ProjectileMover>().target = target;
        boom.GetComponent<ProjectileMover>().isActive = true;
    }

    public void TakeDamage(int damage)
    {
        animator.SetTrigger("Damage");
        petHealthBar.SetDamagePopup(damage);
    }

    public void SetLevel(int level)
    {
        petHealthBar.SetLevel(level);
    }
    public void MoveTactician(int viewIDParent)
    {
        transform.parent.GetComponent<PlayerPosManager>().ChangeParent(gameObject.GetPhotonView().ViewID, viewIDParent);
    }
    public void SetLocalPosition(Vector3 newPosition)
    {
        photonView.RPC(nameof(RPC_SetLocalPosition), RpcTarget.AllBuffered, newPosition);
        //navMeshAgent.enabled = false;
        //transform.localPosition = newPosition;
        ////navMeshAgent.enabled = true;
    }

    public void SetLocalRotation(Vector3 newRotation)
    {
        transform.localRotation = Quaternion.Euler(newRotation.x, newRotation.y, newRotation.z);
    }

    public void ChangeName(string name)
    {
        photonView.RPC(nameof(RPC_ChangeName), RpcTarget.AllBuffered, name);
    }

    public void ChangeParent(int photonviewParent)
    {
        photonView.RPC(nameof(RPC_ChangeParent), RpcTarget.AllBuffered, photonviewParent);
    }

    [PunRPC]
    void RPC_SetLocalPosition(Vector3 newPosition)
    {
        navMeshAgent.enabled = false;
        transform.localPosition = newPosition;
        navMeshAgent.enabled = true;
    }

    [PunRPC]
    void RPC_SetUsername(string username)
    {
        petHealthBar.SetUsername(username);
    }

    [PunRPC]
    void RPC_SetLevel(int level)
    {
        petHealthBar.SetLevel(level);
    }

    [PunRPC]
    void RPC_ChangeName(string name)
    {
        gameObject.name = name;
    }

    [PunRPC]
    void RPC_ChangeParent(int viewID)
    {
        Transform _parent = PhotonView.Find(viewID).transform;
        transform.parent = _parent;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}

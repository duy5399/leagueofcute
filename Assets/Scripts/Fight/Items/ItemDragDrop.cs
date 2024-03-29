using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDragDrop : MonoBehaviourPun
{
    [SerializeField] private bool canDragDrop;
    [SerializeField] private Transform _currentParent;
    [SerializeField] private Transform _selectingDropChampion;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float posY;

    public Transform currentParent
    {
        get { return _currentParent; }
        set { _currentParent = value; }
    }
    public Transform selectingDropChampion
    {
        get { return _selectingDropChampion; }
        set { _selectingDropChampion = value; }
    }

    private void Awake()
    {
        canDragDrop = false;
        currentParent = null;
    }

    private void OnMouseDown()
    {
        if (!photonView.IsMine || !canDragDrop)
        {
            return;
        }
        if (!transform.GetComponent<ItemBase>().isEquipped)
        {
            offset = Input.mousePosition - MouseWorldPosition();
            //transform.GetComponent<Collider>().enabled = false;
            posY = transform.position.y + 2f;
            transform.position = new Vector3(transform.position.x, posY, transform.position.x);
        }
    }

    private void OnMouseDrag()
    {
        if (!photonView.IsMine || !canDragDrop)
        {
            return;
        }
        if ( !transform.GetComponent<ItemBase>().isEquipped)
        {
            //di chuyển unit theo chuột
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition - offset);
            transform.position = new Vector3(newPos.x, posY, newPos.z);
            //tạo 1 raycast từ camera
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 100f;
            var dir = Camera.main.ScreenToWorldPoint(mousePos) - Camera.main.transform.position;
            RaycastHit hit;
            //nếu chiếu tới các object có layer là Champion => active slot được chiếu tới
            if (Physics.Raycast(Camera.main.transform.position, dir, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Champion")))
            {
                selectingDropChampion = hit.transform;
            }
            else
            {
                if (selectingDropChampion != null)
                {
                    selectingDropChampion = null;
                }
            }
            Debug.DrawRay(Camera.main.transform.position, dir, Color.green);
        }
    }

    private void OnMouseUp()
    {
        if (!photonView.IsMine || !canDragDrop)
        {
            return;
        }
        if (selectingDropChampion != null)
        {
            if (selectingDropChampion.gameObject.GetPhotonView().IsMine)
            {
                TryOnEquip();
                selectingDropChampion = null;
            }
            else
            {
                MoveToStorage();
            }
        }
        else
        {
            MoveToStorage();
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ItemBase itemBase = transform.GetComponent<ItemBase>();
            ItemDescriptionManager.instance.gameObject.SetActive(true);
            ItemDescriptionManager.instance.itemDescription.gameObject.SetActive(true);
            ItemDescriptionManager.instance.SetImgIcon(itemBase.item.icon);
            ItemDescriptionManager.instance.SetTxtName(itemBase.item.name);
            ItemDescriptionManager.instance.SetTxtStats(itemBase.item.descriptionStat);
            ItemDescriptionManager.instance.SetTxtPassive(itemBase.item.descriptionPassive);
            
        }
    }

    Vector3 MouseWorldPosition()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    //-1: không thể trang bị, 0: có thể trang bị, 1: có thể nâng cấp
    public int CheckAfterEquip(ItemBase itemBase, ItemManager itemManager)
    {
        if (itemManager.currentState.maxItem - itemManager.currentItem < itemBase.item.slotRequired)
        {
            return -1;
        }
        else
        {
            if (itemBase.item.typeItem == Item.TypeItem.BasicItem)
            {
                List<GameObject> listBasicItem = itemManager.itemLst.FindAll(x => x.GetComponent<ItemBase>().item.typeItem == Item.TypeItem.BasicItem);
                if (listBasicItem == null)
                {
                    return 0;
                }
                else
                {
                    bool canCombine = false;
                    foreach (GameObject item in listBasicItem)
                    {
                        if(canCombine)
                        {
                            break;
                        }
                        ItemBase itemBase1 = item.GetComponent<ItemBase>();
                        switch (itemBase.item.idItem)
                        {
                            case "bf_sword":
                                if (itemBase1.item.recipe.bf_sword != string.Empty)
                                {
                                    canCombine = true;
                                }
                                break;
                            case "recurve_bow":
                                if (itemBase1.item.recipe.recurve_bow != string.Empty)
                                {
                                    canCombine = true;
                                }
                                break;
                            case "chain_vest":
                                if (itemBase1.item.recipe.chain_vest != string.Empty)
                                {
                                    canCombine = true;
                                }
                                break;
                            case "negatron_cloak":
                                if (itemBase1.item.recipe.negatron_cloak != string.Empty)
                                {
                                    canCombine = true;
                                }
                                break;
                            case "needlessly_large_rod":
                                if (itemBase1.item.recipe.needlessly_large_rod != string.Empty)
                                {
                                    canCombine = true;
                                }
                                break;
                            case "tear_of_the_goddess":
                                if (itemBase1.item.recipe.tear_of_the_goddess != string.Empty)
                                {
                                    canCombine = true;
                                }
                                break;
                            case "giants_belt":
                                if (itemBase1.item.recipe.giants_belt != string.Empty)
                                {
                                    canCombine = true;
                                }
                                break;
                            case "sparring_gloves":
                                if (itemBase1.item.recipe.sparring_gloves != string.Empty)
                                {
                                    canCombine = true;
                                }
                                break;
                        }
                    }
                    if (canCombine) {
                        return 1;
                    }
                    return 0;
                    //if (itemManager.itemLst.FirstOrDefault(x => x.GetComponent<ItemBase>().item.typeItem == Item.TypeItem.BasicItem) == null)
                    //{
                    //    return false;
                    //}
                }
            }
            else
            {
                if (itemBase.item.isUnique && itemManager.itemLst.FirstOrDefault(x => x.GetComponent<ItemBase>().item.idItem == itemBase.item.idItem) != null)
                {
                    return -1;
                }
                return 0;
            }
        }
    }

    public void TryOnEquip()
    {
        ItemBase itemBase = transform.GetComponent<ItemBase>();
        if (!itemBase.isEquipped)
        {
            if (selectingDropChampion != null && selectingDropChampion.gameObject.GetPhotonView().IsMine)
            {
                ChampionInfo1 chInfo = selectingDropChampion.gameObject.GetComponent<ChampionInfo1>();
                if (chInfo != null && !chInfo.currentState.dead)
                {
                    int stateEquip = CheckAfterEquip(itemBase, chInfo.items);
                    if (stateEquip == -1)
                    {
                        Debug.Log("CheckAfterEquip cant equip");
                        MoveToStorage();
                    }
                    else if (stateEquip == 0)
                    {
                        Debug.Log("CheckAfterEquip can equip");
                        if(currentParent != null)
                        {
                            ItemsDropManager.instance.dictItemDrop[currentParent.gameObject] = null;
                        }
                        currentParent = chInfo.items.AddItem(this.gameObject).transform;
                        itemBase.OnEquip();
                        selectingDropChampion = null;
                    }
                    else
                    {
                        ItemsDropManager.instance.dictItemDrop[currentParent.gameObject] = null;
                        List<GameObject> listBasicItem = chInfo.items.itemLst.FindAll(x => x.GetComponent<ItemBase>().item.typeItem == Item.TypeItem.BasicItem);
                        foreach (GameObject item in listBasicItem)
                        {
                            ItemBase itemBase1 = item.GetComponent<ItemBase>();
                            switch (itemBase.item.idItem)
                            {
                                case "bf_sword":
                                    if (itemBase1.item.recipe.bf_sword != string.Empty)
                                    {
                                        SocketIO.instance.itemsSocketIO.Emit_CombineItem(chInfo.chStat, itemBase1.item, itemBase.item);
                                        if (photonView.IsMine)
                                        {
                                            PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
                                        }
                                        return;
                                    }
                                    break;
                                case "recurve_bow":
                                    if (itemBase1.item.recipe.recurve_bow != string.Empty)
                                    {
                                        SocketIO.instance.itemsSocketIO.Emit_CombineItem(chInfo.chStat, itemBase1.item, itemBase.item);
                                        if (photonView.IsMine)
                                        {
                                            PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
                                        }
                                        return;
                                    }
                                    break;
                                case "chain_vest":
                                    if (itemBase1.item.recipe.chain_vest != string.Empty)
                                    {
                                        SocketIO.instance.itemsSocketIO.Emit_CombineItem(chInfo.chStat, itemBase1.item, itemBase.item);
                                        if (photonView.IsMine)
                                        {
                                            PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
                                        }
                                        return;
                                    }
                                    break;
                                case "negatron_cloak":
                                    if (itemBase1.item.recipe.negatron_cloak != string.Empty)
                                    {
                                        SocketIO.instance.itemsSocketIO.Emit_CombineItem(chInfo.chStat, itemBase1.item, itemBase.item);
                                        if (photonView.IsMine)
                                        {
                                            PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
                                        }
                                        return;
                                    }
                                    break;
                                case "needlessly_large_rod":
                                    if (itemBase1.item.recipe.needlessly_large_rod != string.Empty)
                                    {
                                        SocketIO.instance.itemsSocketIO.Emit_CombineItem(chInfo.chStat, itemBase1.item, itemBase.item);
                                        if (photonView.IsMine)
                                        {
                                            PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
                                        }
                                        return;
                                    }
                                    break;
                                case "tear_of_the_goddess":
                                    if (itemBase1.item.recipe.tear_of_the_goddess != string.Empty)
                                    {
                                        SocketIO.instance.itemsSocketIO.Emit_CombineItem(chInfo.chStat, itemBase1.item, itemBase.item);
                                        if (photonView.IsMine)
                                        {
                                            PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
                                        }
                                        return;
                                    }
                                    break;
                                case "giants_belt":
                                    if (itemBase1.item.recipe.giants_belt != string.Empty)
                                    {
                                        SocketIO.instance.itemsSocketIO.Emit_CombineItem(chInfo.chStat, itemBase1.item, itemBase.item);
                                        if (photonView.IsMine)
                                        {
                                            PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
                                        }
                                        return;
                                    }
                                    break;
                                case "sparring_gloves":
                                    if (itemBase1.item.recipe.bf_sword != string.Empty)
                                    {
                                        SocketIO.instance.itemsSocketIO.Emit_CombineItem(chInfo.chStat, itemBase1.item, itemBase.item);
                                        if (photonView.IsMine)
                                        {
                                            PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
                                        }
                                        return;
                                    }
                                    break;
                            }
                        }
                        Debug.Log("CheckAfterEquip combine");
                    }
                }
            }
            else
            {
                MoveToStorage();
            }
            //transform.GetComponent<Collider>().enabled = true;
        }
    }

    public void MoveToStorage()
    {
        StartCoroutine(Coroutine_MoveToStorage());
    }

    IEnumerator Coroutine_MoveToStorage()
    {
        if(currentParent == null || !ItemsDropManager.instance.dictItemDrop.ContainsKey(currentParent.gameObject))
        {
            GameObject slotEmpty = ItemsDropManager.instance.dictItemDrop.FirstOrDefault(x => x.Value == null).Key;
            if (slotEmpty != null)
            {
                currentParent = slotEmpty.transform;
                ItemsDropManager.instance.dictItemDrop[slotEmpty]= this.gameObject;
                gameObject.GetComponent<PhotonView>().Synchronization = ViewSynchronization.Off;
                SetParent(slotEmpty.GetPhotonView().ViewID);
                //item.transform.parent = slotEmpty.transform;
                //transform.localScale = new Vector3(15, 15, 15);
                //transform.localRotation = Quaternion.Euler(0, 180, 0);
                SetLocalScale(new Vector3(15, 15, 15));
                SetLocalRotation(new Vector3(0, 180, 0));
                while (transform.localPosition != new Vector3(0, 5.5f, 0))
                {
                    Vector3 position = Vector3.Lerp(transform.localPosition, new Vector3(0, 5.5f, 0), 5 * Time.deltaTime);
                    SetLocalPosition(position);
                    //Debug.Log("MoveToStorage: " + transform.localPosition);
                    yield return null;
                }
                //transform.localPosition = new Vector3(0, 5.5f, 0);
                canDragDrop = true;
            }
        }
        else
        {
            while (transform.localPosition != new Vector3(0, 5.5f, 0))
            {
                //transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 5.5f, 0), 5 * Time.deltaTime);
                Vector3 position = Vector3.Lerp(transform.localPosition, new Vector3(0, 5.5f, 0), 5 * Time.deltaTime);
                SetLocalPosition(position);
                Debug.Log("MoveToStorage: " + transform.localPosition);
                yield return null;
            }
            //transform.localPosition = new Vector3(0, 5.5f, 0);
            canDragDrop = true;
        }
    }

    public void SetParent(int photonviewParent)
    {
        photonView.RPC(nameof(RPC_SetParent), RpcTarget.AllBuffered, photonviewParent);
    }

    public void SetParentOnChampion(int photonviewParent)
    {
        photonView.RPC(nameof(RPC_SetParentOnChampion), RpcTarget.AllBuffered, photonviewParent);
    }

    public void SetLocalPosition(Vector3 position)
    {
        photonView.RPC(nameof(RPC_SetLocalPosition), RpcTarget.AllBuffered, position);
    }

    public void SetLocalRotation(Vector3 rotation)
    {
        photonView.RPC(nameof(RPC_SetLocalRotation), RpcTarget.AllBuffered, rotation);
    }

    public void SetLocalScale(Vector3 scale)
    {
        photonView.RPC(nameof(RPC_SetLocalScale), RpcTarget.AllBuffered, scale);
    }

    [PunRPC]
    void RPC_SetParent(int viewID)
    {
        Transform _parent = PhotonView.Find(viewID).transform;
        transform.parent = _parent;
        Debug.Log("RPC_SetParent: " + _parent.name);
    }

    [PunRPC]
    void RPC_SetParentOnChampion(int viewID)
    {
        Transform _parent = PhotonView.Find(viewID).transform;
        transform.parent = _parent.GetComponent<ChampionBase>().items.transform;
        Debug.Log("RPC_SetParentOnChampion: " + _parent.GetComponent<ChampionBase>().items.transform.name);
    }

    [PunRPC]
    void RPC_SetLocalPosition(Vector3 position)
    {
        transform.localPosition = position;
    }

    [PunRPC]
    void RPC_SetLocalRotation(Vector3 rotation)
    {
        transform.localRotation = Quaternion.Euler(rotation);
    }

    [PunRPC]
    void RPC_SetLocalScale(Vector3 scale)
    {
        transform.localScale = scale;
    }
}

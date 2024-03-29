using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SkillSpawn1;

public class ItemManager : ChampionBase
{
    [SerializeField] private List<GameObject> _itemLst;
    [SerializeField] private int _currentItem;

    public List<GameObject> itemLst
    {
        get { return _itemLst; }
        set { _itemLst = value; }
    }

    public int currentItem
    {
        get { return _currentItem; }
    }

    protected override void Awake()
    {
        base.Awake();
        _itemLst = new List<GameObject>();
        _currentItem = 0;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    public bool EquipItem(GameObject item)
    {
        ItemBase itemBase = item.GetComponent<ItemBase>();
        if ((itemBase.item.isUnique && _itemLst.FirstOrDefault(x => x.GetComponent<ItemBase>().item.idItem == itemBase.item.idItem) != null) || (base.currentState.maxItem - currentItem < itemBase.item.slotRequired))
        {
            return false;
        }
        if (itemBase.item.typeItem == Item.TypeItem.BasicItem)
        {
            GameObject item1 = _itemLst.FirstOrDefault(x => x.GetComponent<ItemBase>().item.typeItem == Item.TypeItem.BasicItem);
            if (item1 != null)
            {
                //SocketIO.instance.itemsSocketIO.Emit_CombineItem(item1.GetComponent<ItemBase>().item.id, itemBase.item.id);
                return true;
            }
        }
        AddItem(item);
        return true; ;
    }

    public GameObject AddItem(GameObject item)
    {
        //_itemLst.Add(item);
        base.SetAddItem(item.GetPhotonView().ViewID);
        ItemDragDrop itemDragDrop = item.GetComponent<ItemDragDrop>();
        itemDragDrop.SetParentOnChampion(base.info.gameObject.GetPhotonView().ViewID);
        itemDragDrop.SetLocalRotation(new Vector3(0, 180, 0));
        itemDragDrop.SetLocalScale(new Vector3(1, 1, 1));
        int index = _itemLst.IndexOf(item);
        switch (index)
        {
            case 0:
                itemDragDrop.SetLocalPosition(new Vector3(-1.02f, 0, 0));
                break;
            case 1:
                itemDragDrop.SetLocalPosition(new Vector3(0, 0, 0));
                break;
            case 2:
                itemDragDrop.SetLocalPosition(new Vector3(0, 0, 1.02f));
                break;
        }
        _currentItem += item.GetComponent<ItemBase>().item.slotRequired;
        return _itemLst[index];
    }

    public void RemoveItem(GameObject item)
    {
        item.GetComponent<ItemBase>().OnUnequip();
        _currentItem -= item.GetComponent<ItemBase>().item.slotRequired;
        _itemLst.Remove(item);
    }

    public void OnCombineItem(Item item)
    {

    }

    public void OnBasicAttack(SkillBase1 skillBase, Transform target)
    {
        foreach (var i in _itemLst)
        {
            ItemBase itemBase = i.GetComponent<ItemBase>();
            itemBase.OnBasicAttack(skillBase, target);
        }
    }

    public void OnSpecialAbility(SkillBase1 skillBase, Transform target)
    {
        if (photonView.IsMine)
        {
            foreach (var i in _itemLst)
            {
                Debug.Log(i.name + " OnSpecialAbility");
                ItemBase itemBase = i.GetComponent<ItemBase>();
                itemBase.OnSpecialAbility(skillBase, target);
            }
        }
    }

    public virtual void OnHit(Transform target, float damage, bool isCritical)
    {
        if (photonView.IsMine)
        {
            foreach (var i in _itemLst)
            {
                ItemBase itemBase = i.GetComponent<ItemBase>();
                itemBase.OnHit(target, damage, isCritical);
            }
        }
    }

    public virtual void OnBeHited(Transform caster, float damage, bool isCritical)
    {
        if (photonView.IsMine)
        {
            foreach (var i in _itemLst)
            {
                Debug.Log(i.name + " OnBeHited");
                ItemBase itemBase = i.GetComponent<ItemBase>();
                itemBase.OnBeHited(caster, damage, isCritical);
            }
        }
    }

    public void OnReset()
    {
        foreach (var item in _itemLst)
        {
            if (item != null)
            {
                item.GetComponent<ItemBase>().OnReset();
            }
        }
    }

    //public override void PhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)       //gửi dữ liệu
    //    {
    //        try
    //        {
    //            stream.SendNext(_itemLst[0].GetPhotonView().ViewID);
    //        }
    //        catch
    //        {
    //            Debug.Log("PhotonSerializeView error SendNext item 0");
    //        }
    //        try
    //        {
    //            stream.SendNext(_itemLst[1].GetPhotonView().ViewID);
    //        }
    //        catch
    //        {
    //            Debug.Log("PhotonSerializeView error SendNext item 1");
    //        }
    //        try
    //        {
    //            stream.SendNext(_itemLst[2].GetPhotonView().ViewID);
    //        }
    //        catch
    //        {
    //            Debug.Log("PhotonSerializeView error SendNext item 2");
    //        }
    //    }
    //    else if (stream.IsReading)  //nhận dữ liệu
    //    {
    //        try
    //        {
    //            int viewID = (int)stream.ReceiveNext();
    //            GameObject item = PhotonView.Find(viewID).gameObject;
    //            _itemLst[0] = item;
    //        }
    //        catch
    //        {
    //            Debug.Log("PhotonSerializeView error ReceiveNext item 0");
    //        }
    //        try
    //        {
    //            int viewID = (int)stream.ReceiveNext();
    //            GameObject item = PhotonView.Find(viewID).gameObject;
    //            _itemLst[1] = item;
    //        }
    //        catch
    //        {
    //            Debug.Log("PhotonSerializeView error ReceiveNext item 1");
    //        }
    //        try
    //        {
    //            int viewID = (int)stream.ReceiveNext();
    //            GameObject item = PhotonView.Find(viewID).gameObject;
    //            _itemLst[2] = item;
    //        }
    //        catch
    //        {
    //            Debug.Log("PhotonSerializeView error ReceiveNext item 2");
    //        }
    //    }
    //}
}

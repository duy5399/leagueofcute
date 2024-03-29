using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChampionDrop : ChampionBase
{
    [SerializeField] private List<GameObject> _itemDrop;
    [SerializeField] private bool isDrop;

    public List<GameObject> itemDrop
    {
        get { return _itemDrop; }
        set { _itemDrop = value; }
    }

    private void FixedUpdate()
    {
        if(base.currentState.dead && base.currentState.hp <= 0 && !isDrop)
        {
            isDrop = true;
            DropItem();
        }
    }

    public void AddItemDrop(Item item)
    {
        if(item == null)
        {
            return;
        }
        Debug.Log("AddItemDrop: " + item.name);
        GameObject obj = Resources.Load<GameObject>("prefabs/fight/items/" + item.idItem);
        if (obj != null)
        {
            obj.GetComponent<ItemBase>().item = item;
            itemDrop.Add(obj);
        }
    }

    public void AddCoinDrop(string coin)
    {
        if (coin == null)
        {
            return;
        }
        Debug.Log("AddCoinDrop: " + coin);
        GameObject obj = Resources.Load<GameObject>("prefabs/fight/items/" + coin);
        if (obj != null)
        {
            itemDrop.Add(obj);
        }
    }

    void DropItem()
    {
        foreach (GameObject i in itemDrop)
        {
            if (i != null)
            {
                GameObject item = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/items/", i.name), new Vector3(transform.position.x, 1.2f, transform.position.z), i.transform.rotation);
                if (item.name == "coin(Clone) coin(Clone)")
                {
                    WaitFor(2f, () =>
                    {
                        if (item.GetComponent<PhotonView>().IsMine)
                        {
                            PhotonNetwork.Destroy(item.GetComponent<PhotonView>());
                        }
                    });
                }
            }
        }
    }
}

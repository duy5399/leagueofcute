using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static StoreClientSocketIO;

public class BoomsManager : MonoBehaviour
{
    public static BoomsManager instance { get; private set; }

    [SerializeField] private Image image_BoomEquiped;
    [SerializeField] private int int_prefab = 1;

    void Awake()
    {
        if (instance == null && instance != this)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        try
        {
            ItemInStoreJSON boomEquip = SocketIO.instance._storeClientSocketIO._booms.First(x => x.itemID == SocketIO.instance._inventoryClientSocketIO._userInventory.boomEquip);
            Sprite sprite = Resources.Load<Sprite>("textures/booms/" + boomEquip.displayImage);
            image_BoomEquiped.sprite = sprite;
        }
        catch
        {
            Debug.Log("Error when instantiate boom equip");
        }
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    if(int_prefab == 7)
        //        int_prefab = -1;
        //    int_prefab++;
        //    GameObject gameObject = Instantiate(go_Prefabs[int_prefab], transform.position, transform.rotation);
        //    //gameObject.transform.parent = transform;
        //}
    }

    public void ChangeBoom(string idItem, Sprite sprite)
    {
        GameObject prefab_Boom = Resources.Load<GameObject>("prefabs/fight/booms/" + idItem);
        GameObject gameObject = Instantiate(prefab_Boom, transform.position, transform.rotation);
        image_BoomEquiped.sprite = sprite;
    }
}

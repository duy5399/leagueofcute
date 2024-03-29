using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static StoreClientSocketIO;

public class TacticiansManager : MonoBehaviour
{
    public static TacticiansManager instance { get; private set; }

    private Dictionary<string, GameObject> dict_Tacticians = new Dictionary<string, GameObject>();
    [SerializeField] private Image image_TacticianEquiped;

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
            ItemInStoreJSON tacticianEquip = SocketIO.instance._storeClientSocketIO._tacticians.First(x => x.itemID == SocketIO.instance._inventoryClientSocketIO._userInventory.tacticianEquip);
            Sprite sprite = Resources.Load<Sprite>("textures/tacticians/" + tacticianEquip.displayImage);
            image_TacticianEquiped.sprite = sprite;
        }
        catch {
            Debug.Log("Error when instantiate tacticin equip");
        }
    }

    public void ChangeTactician(string name, Sprite sprite)
    {
        if (!dict_Tacticians.ContainsKey(name))
        {
            foreach (var i in dict_Tacticians)
            {
                i.Value.SetActive(false);
            }
            GameObject prefab_Tactician = Resources.Load<GameObject>("prefabs/fight/tacticians/" + name);
            GameObject gameObject = Instantiate(prefab_Tactician, transform);
            //gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            gameObject.transform.rotation = Quaternion.Euler(-52.12f, 174.182f, -2.494f);
            gameObject.transform.localScale = new Vector3(3, 3, 3);
            dict_Tacticians.Add(name, gameObject);
        }
        else
        {
            foreach(var i in dict_Tacticians)
            {
                if(i.Key == name)
                    i.Value.SetActive(true);
                else
                    i.Value.SetActive(false);
            }
        }
        image_TacticianEquiped.sprite = sprite;
    }
}

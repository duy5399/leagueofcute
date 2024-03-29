using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static StoreClientSocketIO;

public class StoreManager : MonoBehaviour
{
    public static StoreManager instance { get; private set; }

    [SerializeField] private Dictionary<string, GameObject> dict_DisplayedItem = new Dictionary<string, GameObject>();
    [SerializeField] private Transform scrollView_ItemList;
    [SerializeField] private Transform transform_Content;
    [SerializeField] private GameObject go_PrefabItem;
    [SerializeField] private TextMeshProUGUI text_Gold;
    [SerializeField] private TextMeshProUGUI text_Crystal;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        SetCurrencies(SocketIO.instance._inventoryClientSocketIO._userInventory.gold, SocketIO.instance._inventoryClientSocketIO._userInventory.crystal);
        gameObject.SetActive(false);
    }

    public void SetCurrencies(int gold, int crystal)
    {
        text_Gold.text = gold.ToString();
        text_Crystal.text = crystal.ToString();
    }

    public void GetItems(string itemClass)
    {
        ItemInStoreJSON[] items = new ItemInStoreJSON[0];
        switch (itemClass)
        {
            case "Tacticians":
                items = SocketIO.instance._storeClientSocketIO._tacticians;
                break;
            case "ArenaSkins":
                items = SocketIO.instance._storeClientSocketIO._arenaSkins;
                break;
            case "Booms":
                items = SocketIO.instance._storeClientSocketIO._booms;
                break;
        }
        foreach (var item in items)
        {
            if (!dict_DisplayedItem.ContainsKey(item.itemID))
            {
                GameObject gameObject = Instantiate(go_PrefabItem, transform_Content);
                gameObject.GetComponent<PrefabItemManager>().LoadItem(item);
                gameObject.GetComponent<PrefabItemManager>().LoadName(item.displayName);
                gameObject.GetComponent<PrefabItemManager>().LoadBackground(item.itemClass, item.displayImage);
                dict_DisplayedItem.Add(item.itemID, gameObject);
            }

            if (SocketIO.instance._inventoryClientSocketIO._userInventory.tacticians.Contains(item.itemID) || SocketIO.instance._inventoryClientSocketIO._userInventory.arenaSkins.Contains(item.itemID) || SocketIO.instance._inventoryClientSocketIO._userInventory.booms.Contains(item.itemID))
            {
                dict_DisplayedItem[item.itemID].GetComponent<PrefabItemManager>().LoadEquip(item.itemClass);
                dict_DisplayedItem[item.itemID].GetComponent<PrefabItemManager>().LoadTextCurrency("Equip");
                dict_DisplayedItem[item.itemID].GetComponent<PrefabItemManager>().LoadSpriteCurrency("baiquan mohu");
            }
            else
            {
                dict_DisplayedItem[item.itemID].GetComponent<PrefabItemManager>().LoadBuy();
                dict_DisplayedItem[item.itemID].GetComponent<PrefabItemManager>().LoadTextCurrency(item.price.amount.ToString());
                dict_DisplayedItem[item.itemID].GetComponent<PrefabItemManager>().LoadSpriteCurrency(item.price.currency == "gold" ? "currency_icon-gold" : "currency_icon-crystal");
            }
        }
    }

    public void SetContent(Transform transform)
    {
        transform_Content = transform;
        scrollView_ItemList.GetComponent<ScrollRect>().content = transform_Content.GetComponent<RectTransform>();
    }
}

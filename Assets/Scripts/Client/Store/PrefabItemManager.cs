using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static StoreClientSocketIO;

public class PrefabItemManager : MonoBehaviour
{
    [SerializeField] private ItemInStoreJSON item;
    [SerializeField] private Sprite sprite_imageItem;
    [SerializeField] private TextMeshProUGUI text_Name;
    [SerializeField] private Transform button_Equip;
    [SerializeField] private Image image_Background;
    [SerializeField] private Image image_Currency;
    [SerializeField] private List<Sprite> list_Sprite_Currency;

    private void OnDisable()
    {
        button_Equip.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void LoadItem(ItemInStoreJSON item)
    {
        this.item = item;
    }

    public void LoadName(string displayName)
    {
        text_Name.text = displayName;
    }

    public void LoadBackground(string itemClass, string displayImage)
    {
        switch (itemClass)
        {
            case "Tactician":
                image_Background.sprite = Resources.Load<Sprite>("textures/tacticians/" + displayImage);
                break;
            case "ArenaSkin":
                image_Background.sprite = Resources.Load<Sprite>("textures/arena-skins/" + displayImage);
                break;
            case "Boom":
                image_Background.sprite = Resources.Load<Sprite>("textures/booms/" + displayImage);
                break;
        }
        sprite_imageItem = image_Background.sprite;
    }

    public void LoadTextCurrency(string currency)
    {
        button_Equip.GetChild(0).GetComponent<TextMeshProUGUI>().text = currency;
    }

    public void LoadSpriteCurrency(string currency)
    {
        image_Currency.sprite = list_Sprite_Currency.Find(x => x.name == currency);
    }

    public void LoadEquip(string itemClass)
    {
        switch (itemClass)
        {
            case "Tactician":
                button_Equip.GetComponent<Button>().onClick.AddListener(OnClick_ChangeTactician);
                break;
            case "ArenaSkin":
                button_Equip.GetComponent<Button>().onClick.AddListener(OnClick_ChangeArenaSkin);
                break;
            case "Boom":
                button_Equip.GetComponent<Button>().onClick.AddListener(OnClick_ChangeBoom);
                break;
        }
        button_Equip.GetComponent<Button>().onClick.AddListener(EquipItemClient);
    }

    public void LoadBuy()
    {
        button_Equip.GetComponent<Button>().onClick.AddListener(OnClick_BuyItem);
    }

    public void OnClick_ChangeTactician()
    {
        TacticiansManager.instance.ChangeTactician(item.itemID, sprite_imageItem);
    }

    public void OnClick_ChangeArenaSkin()
    {
        ArenaSkinsManager.instance.RPC_ChangeArenaSkin(item.itemID);
        ArenaSkinsManager.instance.ChangeImageArenaSkinEquiped(sprite_imageItem);
    }

    public void OnClick_ChangeBoom()
    {
        BoomsManager.instance.ChangeBoom(item.itemID, sprite_imageItem);
    }

    public void EquipItemClient()
    {
        SocketIO.instance._inventoryClientSocketIO.Emit_EquipItemClient(item.itemID, item.itemClass);
    }

    public void OnClick_BuyItem()
    {
        SocketIO.instance._storeClientSocketIO.Emit_BuyItem(item.itemID, item.itemClass);
    }
}

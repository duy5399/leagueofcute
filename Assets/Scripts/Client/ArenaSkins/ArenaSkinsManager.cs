using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static StoreClientSocketIO;

public class ArenaSkinsManager : MonoBehaviourPun
{
    public static ArenaSkinsManager instance { get; private set; }

    [SerializeField] private Image image_arenaSkinEquiped;
    [SerializeField] private Renderer meshRenderer;

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
            ItemInStoreJSON arenaSkinEquip = SocketIO.instance._storeClientSocketIO._arenaSkins.First(x => x.itemID == SocketIO.instance._inventoryClientSocketIO._userInventory.arenaSkinEquip);
            Sprite sprite = Resources.Load<Sprite>("textures/arena-skins/" + arenaSkinEquip.displayImage);
            image_arenaSkinEquiped.sprite = sprite;
            RPC_ChangeArenaSkin(SocketIO.instance._inventoryClientSocketIO._userInventory.arenaSkinEquip);
        }
        catch
        {
            Debug.Log("Error when instantiate arena skin equip");
        }
    }

    public void ChangeImageArenaSkinEquiped(Sprite sprite)
    {
        image_arenaSkinEquiped.sprite = sprite;
    }

    public void ChangeArenaSkin(string arenaSkin)
    {
        photonView.RPC(nameof(RPC_ChangeArenaSkin), RpcTarget.AllBuffered, arenaSkin);
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
    public void RPC_ChangeArenaSkin(string arenaSkin)
    {
        var materialsCopy = meshRenderer.materials;
        Material newMaterial = Resources.Load<Material>("textures/arena-skins/" + arenaSkin);
        materialsCopy[8] = newMaterial;
        meshRenderer.materials = materialsCopy;
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
}

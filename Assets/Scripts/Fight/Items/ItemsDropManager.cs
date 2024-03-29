using AYellowpaper.SerializedCollections;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemsDropManager : MonoBehaviourPun
{
    public static ItemsDropManager instance { get; private set; }

    [SerializedDictionary("Slot", "Unit")]
    [SerializeField] private SerializedDictionary<GameObject, GameObject> _dictItemDrop;
    [SerializeField] private const int MAX_ITEMSDROPSLOT = 10;
    [SerializeField] private const int MAX_ROWS = 4;
    [SerializeField] private const int MAX_COLS = 3;
    [SerializeField] private const float POINT_X = -11f;
    [SerializeField] private const float POINT_Y = 8f;
    [SerializeField] private GameObject prefabItemSlot;
    public SerializedDictionary<GameObject, GameObject> dictItemDrop => _dictItemDrop;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        CreateItemsDrop();
    }

    private void CreateItemsDrop()
    {
        int num = 0;
        float posX;
        float posY = 7f;
        for (int x = 0; x < MAX_ROWS; x++)
        {
            posX = -12f;
            posY += 1f;
            for (int y = 0; y < MAX_COLS; y++)
            {
                posX += 1f;
                GameObject itemSlot = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/arenas", prefabItemSlot.name), prefabItemSlot.transform.position, Quaternion.identity);
                itemSlot.GetComponent<SlotManager>().SetNodeName("slot" + x + "_" + y);
                itemSlot.GetComponent<SlotManager>().SetNode(x, y);
                itemSlot.GetComponent<SlotManager>().SetName("slot" + x + "_" + y);
                itemSlot.GetComponent<SlotManager>().SetParent(gameObject.GetPhotonView().ViewID);
                itemSlot.GetComponent<SlotManager>().SetActive(false);
                itemSlot.transform.localPosition = new Vector3(POINT_X + y, itemSlot.transform.position.z, POINT_Y - x);
                _dictItemDrop.Add(itemSlot, null);
                num++;
                if (num == MAX_ITEMSDROPSLOT)
                {
                    return;
                }
            }
        }
    }
    public void MoveItemsDropManager(int viewIDParent)
    {
        transform.parent.GetComponent<PlayerPosManager>().ChangeParent(gameObject.GetPhotonView().ViewID, viewIDParent);
    }
    public void SetLocalPosition(Vector3 newPosition)
    {
        transform.localPosition = newPosition;
    }
    public void SetLocalRotation(Vector3 newRotation)
    {
        transform.localRotation = Quaternion.Euler(newRotation.x, newRotation.y, newRotation.z);
    }
    public void SetParent(int photonviewParent)
    {
        photonView.RPC(nameof(RPC_SetParent), RpcTarget.AllBuffered, photonviewParent);
    }

    [PunRPC]
    void RPC_SetParent(int viewID)
    {
        Transform _parent = PhotonView.Find(viewID).transform;
        transform.parent = _parent;
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BenchManager;

public class SlotManager : MonoBehaviourPun
{
    [SerializeField] private string nodeName;
    [SerializeField] private int[] node;

    public string _nodeName => nodeName;
    public int[] _node => node;

    public void SetNodeName(string nodeName)
    {
        this.nodeName = nodeName;
    }

    public void SetNode(int x, int y)
    {
        node = new int[] { x, y };
    }

    public void ActiveBorder(bool isActive)
    {
        Renderer renderer = transform.GetComponent<Renderer>();
        if (isActive)
            renderer.material.SetColor("_Color", Color.green);
        else
            renderer.material.SetColor("_Color", Color.gray);
    }

    public void SetName(string name)
    {
        photonView.RPC(nameof(RPC_SetName), RpcTarget.AllBuffered, name);
    }

    public void SetParent(int photonviewParent)
    {
        photonView.RPC(nameof(RPC_SetParent), RpcTarget.AllBuffered, photonviewParent);
    }

    public void SetActive(bool isActive)
    {
        photonView.RPC(nameof(RPC_SetActive), RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    void RPC_SetName(string name)
    {
        gameObject.name = name;
    }

    [PunRPC]
    void RPC_SetParent(int viewID)
    {
        Transform tf_Parent = PhotonView.Find(viewID).transform;
        transform.parent = tf_Parent;
    }

    [PunRPC]
    public void RPC_SetActive(bool boolean)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = boolean;
    }
}

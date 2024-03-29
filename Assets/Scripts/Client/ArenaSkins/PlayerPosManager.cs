using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosManager : MonoBehaviourPun
{
    public void ChangeName(string name)
    {
        photonView.RPC(nameof(RPC_ChangeName), RpcTarget.AllBuffered, name);
    }
    public void ChangeParent(int viewIDChildren, int viewIDParent)
    {
        photonView.RPC(nameof(RPC_ChangeParent), RpcTarget.AllBuffered, viewIDChildren, viewIDParent);
    }

    [PunRPC]
    void RPC_ChangeName(string name)
    {
        gameObject.name = name;
    }

    [PunRPC]
    public void RPC_ChangeParent(int viewIDChildren, int viewIDParent)
    {
        Transform _children = PhotonView.Find(viewIDChildren).transform;
        Transform _parent = PhotonView.Find(viewIDParent).transform;
        _children.parent = _parent;
    }
}

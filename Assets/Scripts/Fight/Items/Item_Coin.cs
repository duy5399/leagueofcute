using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Coin : MonoBehaviourPun
{
    [SerializeField] private float destroyTime;

    private void Awake()
    {
        destroyTime = 2f;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            destroyTime -= Time.deltaTime;
            if (destroyTime <= 0)
            {
                PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
            }
        }
    }
}

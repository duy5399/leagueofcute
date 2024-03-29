using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviourPun
{
    [SerializeField] private List<GameObject> _objPool;

    private void Awake()
    {
        _objPool = new List<GameObject>();
    }

    public List<GameObject> objPool
    {
        get {
            _objPool.RemoveAll(x => x == null);
            return _objPool; 
        }
        set { _objPool = value; }
    }

    public void DestroySpawn(GameObject go)
    {
        GameObject _go = _objPool.FirstOrDefault(x => x == go);
        if (_go != null)
        {
            _objPool.Remove(go);
        }
        if (go.GetComponent<PhotonView>().IsMine)
        {
            PhotonNetwork.Destroy(go.GetComponent<PhotonView>());
        }
    }
}

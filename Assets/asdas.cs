using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class asdas : MonoBehaviour
{
    [SerializeField] public Animator anim;
    [SerializeField] public GameObject a;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            anim.avatar = a.GetComponent<Animator>().avatar;
        }
    }
}

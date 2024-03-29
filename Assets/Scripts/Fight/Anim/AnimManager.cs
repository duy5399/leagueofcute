using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private float float_animSpeed = 1f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerAnim(string animName, float animSpeed)
    {
        if (animator == null)
        {
            return;
        }

        float_animSpeed = animSpeed;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System;
using Photon.Pun;

public class PetMovement : MonoBehaviourPun
{
    [SerializeField] private PetManager petManager;
    [SerializeField] private PetControls petControls_input;
    [SerializeField] private ParticleSystem particle_clickEffect;
    [SerializeField] private LayerMask layerMask_Terrain;

    private void Awake()
    {
        petManager = GetComponentInParent<PetManager>();
        petControls_input = new PetControls();
        AssignInputs();
    }

    private void OnEnable()
    {
        petControls_input.Enable();
    }

    private void OnDisable()
    {
        petControls_input.Disable();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            FaceTarget();
            SetAnimations();
        }
    }

    void AssignInputs()
    {
        petControls_input.Pet.Move.performed += ctx => ClickToMove();
    }

    void ClickToMove()
    {
        if (!photonView.IsMine)
            return;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, layerMask_Terrain))
        {
            //navMeshAgent.destination = hit.point;
            petManager.navMeshAgent.SetDestination(hit.point);
            if (particle_clickEffect != null)
            {
                //Instantiate(particle_clickEffect, hit.point += new Vector3(0, 0.1f, 0), particle_clickEffect.transform.rotation);
                particle_clickEffect.gameObject.SetActive(true);
                particle_clickEffect.transform.position = hit.point;
                var particle = particle_clickEffect.main;
                particle.startLifetime = 0.5f;
                particle_clickEffect.Play();
            }
        }
    }

    private void SetAnimations()
    {
        if (petManager.navMeshAgent.velocity != Vector3.zero)
            petManager._animator.SetBool("Run", true);
        else
            petManager._animator.SetBool("Run", false);
    }

    private void FaceTarget()
    {
        if (petManager.navMeshAgent.velocity != Vector3.zero)
        {
            Vector3 direction = (petManager.navMeshAgent.destination - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            Debug.Log("FaceTarget: " + transform.name);
        }   
    }
}

using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProjectileMover : MonoBehaviourPun
{
    [SerializeField] private GameObject _target;
    [SerializeField] private float speedFly;
    [SerializeField] private bool _isActive;

    public GameObject target
    {
        get { return _target; }
        set { _target = value; }
    }
    public bool isActive
    {
        get { return _isActive; }
        set { _isActive = value; }
    }

    public float speed = 5f;
    public float hitOffset = 0f;
    public bool UseFirePointRotation;
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    public GameObject hit;
    public GameObject flash;
    private Rigidbody rb;
    public GameObject[] Detached;

    private void Awake()
    {
        speedFly = 40f;
        isActive = false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (flash != null)
        {
            GameObject flashInstance = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/booms", flash.name), transform.position, Quaternion.identity);
            //var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                //Destroy(flashInstance, flashPs.main.duration);
                _WaitFor(flashPs.main.duration, () =>
                {
                    PhotonNetwork.Destroy(flashInstance.GetComponent<PhotonView>());
                });
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                //Destroy(flashInstance, flashPsParts.main.duration);
                _WaitFor(flashPsParts.main.duration, () =>
                {
                    PhotonNetwork.Destroy(flashInstance.GetComponent<PhotonView>());
                });
            }
        }
        _WaitFor(3f, () =>
        {
            PhotonNetwork.Destroy(photonView);
        });
        //Destroy(gameObject,5);
    }

    void FixedUpdate ()
    {
        if (isActive && target != null)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, speedFly * Time.fixedDeltaTime);
            this.transform.LookAt(target.transform.position);
        }
        //if (speed != 0)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, new Vector3(-8.05f, 21f, 28.5f), speed);
        //    //rb.velocity = transform.forward * speed;
        //    //transform.position += transform.forward * (speed * Time.deltaTime);         
        //}
	}

    public IEnumerator _WaitFor(float delay, Action func)
    {
        yield return new WaitForSeconds(delay);
        func();
    }

    //https ://docs.unity3d.com/ScriptReference/Rigidbody.OnCollisionEnter.html
    //void OnCollisionEnter(Collision collision)
    //{
    //    //Lock all axes movement and rotation
    //    rb.constraints = RigidbodyConstraints.FreezeAll;
    //    speed = 0;

    //    ContactPoint contact = collision.contacts[0];
    //    Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
    //    Vector3 pos = contact.point + contact.normal * hitOffset;

    //    if (hit != null)
    //    {
    //        var hitInstance = Instantiate(hit, pos, rot);
    //        if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
    //        else if (rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(rotationOffset); }
    //        else { hitInstance.transform.LookAt(contact.point + contact.normal); }

    //        var hitPs = hitInstance.GetComponent<ParticleSystem>();
    //        if (hitPs != null)
    //        {
    //            Destroy(hitInstance, hitPs.main.duration);
    //        }
    //        else
    //        {
    //            var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
    //            Destroy(hitInstance, hitPsParts.main.duration);
    //        }
    //    }
    //    foreach (var detachedPrefab in Detached)
    //    {
    //        if (detachedPrefab != null)
    //        {
    //            detachedPrefab.transform.parent = null;
    //        }
    //    }
    //    isActive = false;
    //    target = null;
    //    gameObject.SetActive(false);
    //}
    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("ProjectileMover: " + collider.name);
        if (collider != null && collider.gameObject == target)
        {
            //Debug.Log("ProjectileMover OnTriggerEnter: " + target.name);
            //Lock all axes movement and rotation
            rb.constraints = RigidbodyConstraints.FreezeAll;
            speed = 0;

            //ContactPoint contact = collider.GetComponent<Collision>().contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
            //Vector3 pos = contact.point + contact.normal * hitOffset;
            Vector3 pos = collider.transform.position;

            if (hit != null)
            {
                //var hitInstance = Instantiate(hit, pos, rot);
                GameObject hitInstance = PhotonNetwork.Instantiate(Path.Combine("prefabs/fight/booms", hit.name), transform.position, Quaternion.identity);
                if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
                else if (rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(rotationOffset); }
                else { hitInstance.transform.LookAt(collider.transform.position); }

                var hitPs = hitInstance.GetComponent<ParticleSystem>();
                if (hitPs != null)
                {
                    //Destroy(hitInstance, hitPs.main.duration);
                    _WaitFor(hitPs.main.duration, () =>
                    {
                        PhotonNetwork.Destroy(hitInstance.GetComponent<PhotonView>());
                    });
                }
                else
                {
                    var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                    //Destroy(hitInstance, hitPsParts.main.duration);
                    _WaitFor(hitPsParts.main.duration, () =>
                    {
                        PhotonNetwork.Destroy(hitInstance.GetComponent<PhotonView>());
                    });
                }
            }
            foreach (var detachedPrefab in Detached)
            {
                if (detachedPrefab != null)
                {
                    detachedPrefab.transform.parent = null;
                }
            }
            isActive = false;
            target = null;
            gameObject.SetActive(false);
        }
    }
}

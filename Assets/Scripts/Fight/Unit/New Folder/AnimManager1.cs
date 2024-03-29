using Animancer;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using static SkillBase1;

public class AnimManager1 : ChampionBase
{
    //[Serializable]
    //public class AnimEffects
    //{
    //    public string effectName;
    //    public GameObject effectsPrefeb;
    //    public Vector3 effectPosition;
    //    public float lifeTime;
    //    public bool followHero = true;
    //}

    public enum Status
    {
        idle = 0,
        run = 1,
        death = 2,
        orther = 3
    }

    [SerializeField] private Animator animator;
    [SerializeField] private AnimancerComponent _animancer;
    //[SerializeField] private float animSpeed = 1f;
    [SerializeField] private Status status;
    //[SerializeField] private float animTime;

    public AnimancerComponent animancer
    {
        get { return _animancer; }
        set { _animancer = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
        animancer = GetComponentInChildren<AnimancerComponent>();
    }

    private void Start()
    {
        CreateStates(animator, animancer);
    }

    private void OnEnable()
    {
        animancer.TryPlay("idle");
    }

    private void CreateStates(Animator animator, AnimancerComponent animancer)
    {
        if (animator == null || animancer == null)
        {
            return;
        }
        foreach (var i in animator.runtimeAnimatorController.animationClips)
        {
            if(base.info.chStat.championName == "Jinx")
            {
                if(i.name == "idle02_0")
                {
                    i.name = "idle";
                }
            }
            animancer.States.GetOrCreate(i.name, i);
            Debug.Log(i.name);
        }
        animancer.TryPlay("idle");
    }

    //private void GetAnimAndEffects()
    //{
    //    if (File.Exists(string.Concat(Application.dataPath, animPath)))
    //    {
    //        Debug.Log("Save path found" + string.Concat(Application.dataPath, animPath));
    //        string loadAnimEffecsAllHero = File.ReadAllText(string.Concat(Application.dataPath, animPath));
    //        animLst = JsonConvert.DeserializeObject<AnimEffecsAllHero[]>(loadAnimEffecsAllHero).FirstOrDefault(x => x.charactor == unitManager._unitStat.championName).heroAnim;
    //    }
    //    else
    //    {
    //        Debug.Log("Save path not found" + string.Concat(Application.dataPath, animPath));
    //    }
    //}

    [PunRPC]
    void PlayAnimation(string animName, float animSpeed)
    {
        var state = animancer.TryPlay(animName);
        state.Speed = animSpeed;
        state.Time = 0;
        state.NormalizedTime = 0;
        if (animName == "r_s")
        {
            //Debug.Log("state Janna r_s");
            state.Events.OnEnd = () =>
            {
                //PlayAnimation("r_loop", animSpeed);
                photonView.RPC(nameof(PlayAnimation), RpcTarget.All, "r_loop", animSpeed);
                //Debug.Log("state Janna r_loop");
            };
        }
        else if (animName == "death")
        {
            state.Events.Add(0.99f, OnDeath);
        }
    }

    public virtual void TriggerAnim(string animName, float animSpeed = 1f, bool froce = false)
    {
        if (animator == null || animancer == null || (animName == "idle" && status == Status.idle) || (animName == "run" && status == Status.run))
        {
            if(animator == null)
            {
                Debug.Log("TriggerAnim: error animator" + base.info.name);
            }
            if (animancer == null)
            {
                Debug.Log("TriggerAnim: error animancer" + base.info.name);
            }
            if (animName == "idle" && status == Status.idle)
            {
                Debug.Log("TriggerAnim: error animName == \"idle\"" + base.info.name);
            }
            if (animName == "run" && status == Status.run)
            {
                Debug.Log("TriggerAnim: error animName == \"run\"" + base.info.name);
            }
            return;
        }
        if (base.info.chStat.championName == "Jax" && animName == "e")
        {
            return;
        }
        Debug.Log("TriggerAnim: " + base.info.name + " - " + animName);
        if (animName == "idle" || animName == "forceIdle")
        {
            status = Status.idle;
        }
        else if (animName == "run")
        {
            status = Status.run;
        }
        else if (animName == "death")
        {
            status = Status.death;
        }
        else
        {
            status = Status.orther;
        }
        AnimancerState state;
        if (froce)
        {
            if (base.info.chStat.championName == "Janna" && animName == "r")
            {
                //PlayAnimation("r_s", animSpeed);
                photonView.RPC(nameof(PlayAnimation), RpcTarget.All, "r_s", animSpeed);
            }
            else
            {
                //PlayAnimation(animName, animSpeed);
                photonView.RPC(nameof(PlayAnimation), RpcTarget.All, animName, animSpeed);
            }
        }
        else
        {
            state = animancer.States.Current;
            state.Events.OnEnd = () =>
            {
                photonView.RPC(nameof(PlayAnimation), RpcTarget.All, animName, animSpeed);
                //PlayAnimation(animName, animSpeed);
            };
        }
    }

    public void TriggerIdle(bool isInterrupt = false, bool force = false)
    {
        TriggerAnim("idle", 1f, force);
    }

    public void TriggerForceIdle(bool isInterrupt = true, bool force = true)
    {
        TriggerAnim("idle", 1f, force);
    }

    public void TriggerRun(bool force = false)
    {
        TriggerAnim("run", 1f, force);
    }

    public void TriggerDeath(bool force = false)
    {
        TriggerAnim("death", 1f, force);
    }

    public void TriggerVictory(bool force = false)
    {
        TriggerAnim("victory", 1f, force);
    }

    public void OnDeath()
    {
        Debug.Log("state.Events.OnEnd death");
        if(base.info.chCategory == ChampionInfo1.Categories.Monster)
        {
            if (photonView.IsMine)
            {
                this.GetComponent<PhotonView>().Synchronization = ViewSynchronization.Off;
                PhotonNetwork.Destroy(this.gameObject.GetComponent<PhotonView>());
            }
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class SkillEffect : ChampionBase
{
    //private Seq<MeshRenderer> _meshRenderLst;
    //private Seq<TrailRenderer> _trailRenderLst;
    //private Seq<Transform> _sonTransforms;

    [SerializeField] private List<ParticleSystem> _particleSystemLst;

    private ChampionBase _master;
    public bool isFollowParent;

    protected List<ParticleSystem> particleSystemLst
    {
        get
        {
            if (_particleSystemLst == null)
            {
                _particleSystemLst = GetComponentsInChildren<ParticleSystem>().ToList();
            }
            return _particleSystemLst;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _particleSystemLst = GetComponentsInChildren<ParticleSystem>().ToList();
    }

    public virtual void Play()
    {
        particleSystemLst.ForEach(p =>
        {
            p.Play();
        });
    }

    public virtual void Stop()
    {
        particleSystemLst.ForEach(p =>
        {
            p.Stop();
        });
    }

    public ChampionBase master
    {
        get
        {
            if (_master == null)
            {
                _master = GetComponent<SkillSpawn1>().currentCasterStatus.info;
            }
            return _master;
        }
        set
        {
            _master = value;
        }
    }

    public void OnEnable()
    {
        Play();
    }

    public void OnDisable()
    {
        Stop();
    }

    private void FixedUpdate()
    {
        if (isFollowParent)
        {
            base.transform.position = base.transform.parent.position;
            base.transform.rotation = base.transform.parent.rotation;
        }
    }
}

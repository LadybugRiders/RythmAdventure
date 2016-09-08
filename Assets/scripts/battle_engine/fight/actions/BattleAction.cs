using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleAction : MonoBehaviour {

    [SerializeField] protected List<BattleActionEffect> m_effects;

    [SerializeField] protected AudioClip m_mainSoundClip;
    protected AudioSource m_audioSource;

    protected bool m_launched = false;
    protected bool m_dead = false;

    protected BattleActor m_caster;
    protected BattleActor m_target;

    protected int m_duelID = -1;
    /// <summary>
    /// Accuracy of the note hit for this magic action
    /// </summary>
    protected HitAccuracy m_noteAccuracy = HitAccuracy.MISS;
        
    protected int m_power = 20;
    protected int m_costByUse = 35;

    protected float m_duration = 1.0f;

    // Use this for initialization
    virtual protected void Start () {
        m_audioSource = GetComponent<AudioSource>();
        m_effects = new List<BattleActionEffect>(this.GetComponentsInChildren<BattleActionEffect>(true));
        Die();
    }
	
	// Update is called once per frame
	virtual protected void Update () {
	
	}

    public void Launch(BattleActor _caster, BattleActor _target, int _duelID, HitAccuracy _noteAcuracy)
    {
        gameObject.SetActive(true);
        m_launched = true;
        m_target = _target;
        m_caster = _caster;
        m_duelID = _duelID;
        m_noteAccuracy = _noteAcuracy;
        Launch();
    }

    virtual protected void Launch()
    {
    }

    /// <summary>
    /// Called by an effect when it has hit its target(s)
    /// </summary>
    virtual public void OnHit()
    {
    }

    public BattleActionEffect GetFreeEffect()
    {
        for (int i = 0; i < m_effects.Count; i++)
        {
            if (!m_effects[i].IsLaunched)
            {
                return m_effects[i];
            }
        }
        return null;
    }

    virtual public void Die()
    {
        for (int i = 0; i < m_effects.Count; i++)
        {
            m_effects[i].Die();
        }
        m_launched = false;
    }

    #region PROPERTIES
    public bool IsLaunched
    {
        get
        {
            return m_launched;
        }
    }

    public BattleActor Caster
    {
        get
        {
            return m_caster;
        }
        set
        {
            m_caster = value;
        }
    }

    public BattleActor Target
    {
        get
        {
            return m_target;
        }
        set
        {
            m_target = value;
        }
    }

    public bool IsDead
    {
        get
        {
            return m_dead;
        }
    }

    public int DuelID
    {
        get
        {
            return m_duelID;
        }
    }

    public int Power
    {
        get
        {
            return m_power;
        }
    }

    public float Duration
    {
        get
        {
            return m_duration;
        }
        set
        {
            m_duration = value;
        }
    }

    public int CostByUse
    {
        get
        {
            return m_costByUse;
        }
    }
    #endregion
}

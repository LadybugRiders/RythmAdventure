using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFightMagic : MonoBehaviour {
	
	[SerializeField] protected BattleFightMagicManager m_magicManager;
	
	[SerializeField] protected SpriteRenderer m_casterEffect;
	[SerializeField] protected List<BattleFightMagicEffect> m_effects;

	[SerializeField] protected AudioClip m_mainSoundClip;
	protected AudioSource m_audioSource;

	protected bool m_launched = false;
	protected bool m_dead = false;

	protected BattleActor m_caster;
	protected BattleActor m_target;

	protected int m_duelID = -1;
	protected float m_duration = 1.0f;

	/** RAw power of the magic */
	protected int m_power = 20;
	/** How much mana are gained with a magic note */
	protected int m_fillRate = 10;
	/** Mana used when launching an effect ie:bullet */
	protected int m_costByUse = 35;
	/** Mana used by second when the magic is active */
	protected int m_decreaseRate = 1;

	// Use this for initialization
	virtual protected void Start () {
		m_audioSource = GetComponent<AudioSource> ();
		m_effects = new List<BattleFightMagicEffect> ( this.GetComponentsInChildren<BattleFightMagicEffect>(true ) );
		for (int i = 0; i < m_effects.Count; i++) {
			m_effects[i].Magic = this;
		}
		Dismiss ();
	}
	
	// Update is called once per frame
	virtual protected void Update () {
	
	}

	#region LAUNCH
	public void Launch(BattleActor _caster, BattleActor _target, int _duelID){
		gameObject.SetActive(true);
		m_launched = true;
		m_target = _target;
		m_caster = _caster;
		m_duelID = _duelID;
		//replace magic on the user
		Transform t = transform;
		float z = t.position.z;
		t.position = m_caster.transform.position;
		Utils.SetPositionZ (t, z);
		//
		Launch ();
	}

	/** Override this to launch the magic */
	virtual protected void Launch(){
		//Effect around the caster
		Utils.SetAlpha (m_casterEffect, 1f);
		m_casterEffect.gameObject.GetComponent<Animation> ().Play ();
		//Launch effect at first trigger ???
		Attack ();
	}
	
	virtual public void Dismiss(){
		if (m_launched) {
			m_magicManager.OnMagicEnded (this);
		}
		//kill caster effect
		Utils.SetAlpha (m_casterEffect, 0.0f);
		//kill effects
		Utils.SetAlpha (m_casterEffect, 0f);
		for (int i=0; i < m_effects.Count; i++) {			
			m_effects[i].Die();       
		}
		m_launched = false;
	}

	#endregion

	virtual public void Attack(){	
		BattleFightMagicEffect effect = GetFreeEffect ();
		if (effect == null) {
			Debug.Log( "no effect");
			return;
		}
		effect.Launch (m_caster.transform.position, m_target.transform.position);
	}

	/** Called by an effect when it has hit its target(s) */
	virtual public void OnHit(){
		int damage = this.m_power;
		damage = m_target.TakeMagicDamage (damage,this);
		m_target.FightDuel.OnActorTakeDamage (m_target, damage);
		//play main sound if any
		if (m_mainSoundClip) {
			m_audioSource.clip = m_mainSoundClip;
			m_audioSource.Play();
		}
	}

	#region EFFECTS

	/** Returns an effect that is not launched */
	public BattleFightMagicEffect GetFreeEffect(){
		for (int i=0; i < m_effects.Count; i++) {
			if(! m_effects[i].IsLaunched){
				return m_effects[i];
			}                        
		}
		return null;
	}

	#endregion

	#region GETTERS-SETTERS

	public bool IsLaunched{
		get{
			return m_launched;
		}
	}

	public BattleActor Caster {
		get {
			return m_caster;
		}
		set {
			m_caster = value;
		}
	}

	public BattleActor Target {
		get {
			return m_target;
		}
		set {
			m_target = value;
		}
	}

	public bool IsDead {
		get {
			return m_dead;
		}
	}

	public int DuelID {
		get {
			return m_duelID;
		}
	}

	public float Duration {
		get {
			return m_duration;
		}
		set {
			m_duration = value;
		}
	}

	public int Power {
		get {
			return m_power;
		}
	}

	public SpriteRenderer CasterEffect{
		get{
			return m_casterEffect;
		}
	}

	public int FillRate {
		get {
			return m_fillRate;
		}
	}

	public int CostByUse {
		get {
			return m_costByUse;
		}
	}
	#endregion
}

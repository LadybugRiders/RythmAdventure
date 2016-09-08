using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleActor : MonoBehaviour {

    //Main Sprite
    [SerializeField] protected SpriteRenderer m_sprite;
    //UI
    [SerializeField] protected UIBattleLifeBar m_lifeGauge;
    [SerializeField] protected UIBattleLifeBar m_manaGauge;

    [SerializeField] protected Transform m_attacksGroup; 

    public enum State{ IDLE, ATTACKING, DEFENDING, HIT, DEAD };
	protected State m_state = State.IDLE;

	public enum ActorType { CHARACTER, ENEMY };
	protected ActorType m_type = ActorType.CHARACTER;

	//Battle Fight references
	protected BattleFightManager m_fightManager;
	protected BattleFightManager.FightDuel m_fightDuel;
    
    //STATS
    protected Stats m_currentStats = new Stats();
    protected Stats m_maxStats = new Stats();

    protected BattleMagic m_currentMagic = null;

    protected BattleAction m_attack;
    protected List<BattleMagic> m_magics = new List<BattleMagic>();
    
	protected bool m_dead = false;

	protected Transform m_transform;

    void Awake()
    {
        Init();
    }

	// Use this for initialization
	virtual protected void Start ()
    {
        //Transform
		m_transform = transform;
		RefreshLifeGauge();
		RefreshManaGauge();
    }

    void Init()
    {
        m_magics = new List<BattleMagic>(2) { null, null };
        //find fight manager and ui stuff
        m_fightManager = BattleFightManager.instance;

		//ui gauges ( when instanciating the actors, it hasnt a parent yet )
		if ( (m_lifeGauge == null || m_manaGauge == null) && transform.parent != null )
		{
			var bars = transform.parent.GetComponentsInChildren<UIBattleLifeBar>();
			foreach (var bar in bars)
			{
				if (bar.IsMana)
					m_manaGauge = bar;
				else
					m_lifeGauge = bar;
			}
		}
    }

    virtual public void Load(string _name){
        Init();
        m_transform = transform;
    }
	
	// Update is called once per frame
	virtual protected void Update () {
		switch (m_state) {
		    case State.ATTACKING : UpdateAttacking(); break; 
		}
	}
    
	#region UPDATES
	
	virtual protected void UpdateAttacking(){
	}
	
	#endregion
	
	#region ACTIONS
	
	virtual public int GetAppliedAttackingPower( NoteData _noteData ){
		m_state = State.ATTACKING;
		return CurrentStats.Attack;
	}

	virtual public void OnAttackEnded(){
		this.m_state = State.IDLE; 
	}

	virtual public int TakeDamage(int _damage, NoteData _note){
		int damage = _damage;
		damage -= CurrentStats.Defense ;
		if (damage < 0)
			damage = 0;
		CurrentStats.HP -=  damage;
		RefreshLifeGauge ();

		//Notify manager if dead
//		bool dead = false;
		if (CurrentStats.HP <= 0) {
			Die ();
		}

		return damage;
	}

	virtual public int TakeMagicDamage( int _damage, BattleMagic _magic){

		_damage -= CurrentStats.Magic ;
		if (_damage < 0)
			_damage = 0;
		CurrentStats.HP -=  _damage;
		RefreshLifeGauge ();
		
		//Notify manager if dead
		CheckDeath ();
		
		return _damage;
	}

	/** Adds MP and return true if full */
	protected bool AddMP(int _mp){
		//TODO
		if (isCasting)
			return false;
		bool full = false;
		CurrentStats.MP += _mp;
		if( CurrentStats.MP >= MaxStats.MP){
			CurrentStats.MP = MaxStats.MP;
			full = true;
		}
		RefreshManaGauge ();
		return full;
	}

	virtual protected void CheckDeath(){
		if ( m_dead == false && CurrentStats.HP <= 0) {
			Die ();
		}
	}

	virtual protected bool Die(){
		m_dead = true;
		m_fightDuel.OnActorDead(this);
		return true;
	}
	
	#endregion

	#region MAGIC

    /// <summary>
    /// Launches the magic on the target
    /// </summary>
	virtual public BattleMagic LaunchMagic(BattleActor _target, int _duelID, HitAccuracy _accuracy){
        m_currentMagic = GetMagic(true);
        if (m_currentMagic == null)
            return null;

        m_currentMagic.Launch (this, _target,_duelID,_accuracy);
        
        //drain mp
        CurrentStats.MP -= m_currentMagic.CostByUse;
        RefreshManaGauge ();
        return m_currentMagic;
	}
    
    /// <summary>
    /// Called when a magic attack is cast. USe MP and return magic power
    /// </summary>
	virtual public int GetAppliedMagicAttackPower(HitAccuracy _accuracy){
		return CurrentStats.Magic;
	}

    /// <summary>
    /// Called when the magic is dismissed
    /// </summary>
	virtual public void OnDismissMagic(){
		RefreshManaGauge ();
		m_currentMagic.Die ();
	}

    public BattleMagic GetMagic(bool _attacking)
    {
        if( _attacking)
        {
            return m_magics[0];
        }
        return m_magics[1];
    }
	#endregion

	#region UI
	protected void RefreshLifeGauge(){
		if (m_lifeGauge == null)
			return;

		float hpPercent = MaxStats.HP == 0 ? 0 : (float)CurrentStats.HP / (float)MaxStats.HP;
		m_lifeGauge.SetValue( hpPercent );
	}

	protected void RefreshManaGauge(){
		if (m_manaGauge == null)
			return;

		float mpPercent = MaxStats.MP == 0 ? 0 : (float)CurrentStats.MP / (float)MaxStats.MP;
		m_manaGauge.SetValue( mpPercent );
	}

    public void RefreshUI()
    {
        RefreshLifeGauge();
        RefreshManaGauge();
    }

    #endregion ui

    #region PROPERTIES
    public ActorType Type {
		get {
			return m_type;
		}
	}

	public BattleFightManager.FightDuel FightDuel {
		get {
			return m_fightDuel;
		}
		set {
			m_fightDuel = value;
		}
	}

	public bool isCasting{
		get{
			return m_currentMagic != null && m_currentMagic.IsLaunched;
		}
	}

	public bool isDead{
		get{
			return m_dead;
		}
	}

	public SpriteRenderer MainSprite{
		 get{
			return m_sprite;
		}
	}

	public BattleMagic CurrentMagic {
		get {
			return m_currentMagic;
		}
	}

    protected Stats MaxStats
    {
        get
        {
            return m_maxStats;
        }
    }

    public Stats CurrentStats
    {
        get
        {
            return m_currentStats;
        }        
    }
    #endregion
}

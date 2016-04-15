using UnityEngine;
using System.Collections;

public class BattleActor : MonoBehaviour {
		
	public enum State{ IDLE, ATTACKING, DEFENDING, HIT, DEAD };
	protected State m_state = State.IDLE;

	public enum ActorType { CHARACTER, ENEMY };
	protected ActorType m_type = ActorType.CHARACTER;

	//Battle Fight references
	[SerializeField] protected BattleFightManager m_fightManager;
	protected BattleFightManager.FightDuel m_fightDuel;

	//Main Sprite
	[SerializeField] protected SpriteRenderer m_sprite;

	//UI
	[SerializeField] protected UIBattleLifeBar m_lifeGauge;
	[SerializeField] protected UIBattleLifeBar m_manaGauge;

	//DATA
	protected DataActor m_data;
	//STATS
	protected Stats m_currentStats;
	protected Stats m_maxStats;

	[SerializeField] protected BattleFightMagic m_currentMagic = null;

	protected bool m_dead = false;

	protected Transform m_transform;

	// Use this for initialization
	virtual protected void Start () {
		m_data = new DataActor ();
		m_data.Init ();
		m_currentStats = m_data.CurrentStats;
		m_maxStats = m_data.MaxStats;


		RefreshLifeGauge ();
		RefreshManaGauge ();
		//Transform
		m_transform = transform;
	}

	virtual public void Load(string _name){

	}
	
	// Update is called once per frame
	virtual protected void Update () {
		switch (m_state) {
		case State.ATTACKING : UpdateAttacking(); break; 
		}
	}

	virtual public BattleFightManager.ActorAttackAction AddNote(NoteData _noteData, bool _attacking){
		//Check noteData
		if (_attacking && _noteData.Subtype == NoteData.NoteSubtype.MAGIC) {
			if( AddMP(50) ){
				return BattleFightManager.ActorAttackAction.MAGIC;
			}
		}
		if (_attacking) {			
			return BattleFightManager.ActorAttackAction.ATTACK;
		}
		return BattleFightManager.ActorAttackAction.NONE;
	}

	#region UPDATES
	
	virtual protected void UpdateAttacking(){
	}
	
	#endregion
	
	#region ACTIONS
	
	virtual public int Attack( NoteData _noteData ){
		m_state = State.ATTACKING;
		return m_currentStats.Attack;
	}

	virtual public void OnAttackEnded(){
		this.m_state = State.IDLE; 
	}

	virtual public int TakeDamage(int _damage, NoteData _note){
		int damage = _damage;
		damage -= m_currentStats.Defense ;
		if (damage < 0)
			damage = 0;
		m_currentStats.HP -=  damage;

		//UI
		RefreshLifeGauge ();

		//Notify manager if dead
//		bool dead = false;
		if (m_currentStats.HP <= 0) {
			Die ();
		}

		return damage;
	}

	virtual public int TakeMagicDamage( int _damage, BattleFightMagic _magic){

		_damage -= m_currentStats.Magic ;
		if (_damage < 0)
			_damage = 0;
		m_currentStats.HP -=  _damage;
		
		//UI
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
		m_currentStats.MP += _mp;
		if( m_currentStats.MP >= m_maxStats.MP){
			m_currentStats.MP = m_maxStats.MP;
			full = true;
		}
		RefreshManaGauge();
		return full;
	}

	virtual protected void CheckDeath(){
		if ( m_dead == false && m_currentStats.HP <= 0) {
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
	virtual public BattleFightMagic LaunchMagic(BattleActor _target, int _duelID){
		if (m_currentMagic != null)
			m_currentMagic.Launch (this, _target,_duelID);
		RefreshManaGauge ();
		return m_currentMagic;
	}

	/** Called when a magic attack is cast */
	virtual public void MagicAttack(NoteData _noteData){
		m_currentMagic.Attack ();
		m_currentStats.MP -= m_currentMagic.CostByUse;
		if (m_currentStats.MP <= 0) {
			OnDismissMagic();
		}
		RefreshManaGauge ();
	}

	virtual public void OnDismissMagic(){
		m_currentStats.MP = 0;
		RefreshManaGauge ();
		m_currentMagic.Dismiss ();
	}
	#endregion

	#region UI
	protected void RefreshLifeGauge(){
		if (m_lifeGauge == null)
			return;
		
		float hpPercent = (float)m_currentStats.HP / (float)m_maxStats.HP;
		m_lifeGauge.SetValue( hpPercent );
	}

	protected void RefreshManaGauge(){
		if (m_manaGauge == null)
			return;
		
		float mpPercent = (float)m_currentStats.MP / (float)m_maxStats.MP;
		m_manaGauge.SetValue( mpPercent );
	}

	#endregion ui

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

	public BattleFightMagic CurrentMagic {
		get {
			return m_currentMagic;
		}
	}

}

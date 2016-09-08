using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFightManager : MonoBehaviour {

    static BattleFightManager _instance;

	[SerializeField] BattleEngine m_engine;
	[SerializeField] BattleDamageTextManager m_damageTextManager;

	[SerializeField] List<BattleCharacter> m_party;
	[SerializeField] List<BattleEnemy> m_enemies;
	List<FightDuel> m_duels;

    [SerializeField] SpriteRenderer m_backgroundSprite;

	//Event for death
	public event System.EventHandler<ActorDeadEventInfo> actorDeadEventHandler;
	public class ActorDeadEventInfo : System.EventArgs{
		public BattleActor DeadActor{ get; set; }

		public ActorDeadEventInfo(BattleActor _actor){
			DeadActor = _actor;
		}
	}

	//Event for battle end
	public event System.EventHandler<EndBattleEventInfo> endBattleEventHandler;
	public class EndBattleEventInfo : System.EventArgs{
		public bool Win {get;set;}
		public EndBattleEventInfo(bool _win){
			Win = _win;
		}
	}
    
	public static float DEPTH_BACKGROUND = 30.0f;
	public static float DEPTH_PLAYERS = 10.0f;
	public static float DEPTH_UI = 0.0f;

	void Awake(){
        _instance = this;
		m_duels = new List<FightDuel>();
		for(int i=0; i<3 ; i ++){
			m_duels.Add( new FightDuel(i,m_damageTextManager,this) );
		}
	}

	// Use this for initialization
	void Start () {
		BattleTracksManager.instance.noteEventHandler += OnReceiveNoteEvent;
		BattleTracksManager.instance.actionEventHandler += OnReceiveActionEvent;
	}

	/// <summary>
	/// Called by battle engine at start of the scene. Loads all the actors
	/// </summary>
	public void Load(BattleDataAsset battleData){

        //load team
        var teamCharsIds = ProfileManager.instance.GetProfile().CurrentTeam;
        for(int i=0; i < teamCharsIds.Count; i++)
        {
            m_party[i].Load(teamCharsIds[i]);
        }

        //Loads enemies
        if (battleData != null)
        {
            for(int i=0; i < battleData.Enemies.Count; i++)
            {
                if(m_enemies[i])
                {
                    //Destroy the temporary enemy
                    Transform parent = m_enemies[i].transform.parent;
                    Vector3 position = m_enemies[i].transform.position;
                    Destroy(m_enemies[i].gameObject);
                    //instantiate and place the new one
                    var prefab = battleData.Enemies[i].Prefab;
                    if( prefab != null)
                    {
                        //instantiate enemy
                        GameObject go = Instantiate(prefab) as GameObject;
                        m_enemies[i] = go.GetComponent<BattleEnemy>();
                        go.transform.SetParent(parent, true);
                        go.transform.position = position;
                        //load
                        m_enemies[i].Load(battleData.Enemies[i].Name, battleData.Enemies[i].Level);
                    }else
                    {
                        Debug.LogError("Enemy Prefab was null");
                    }
                }
            }
        }

        //Start duels
        for (int i = 0; i < m_duels.Count; i++)
        {
            m_duels[i].Start(m_party[i], m_enemies[i]);
        }

        //Background
        if (battleData.Background != null)
            m_backgroundSprite.sprite = battleData.Background;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region EVENTS

	public void OnReceiveNoteEvent(object sender, BattleTracksManager.NoteEventInfo eventInfo){
		
	}

	public void OnReceiveActionEvent(object sender, BattleTracksManager.NoteEventInfo eventInfo){

        /*if( eventInfo.NoteHit.TimeBegin > 8.0f)
            Debug.Log("RECEIVE ACTION " + eventInfo.NoteHit.TimeBegin);*/
		//nothing for missed notes
		if (eventInfo.Accuracy == HitAccuracy.MISS)
			return;
		//no attack for long note's head
		if (eventInfo.NoteHit.Type == NoteData.NoteType.LONG && eventInfo.NoteHit.Head)
			return;

		//Get the duel
		FightDuel duel = m_duels [eventInfo.NoteHit.TrackID];
		//If final note, no action is done
		if (eventInfo.IsFinal) {
			duel.BlankNoteBonus (eventInfo.NoteHit);
		} else {
			//check magic
			if (eventInfo.IsMagic) {
                //force attacking state ( our input doesnt trigger enemy magic )
                duel.MagicAttack (true, eventInfo.NoteHit);
			} else {
                duel.RegularAttack (m_engine.IsAttacking, eventInfo.NoteHit);
			}
		}
	}

	public void RaiseActorDeadEvent(BattleActor _actor){
		if (actorDeadEventHandler == null)
			return;
		var eventInfo = new ActorDeadEventInfo (_actor);
		actorDeadEventHandler.Invoke (this, eventInfo);
	}

	#endregion

	#region ACTOR_DIE

	/** Called by an FightDuel when an actor's dead*/
	public void OnActorDead(BattleActor _actor, FightDuel _duel){
		int index = m_duels.IndexOf (_duel);
		//Get alive remaining actors and move them
		//-- here --
		//If duel is finished ( there's no more actor on one side )
		if (_duel.Enabled == false) {
			//Check Combat End
			bool end = true;
			int repTrack = 0;
			for( int i=0; i < m_duels.Count; i ++){
				bool tempEnabled = m_duels[i].Enabled;
				//find a replacement track
				if( tempEnabled )
					repTrack = i;
				//check end of the whole battle
				end = end && !tempEnabled;
			}

			//Notify BattleEngine
			if (end) {
				bool win = _actor.GetType() == typeof(BattleEnemy);
				endBattleEventHandler.Invoke (this, new EndBattleEventInfo(win));
			}else{				
				m_engine.OnDisableTrack( index, repTrack ) ;
			}
		}
	}

	#endregion

	#region LAUNCH_MAGIC

	/// <summary>
    /// Launched by the duel. When a magic is done, the duel call this method to effectively launch the magic ( graphically )
    /// </summary>
	public BattleMagic LaunchMagic(BattleActor _actor, List<BattleActor> _targets,FightDuel _duel, HitAccuracy _accuracy){
		BattleMagic magic = _actor.LaunchMagic (_targets[0],_duel.ID,_accuracy);
		if( magic != null )
			m_engine.OnLaunchMagic (magic);
        return magic;
	}

	public void OnMagicEnded( BattleMagic _magic){
		m_engine.OnMagicEnded (_magic);
	}

	#endregion

	int FindDuelIndexByActor( BattleActor _actor){
		for( int i=0; i < m_duels.Count; i ++){
			if( m_duels[i].Contains(_actor)){
				return i;
			}
		}
		return -1;
	}

	public static BattleFightManager instance
	{
		get
		{
			return _instance;
		}
	}

	#region DUEL_CLASS
	public class FightDuel{
		public int ID;
		List<BattleActor> m_characters;
		List<BattleActor> m_enemies;

		BattleDamageTextManager m_damageTextManager;
		BattleFightManager m_manager;

		bool m_enabled = true;

		public FightDuel(int _id,BattleDamageTextManager _damageMngr, BattleFightManager _manager){
			m_characters = new List<BattleActor>();
			m_enemies = new List<BattleActor>();
			m_damageTextManager = _damageMngr;
			m_manager = _manager;
			ID = _id;
		}

		public void Start(BattleCharacter _char, BattleEnemy _enemy){
            m_characters.Clear();
			m_characters.Add (_char);
			_char.FightDuel = this;
            m_enemies.Clear();
			m_enemies.Add (_enemy);
			_enemy.FightDuel = this;
		}

		#region ATTACKS
		public int RegularAttack(bool _fromPlayer, NoteData _noteData){
			//if attack from player missed
			if (_fromPlayer && _noteData.HitAccuracy == HitAccuracy.MISS)
				return -1;
			//Affect attackers & defenders
			List<BattleActor> m_attackers = null;
			List<BattleActor> m_defenders = null;

			m_attackers = _fromPlayer ? m_characters : m_enemies;
			m_defenders = _fromPlayer ? m_enemies : m_characters;
			
			int totalDamage = 0;

			//Begin attack process
			BattleActor attacker = null;
			for (int i=0; i < m_attackers.Count; i++) {		
				attacker = m_attackers[i];

				totalDamage += m_attackers [i].GetAppliedAttackingPower (_noteData);
			}
			if (totalDamage < 0)
				totalDamage = 0;

			//Defenders take damage
			BattleActor defender = null;
			for (int i=0; i < m_defenders.Count; i++) {
				defender = m_defenders[i];
				int damage = defender.TakeDamage (totalDamage,_noteData);
				//text
				m_damageTextManager.LaunchDamage(defender.gameObject,damage,false);
			}
			return totalDamage;
		}

		public int MagicAttack(bool _fromPlayer, NoteData _noteData){
			//if attack from player missed
			if (_fromPlayer && _noteData.HitAccuracy == HitAccuracy.MISS)
				return -1;
			//Affect attackers & defenders
			List<BattleActor> m_attackers = null;
			List<BattleActor> m_defenders = null;

			m_attackers = _fromPlayer ? m_characters : m_enemies;
			m_defenders = _fromPlayer ? m_enemies : m_characters;

			int totalDamage = 0;
            BattleMagic magic = null;

			//Begin attack process == actually find the real caster
			BattleActor attacker = null;
			for (int i=0; i < m_attackers.Count; i++) {		
				attacker = m_attackers[i];
                //apply magic attack and get the power 
				totalDamage += m_attackers [i].GetAppliedMagicAttackPower (_noteData.HitAccuracy);
                //Effectively Launch the magic
                magic = m_manager.LaunchMagic(attacker, m_defenders, this, _noteData.HitAccuracy);
			}
			if (totalDamage < 0)
				totalDamage = 0;

			//Defenders take damage
			BattleActor defender = null;
			for (int i=0; i < m_defenders.Count; i++) {
				defender = m_defenders[i];
				//int damage = defender.TakeMagicDamage (totalDamage, magic);
                //text
                //m_damageTextManager.LaunchDamage(defender.gameObject,damage,false);
			}
			return totalDamage;
		}

		/// <summary>
		/// Triggered when a note is hit on a disabled track ( enemy is dead )
		/// </summary>
		public int BlankNoteBonus( NoteData _noteData){
			return 0;
		}

        #endregion

        public void OnActorTakeDamage( BattleMagic _magic )
        {
            //MAGIC DAMAGE
            int damage = _magic.Target.TakeMagicDamage(_magic.Caster.CurrentStats.Magic, _magic);
            m_damageTextManager.LaunchDamage(_magic.Target.gameObject, damage, false);
        }

        public void OnActorTakeDamage(BattleActor _target, int _damage){
            //
			m_damageTextManager.LaunchDamage (_target.gameObject, _damage, false);
		}

		/** Called by an actor when it's dead*/
		public bool OnActorDead(BattleActor _actor){
			bool enemiesDead = m_enemies.Contains (_actor);
			bool charsDead = m_characters.Contains (_actor);
			if( enemiesDead || charsDead ){
				if( enemiesDead){
					m_enabled = (CountAlive(m_enemies) > 1);
				}else{
					m_enabled = (CountAlive(m_characters) > 1);
				}
				//notify the manager
				m_manager.OnActorDead(_actor,this);
				return true;
			}
			return false;
		}

		/** Returns true if the actor is inside this duel */
		public bool Contains(BattleActor _actor){
			return m_characters.Contains (_actor) || m_enemies.Contains (_actor);
		}

		/** Returns how many actors are alive in the list */
		public int CountAlive(List<BattleActor> _actors){
			int aliveCount = 0;
			for (int i=0; i < _actors.Count; i++) {
				if( !_actors[i].isDead ) 
					aliveCount ++;
			}
			return aliveCount;
		}

		public bool Enabled {
			get {
				return m_enabled;
			}
		}
	}
	#endregion
}

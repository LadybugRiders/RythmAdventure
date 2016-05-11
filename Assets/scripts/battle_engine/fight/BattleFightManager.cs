using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFightManager : MonoBehaviour {

	[SerializeField] BattleEngine m_engine;
	[SerializeField] BattleFightMagicManager m_magicManager;
	[SerializeField] BattleDamageTextManager m_damageTextManager;

	[SerializeField] List<BattleCharacter> m_party;
	[SerializeField] List<BattleEnemy> m_enemies;
	List<FightDuel> m_duels;

    [SerializeField] SpriteRenderer m_backgroundSprite;

	/** used when a note is hit to determine the current actor action */
	public enum ActorAttackAction { NONE, ATTACK, MAGIC, _COUNT };

	public static float DEPTH_BACKGROUND = 30.0f;
	public static float DEPTH_PLAYERS = 10.0f;
	public static float DEPTH_UI = 0.0f;

	void Awake(){
		m_duels = new List<FightDuel>();
		for(int i=0; i<3 ; i ++){
			m_duels.Add( new FightDuel(i,m_damageTextManager,this) );
		}
	}

	// Use this for initialization
	void Start () {
	}

	/// <summary>
	/// Called by battle engine at start of the scene. Loads all the actors
	/// </summary>
	public void Load(BattleDataAsset battleData){
		m_party [0].Load ("rodriguez");
		m_party [1].Load ("player");
		m_party [2].Load ("nidan");

        //Loads enemies
        if (battleData != null)
        {
            for(int i=0; i < battleData.Enemies.Count; i++)
            {
                if(m_enemies[i])
                {
                    Transform parent = m_enemies[i].transform.parent;
                    Vector3 position = m_enemies[i].transform.position;
                    Destroy(m_enemies[i].gameObject);
                    var prefab = battleData.Enemies[i];
                    if( prefab != null)
                    {
                        //instantiate enemy
                        GameObject go = Instantiate(prefab) as GameObject;
                        m_enemies[i] = go.GetComponent<BattleEnemy>();
                        go.transform.SetParent(parent, true);
                        go.transform.position = position;
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

	public void AddNote(NoteData _note){
		switch (_note.Subtype) {
			case NoteData.NoteSubtype.REGULAR :
			case NoteData.NoteSubtype.MAGIC :
				ProcessRegular(_note); break;
		}
	}

	public void ProcessRegular(NoteData _note ){
		//no attack for long note's head
		if (_note.Type == NoteData.NoteType.LONG && _note.Head)
			return;

		//Get the duel
		FightDuel duel = m_duels [_note.TrackID];
		duel.RegularAttack ( m_engine.IsAttacking,_note);
	}

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
				m_engine.OnFightEnded(true);
			}else{				
				m_engine.OnDisableTrack( index, repTrack ) ;
			}
		}
	}

	#endregion

	#region LAUNCH_MAGIC

	/** Launched by the FightDuel when magic is full */
	public void LaunchMagic(BattleActor _actor, List<BattleActor> _targets,FightDuel _duel){
		BattleFightMagic magic = _actor.LaunchMagic (_targets[0],_duel.ID);
		if( magic != null )
			m_engine.OnLaunchMagic (magic);
	}

	public void OnMagicEnded( BattleFightMagic _magic){
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
			if (_fromPlayer && _noteData.HitAccuracy == BattleScoreManager.Accuracy.BAD)
				return -1;
			//Affect attackers & defenders
			List<BattleActor> m_attackers = null;
			List<BattleActor> m_defenders = null;
			if (_fromPlayer) {
				m_attackers = m_characters;
				m_defenders = m_enemies;
			} else {
				m_attackers = m_enemies;
				m_defenders = m_characters;
			}
			
			int totalDamage = 0;
			ActorAttackAction tempAction;

			//Begin attack process
			BattleActor attacker = null;
			for (int i=0; i < m_attackers.Count; i++) {		
				attacker = m_attackers[i];
				tempAction = attacker.AddNote(_noteData,true);

				if( tempAction == ActorAttackAction.MAGIC && !attacker.isCasting){
					m_manager.LaunchMagic(attacker,m_defenders,this);
				}else{
					if( attacker.isCasting ){
						m_attackers [i].MagicAttack (_noteData);
					}else{
						totalDamage += m_attackers [i].Attack (_noteData);
					}
				}
			}
			if (totalDamage < 0)
				totalDamage = 0;

			//Defenders take damage
			BattleActor defender = null;
			for (int i=0; i < m_defenders.Count; i++) {
				defender = m_defenders[i];
				defender.AddNote(_noteData,false);	
				int damage = defender.TakeDamage (totalDamage,_noteData);
				m_damageTextManager.LaunchDamage(defender.gameObject,damage,false);
			}
			return totalDamage;
		}

		#endregion

		public void OnActorTakeDamage(BattleActor _actor, int _damage){
			m_damageTextManager.LaunchDamage (_actor.gameObject, _damage, false);
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

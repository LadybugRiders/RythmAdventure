using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFightManager : MonoBehaviour {

    static BattleFightManager _instance;

	[SerializeField] BattleEngine m_engine;
	[SerializeField] BattleDamageTextManager m_damageTextManager;

	[SerializeField] List<BattleCharacter> m_party;
	[SerializeField] List<BattleEnemy> m_enemies;

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

    public event System.EventHandler<DamageEventInfo> damageEventHandler;
    public class DamageEventInfo : System.EventArgs
    {
        public BattleActor Actor { get; set; }
        public int Damage { get; set; }
        public DamageEventInfo(BattleActor _actor, int _damage)
        {
            Actor = _actor;
            Damage = _damage;
        }
    }

    public static float DEPTH_BACKGROUND = 30.0f;
	public static float DEPTH_PLAYERS = 10.0f;
	public static float DEPTH_UI = 0.0f;

	void Awake(){
        _instance = this;
	}

	// Use this for initialization
	void Start () {
		BattleTracksManager.instance.noteEventHandler += OnReceiveNoteEvent;
		BattleTracksManager.instance.actionEventHandler += OnReceiveActionEvent;
        damageEventHandler += OnReceiveDamageEvent;
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

        //Background
        if (battleData.Background != null)
            m_backgroundSprite.sprite = battleData.Background;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    #region DAMAGE_DEALING

    public int ComputeDamage(BattleAction _action, BattleActor _caster, BattleActor _target)
    {
        var type = _action.GetType();
        return _caster.CurrentStats.Attack;
    }

    #endregion

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
        int trackId = eventInfo.NoteHit.TrackID;
        
		//If final note, no action is done
		if (eventInfo.IsFinal) {
			//duel.BlankNoteBonus (eventInfo.NoteHit);
		} else {
            //in offense, the player always attacks 
            if( eventInfo.Offensive)
            {
                //Get caster
                BattleActor caster = m_party[trackId];
                //check magic
                if (eventInfo.IsMagic)
                {
                    //force attacking state ( our input doesnt trigger enemy magic )
                    SpecialAction(caster, trackId);
                }
                else
                {
                    RegularAction(caster, trackId);
                }
            }
            else // in defense, we can launch a magic back
            {
                BattleActor caster = m_enemies[trackId];
                RegularAction(caster, trackId);
            }
		}
	}

    public void OnReceiveDamageEvent(object sender, DamageEventInfo eventInfo)
    {
        m_damageTextManager.LaunchDamage(eventInfo.Actor.gameObject,eventInfo.Damage,false);
    }

    public void RaiseActorDeadEvent(BattleActor _actor){
		if (actorDeadEventHandler == null)
			return;
		var eventInfo = new ActorDeadEventInfo (_actor);
		actorDeadEventHandler.Invoke (this, eventInfo);
	}

    public void RaiseActorDamageEvent(BattleActor _actor, int _damage)
    {
        if (damageEventHandler == null)
            return;
        var eventInfo = new DamageEventInfo(_actor,_damage);
        damageEventHandler.Invoke(this, eventInfo);
    }

    #endregion

    #region ACTIONS

    void RegularAction(BattleActor _caster, int _trackId)
    {
        BattleActor target = _caster.Type == BattleActor.ActorType.ENEMY ? (BattleActor) m_party[_trackId] : m_enemies[_trackId];
        int damage = ComputeDamage(_caster.AttackAction, _caster, target);
        _caster.Attack(target, damage);
    }

    void SpecialAction(BattleActor _caster, int _trackId)
    {
        BattleMagic magic = _caster.GetMagic(_caster.Type == BattleActor.ActorType.CHARACTER);
        if (magic == null)
        {
            //RegularAction(_caster, _trackId);
            return;
        }
        BattleActor target = m_enemies[_trackId];
        int damage = ComputeDamage(magic, _caster, target);
        _caster.LaunchMagic(target, damage, true);
    }
    #endregion

    #region ACTOR_DIE
    
    public void OnActorDead(BattleActor _actor){

        int index = FindActorIndex(_actor);

        var chara = m_party[index];
        var enemy = m_enemies[index];
        
		//Check Combat End
		bool end = true;
		int repTrack = 0;

		for( int i=0; i < m_party.Count; i ++){
			bool tempEnabled = ! IsDuelFinished(i);
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

	#endregion
    
	int FindActorIndex( BattleActor _actor){
        int i = m_party.FindIndex(a => a == _actor);
        if (i < 0)
            i = m_enemies.FindIndex(e => e == _actor);
		return i;
	}

    /// <summary>
    /// Return true if either one party is dead
    /// </summary>
    bool IsDuelFinished(int _index)
    {
        return m_party[_index].isDead || m_enemies[_index].isDead;
    }

	public static BattleFightManager instance
	{
		get
		{
			return _instance;
		}
	}
    
}

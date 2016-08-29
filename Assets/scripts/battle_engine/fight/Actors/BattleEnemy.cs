using UnityEngine;
using System.Collections;

public class BattleEnemy : BattleActor {

	[SerializeField] Animator m_animator;

	[SerializeField] Animator m_smokeAnimator;

	// Use this for initialization
	override protected void Start () {
		base.Start ();
		m_type = ActorType.ENEMY;
        
		m_animator.Play("idle",0, Random.Range(0.0f,1.0f) );
    }    
	
	// Update is called once per frame
	override protected void Update () {
		base.Update ();
	}

	#region LOADING 
	public void Load(string _name, int _level){
        base.Load(_name);
        var dataManager = DataManager.instance.EnemiesManager;
        var levelData = dataManager.GetFullStats(_name, _level);
        if (levelData != null)
        {
            m_maxStats = new Stats(levelData.Stats);
            m_currentStats = new Stats(levelData.Stats);
        }
        CurrentStats.MP = 0;
    }
	#endregion

	#region ACTION
	
	override public int GetAppliedAttackingPower(NoteData _noteData){
		m_state = State.ATTACKING;

		if( Utils.IsAnimationStateRunning(m_animator,"attack") ){
			m_animator.Play("attack",0,0.0f);
		}else{
			m_animator.SetTrigger ("attackTrigger");
		}

		return CurrentStats.Attack;

	}
		
	#endregion

	override public int TakeDamage(int _damage, NoteData _note){
		int damage = base.TakeDamage (_damage, _note);
		if( Utils.IsAnimationStateRunning(m_animator,"hit") ){
			m_animator.Play("hit",0,0.0f);
		}else{
			m_animator.SetTrigger ("hitTrigger");
		}

		CheckDeath ();
		return damage;
	}

	override protected bool Die(){
		m_smokeAnimator.SetTrigger ("explodeTrigger");
		base.Die ();
        gameObject.SetActive(false);
		return true;
	}
}

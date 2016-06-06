using UnityEngine;
using System.Collections;

public class BattleCharacter : BattleActor {

	[SerializeField] BattleCharacterAnimator m_charAnimator;

	override protected void Start () {
		base.Start ();
		m_type = ActorType.CHARACTER;
        m_lifeGauge.ChangeOrientation(UIGauge.ORIENTATION.HORIZONTAL, UIGauge.ALIGN.LEFT);
        m_manaGauge.ChangeOrientation(UIGauge.ORIENTATION.HORIZONTAL, UIGauge.ALIGN.LEFT);
    }

	#region LOADING 
	override public void Load(string _name){
		m_charAnimator.LoadSprites(_name);
        DataCharManager.LevelUpData levelup = DataManager.instance.CharacterManager.GetFullStats(_name);
        if(levelup != null)
        {
            m_maxStats = new Stats(levelup.Stats);
            m_currentStats = new Stats(levelup.Stats);
        }
	}
	#endregion

	override protected void UpdateAttacking(){

	}

	#region ACTION

	override public int Attack( NoteData _noteData){
		m_state = State.ATTACKING;

		m_charAnimator.Attack ();

		this.AddMP (5);

		return CurrentStats.Attack;
	}

	override public int TakeDamage(int _damage, NoteData _note){
		int damage = _damage;
		damage -= CurrentStats.Defense ;
		//Reduce damage by blocking
		switch(_note.HitAccuracy){
			case BattleScoreManager.Accuracy.PERFECT :
				damage = damage - (int) (damage * CurrentStats.blockPerfectModifier);
				break;
			case BattleScoreManager.Accuracy.GREAT :
				damage = damage - (int) (damage * CurrentStats.blockGreatModifier);
				break;
			case BattleScoreManager.Accuracy.GOOD :
				damage = damage - (int) (damage * CurrentStats.blockGoodModifier);
				break;
			case BattleScoreManager.Accuracy.BAD :
				damage = damage - (int) (damage * CurrentStats.blockBadModifier);
				break;
		}

		if (damage < 0)
			damage = 0;
		CurrentStats.HP -=  damage;

		m_charAnimator.TakeHit ();

		RefreshLifeGauge ();

		CheckDeath ();

		return damage;
	}

	#endregion

	override protected bool Die(){
		base.Die ();
		Utils.SetAlpha(m_sprite,0.0f);
		return true;
	}
}

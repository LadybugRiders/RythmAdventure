using UnityEngine;
using System.Collections;

public class BattleCharacter : BattleActor {

	[SerializeField] BattleCharacterAnimator m_charAnimator;
    [SerializeField] CharacterBuild m_build;

	override protected void Start () {
		base.Start ();
		m_type = ActorType.CHARACTER;

        m_lifeGauge.ChangeOrientation(UIGauge.ORIENTATION.HORIZONTAL, UIGauge.ALIGN.LEFT);
        m_manaGauge.ChangeOrientation(UIGauge.ORIENTATION.HORIZONTAL, UIGauge.ALIGN.LEFT);

    }

	#region LOADING 
	override public void Load(string _id){
        base.Load(_id);
        
        var charData = ProfileManager.instance.GetCharacter(_id);
        //Load equipement and looks
        m_build.Load(charData);

        //Get Stats
        var stats = DataManager.instance.CharacterManager.ComputeStats(charData);
        if(charData != null )
        {
            m_maxStats = new Stats(stats);
            m_currentStats = new Stats(stats);
        }
        RefreshUI();

        //Load Attacks & Magics
        var inventory = DataManager.instance.InventoryManager;
        if (charData.Attack != null && !string.IsNullOrEmpty( charData.Attack.Id ) )
        {
            var attackData = inventory.GetActionData(charData.Attack.Id, "attack");
            string pathToPrefab = "prefabs/battle/" + "attack/" + attackData.Prefab ;
            //Load prefab
            GameObject go = Instantiate(Resources.Load(pathToPrefab)) as GameObject;
            if (go != null)
            {
                go.transform.SetParent(m_attacksGroup, false);
                m_attack = go.GetComponent<BattleAction>();
            }
        }
        if( charData.Magics != null && charData.Magics.Count > 0)
        {
            for(int i=0; i < charData.Magics.Count; i++)
            {
                var magic = charData.Magics[i];
                if(!string.IsNullOrEmpty(magic.Id) && magic.equipped)
                {
                    var magicData = inventory.GetActionData(magic.Id, "magic");
                    var pathToPrefab = "prefabs/battle/" + "magic/" + magicData.Prefab;
                    //Load prefab
                    var go = Instantiate(Resources.Load(pathToPrefab)) as GameObject;
                    if (go != null)
                    {
                        go.transform.SetParent(m_attacksGroup, false);
                        m_magics[0] = go.GetComponent<BattleMagic>();
                        m_magics[1] = go.GetComponent<BattleMagic>();
                    }
                }
            }
        }
    }
	#endregion

	override protected void UpdateAttacking(){

	}

	#region ACTION

	override public int GetAppliedAttackingPower( NoteData _noteData){
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
			case HitAccuracy.PERFECT :
				damage = damage - (int) (damage * CurrentStats.blockPerfectModifier);
				break;
			case HitAccuracy.GREAT :
				damage = damage - (int) (damage * CurrentStats.blockGreatModifier);
				break;
			case HitAccuracy.GOOD :
				damage = damage - (int) (damage * CurrentStats.blockGoodModifier);
				break;
			case HitAccuracy.MISS :
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

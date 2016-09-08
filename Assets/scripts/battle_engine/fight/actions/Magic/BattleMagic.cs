using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleMagic : BattleAction {
    
	/** Mana used by second when the magic is active */
	protected int m_decreaseRate = 1;

	// Use this for initialization
	override protected void Start () {
        base.Start();
		for (int i = 0; i < m_effects.Count; i++) {
			((BattleMagicEffect)m_effects[i]).Magic = this;
		}
	}
	
	// Update is called once per frame
	override protected void Update () {
        base.Update();
	}

	#region LAUNCH

	/** Override this to launch the magic */
	override protected void Launch(){

        //replace magic on the user
        Transform t = transform;
        float z = t.position.z;
        t.position = m_caster.transform.position;
        Utils.SetPositionZ(t, z);

        //Launch effect 
        BattleMagicEffect effect =(BattleMagicEffect) GetFreeEffect();
        if (effect == null)
        {
            Debug.Log("no effect");
            return;
        }
        effect.Launch(m_caster.transform.position, m_target.transform.position);
    }

    #endregion
    /// <summary>
    /// Called by an effect when it has hit its target(s) */
    /// </summary>
    override public void OnHit(){
        int damage = m_caster.GetAppliedMagicAttackPower(m_noteAccuracy);
		damage = m_target.TakeMagicDamage (damage,this);
		m_target.FightDuel.OnActorTakeDamage (m_target, damage);
		//play main sound if any
		if (m_mainSoundClip) {
			m_audioSource.clip = m_mainSoundClip;
			m_audioSource.Play();
		}
	}

    override public void Die()
    {
        base.Die();
    }
    
	#region GETTERS-SETTERS

	#endregion
}

using UnityEngine;
using System.Collections;

public class BattleMagicEffectHoming : BattleMagicEffect {

	[SerializeField] protected SpriteRenderer m_destructionSprite;

	Animator m_destructionAnimator;

	private string m_state = "idle";

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
		m_destructionAnimator = m_destructionSprite.GetComponent<Animator> ();
		m_destructionSprite.enabled = false;
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
		if (m_launched) {
			switch( m_state ){
			    case "homing": UpdateHoming(); break;
			    case "exploding" : UpdateExplosion(); break;
			}
		}
	}

	void UpdateHoming(){
		//wait for the homing animation to end
		if ( ! m_animationComponent.isPlaying ) {
			m_magic.OnHit();
			m_state = "exploding";
			m_destructionAnimator.Play("explosion");
		}
	}

	void UpdateExplosion(){
		//wait for the animation of the explosion to finish
		if (m_destructionAnimator.GetCurrentAnimatorStateInfo (0).IsName ("idle")) {			
			Die ();
			m_state = "idle";
			m_destructionSprite.enabled = false;
		}
	}

	override public void Launch(Vector3 _origin, Vector3 _destination){
        base.Launch(_origin,_destination);
		Utils.SetAlpha (m_effectSprite, 1f);
		transform.position = _destination;
		//launch animation
		m_animationComponent.Play();
		m_destructionSprite.enabled = true;
		m_state = "homing";
	}

	override public void Die(){
		base.Die ();
		m_destructionSprite.enabled = false;
	}
}

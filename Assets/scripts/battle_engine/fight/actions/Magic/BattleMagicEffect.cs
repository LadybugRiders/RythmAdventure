using UnityEngine;
using System.Collections;

/** Visual effect of the magic, such as a fire ball or cure drop */ 
public class BattleMagicEffect : BattleActionEffect {
    
	/** Set by the BattleFightMagic when started */
	protected BattleMagic m_magic;

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

	override public void Launch(Vector3 _origin, Vector3 _destination){
        base.Launch(_origin,_destination);
		Utils.SetAlpha (m_effectSprite, 1f);
		transform.position = _destination;
		TweenEngine.Tween tween = TweenEngine.instance.RotateAroundZTo (m_effectSprite.transform, 360.0f, 0.3f, false, 1, "Die");
		tween.CallbackObject = gameObject;
	}

	override public void Die(){
        base.Die();
		Utils.SetAlpha (m_effectSprite, 0f);
	}


	public BattleMagic Magic {
		get {
			return m_magic;
		}
		set {
			m_magic = value;
		}
	}
}

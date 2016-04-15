using UnityEngine;
using System.Collections;

public class BattleCharacterAnimator : MonoBehaviour {

	[SerializeField] Animator m_animator;

	[SerializeField] GameObject m_body;
	[SerializeField] GameObject m_eyes;
	[SerializeField] GameObject m_arm1;
	[SerializeField] GameObject m_arm2;

	[SerializeField] GameObject m_eyelids;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Plays the attack animation.
	/// </summary>
	public void Attack(){
		if (Utils.IsAnimationStateRunning (m_animator, "attack")) {
			m_animator.Play ("attack", 0, 0.0f);
		} else {
			m_animator.SetTrigger ("attackTrigger");
		}
	}

	/// <summary>
	/// Place the animation "takes a hit".
	/// </summary>
	public void TakeHit(){
		if (Utils.IsAnimationStateRunning (m_animator, "hit")) {
			m_animator.Play ("hit", 0, 0.0f);
		} else {
			m_animator.SetTrigger ("hitTrigger");
		}
	}

	#region RENDERING

	/// <summary>
	/// Replaces the eyelids gameObject, or deactivate if paramaeter is null
	/// </summary>
	/// <param name="_go">_go.</param>
	public void ReplaceEyelids(GameObject _go){
		if (_go == null) {
			m_eyelids.SetActive(false);
			return;
		}
		Transform t = m_eyelids.transform;
		Utils.SetParentKeepTransform( _go.transform, t.parent );
		m_eyelids = _go;
		Destroy (t.gameObject);
	}

	public void SetColor( Color _color){
		m_body.GetComponent<SpriteRenderer> ().color = _color;
		m_arm1.GetComponent<SpriteRenderer> ().color = _color;
		m_arm2.GetComponent<SpriteRenderer> ().color = _color;
		m_eyelids.GetComponent<SpriteRenderer> ().color = _color;
	}

	public void LoadSprites(string _characterName){
		//change Eyelid
		JSONObject eyelidObject = DataManager.instance.GameData.GetField ("playerEyelid");
		if (eyelidObject && Utils.IsValidString(eyelidObject.str)) {
			GameObject goEyelid = Instantiate (Resources.Load ("prefabs/character/eyelids/" + eyelidObject.str) as GameObject);
			ReplaceEyelids (goEyelid);
		}else{
			ReplaceEyelids(null);
		}
		//change color
		JSONObject colorObject = DataManager.instance.GameData.GetField ("playerColor");
		Color c = new Color();
		c.r = colorObject[0].n;
		c.g = colorObject[1].n;
		c.b = colorObject[2].n;
		c.a = 1;
		SetColor(c);
	}

	#endregion
}

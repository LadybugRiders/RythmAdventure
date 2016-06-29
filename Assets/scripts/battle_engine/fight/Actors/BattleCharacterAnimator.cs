using UnityEngine;
using System.Collections;

public class BattleCharacterAnimator : MonoBehaviour {

	[SerializeField] Animator m_animator;

	[SerializeField] SpriteRenderer m_body;
	[SerializeField] SpriteRenderer m_eyebrows;
	[SerializeField] SpriteRenderer m_arm;
	[SerializeField] SpriteRenderer m_eyes;

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

	public void SetColor( Color _color){
		m_body.color = _color;
		m_arm.color = _color;
		//m_eyebrows.color = _color;
	}

	public void LoadSprites(string _characterName){
		//change color
		JSONObject colorObject = DataManager.instance.GameData.GetField ("playerColor");
        if( colorObject != null)
        {
            Color c = new Color();
            c.r = colorObject[0].n;
            c.g = colorObject[1].n;
            c.b = colorObject[2].n;
            c.a = 1;
            SetColor(c);
        }
	}

	#endregion
}

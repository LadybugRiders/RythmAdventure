using UnityEngine;
using System.Collections;

public class UIBattlePhaseTitle : MonoBehaviour {

	[SerializeField] SpriteRenderer m_titleRenderer;

	[SerializeField] Sprite m_attackSprite;
	[SerializeField] Sprite m_defenseSprite;

	Vector3 m_centerPosition;

	bool m_attack = false;

	enum State { IDLE, LEAVING, COMING };
	State m_state = State.COMING;

	// Use this for initialization
	void Start () {		
		m_centerPosition = transform.localPosition;
		//Utils.SetAlpha (m_titleRenderer, 0.0f);
		Utils.SetLocalPositionX (transform, -35.0f);
	}

	public void Switch( bool _attack ){
		m_attack = _attack;
		if (m_state == State.IDLE) {
			Leave ();
		} else {
			ComeBack();
		}
	}

	void Leave(){
		m_state = State.LEAVING;
		Vector3 pos = transform.localPosition;
		pos.x = 30.0f;
		TweenEngine.instance.PositionTo (transform, pos, 0.5f, "ComeBack");
	}

	void ComeBack(){
		m_state = State.COMING;
		if (m_attack) {
			m_titleRenderer.sprite = m_attackSprite;
		} else {
			m_titleRenderer.sprite = m_defenseSprite;
		}
		Utils.SetLocalPositionX (transform, -35.0f);
		TweenEngine.instance.PositionTo (transform, m_centerPosition, 0.5f, "OnCentered");
	}

	void OnCentered(){
		m_state = State.IDLE;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

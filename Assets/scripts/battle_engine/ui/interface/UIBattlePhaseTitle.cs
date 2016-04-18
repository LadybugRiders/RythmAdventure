using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBattlePhaseTitle : MonoBehaviour {

	[SerializeField] Text m_titleRenderer;

	[SerializeField] string m_attackString;
	[SerializeField] string m_defenseString;

    float leftPositionX = -305.0f;
    float rightPositionX = 305.0f;
	Vector3 m_centerPosition;

	bool m_attack = false;

	enum State { IDLE, LEAVING, COMING };
	State m_state = State.COMING;

	// Use this for initialization
	void Start () {		
		m_centerPosition = transform.localPosition;
		//Utils.SetAlpha (m_titleRenderer, 0.0f);
		Utils.SetLocalPositionX (transform, leftPositionX);
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
		pos.x = rightPositionX;
		TweenEngine.instance.PositionTo (transform, pos, 0.5f, "ComeBack");
	}

	void ComeBack(){
		m_state = State.COMING;
		if (m_attack) {
			m_titleRenderer.text = m_attackString;            
		} else {
			m_titleRenderer.text = m_defenseString;
		}
		Utils.SetLocalPositionX (transform, leftPositionX);
		TweenEngine.instance.PositionTo (transform, m_centerPosition, 0.5f, "OnCentered");
	}

	void OnCentered(){
		m_state = State.IDLE;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

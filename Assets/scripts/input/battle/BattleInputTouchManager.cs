using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleInputTouchManager : MonoBehaviour {
	
	[SerializeField] private BattleTracksManager m_tracksManager;

	[SerializeField] private BoxCollider2D m_attackCollider;
	[SerializeField] private BoxCollider2D m_defendCollider;

	bool m_pressed = false;
    /// <summary>
    /// The id of the button pressed
    /// </summary>
	int m_inputDown = -1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Check press
		if (m_pressed==false && IsPressed() ){
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint( CustomGetTouchPosition() );
			Vector2 touchPos = new Vector2(worldPoint.x,worldPoint.y);
			Collider2D coll = Physics2D.OverlapPoint(touchPos);
			//check attack 
			if( m_attackCollider == coll ){				
				m_pressed = true;
				m_inputDown = 1;
				m_tracksManager.OnInputDown(1);
			}else if( m_defendCollider == coll){				
				m_pressed = true;
				m_inputDown = -1;
				m_tracksManager.OnInputDown(-1);
			}
		}

		//Check Release
		if( m_pressed && IsReleased() ){
			m_pressed = false;
			m_tracksManager.OnInputUp(m_inputDown);
		}
	}

	Vector2 CustomGetTouchPosition(){
#if UNITY_STANDALONE || UNITY_EDITOR
		return Input.mousePosition;
#else
		return Input.GetTouch(0).position;
#endif
	}

	bool IsPressed(){
#if UNITY_STANDALONE|| UNITY_EDITOR
			return Input.GetMouseButtonDown(0);
#else
			return Input.touchCount == 1;
#endif
	}

	bool IsReleased(){
#if UNITY_STANDALONE|| UNITY_EDITOR
	return Input.GetMouseButtonUp(0);
#else
	return Input.touchCount < 1 ;
#endif	
	}
}

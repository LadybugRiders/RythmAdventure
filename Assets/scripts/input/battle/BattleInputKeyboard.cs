using UnityEngine;
using System.Collections;

public class BattleInputKeyboard : MonoBehaviour {

	[SerializeField] private BattleTracksManager m_tracksManager;

	[SerializeField] string m_inputAttack = "right";
	[SerializeField] string m_inputDefend = "left";
	// Use this for initialization
	void Start () {
		if (Application.platform == RuntimePlatform.Android) {
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (CheckInput (m_inputAttack, 1)) {

		} else if(CheckInput (m_inputDefend, -1) ){

		}
	}

	bool CheckInput( string _input, int _index){
		if (Input.GetKeyDown (_input)) {
			m_tracksManager.OnInputDown (_index);
			return true;
		} else if( Input.GetKeyUp(_input) ){
			m_tracksManager.OnInputUp(_index);
			return true;
		}
		return false;
	}
}

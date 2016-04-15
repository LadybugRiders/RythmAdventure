using UnityEngine;
using System.Collections;

public class DebugFirstScene : MonoBehaviour {

	public static DebugFirstScene instance = null;

	bool m_launched = false;
	bool m_generatorFound = false;

	float m_btnSize;

	//variables to modify
	float m_deltaTimeSynchro = 0.0f;

	void Awake(){
		m_btnSize = Screen.width * 0.1f;
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (this.gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (m_launched) {
			if( m_generatorFound == false){
				BattleNotesGenerator gen = FindObjectOfType( typeof( BattleNotesGenerator) ) as BattleNotesGenerator;
				if( gen != null ){
					m_generatorFound = true;
					gen.TimeSynchroDelta = m_deltaTimeSynchro;
				}
			}
		}
	}

	void OnGUI(){
		if (m_launched == true) {
			if (GUI.Button (new Rect (0, 0, m_btnSize, m_btnSize), "<")) {
				Reboot ();
			}
		} else {
			if (GUI.Button (new Rect (Screen.width - m_btnSize*2f, Screen.height - m_btnSize*2f, m_btnSize*2f, m_btnSize*2f), "Launch")) {
				LaunchScene ();
			}
			float y = Screen.height *0.5f;


			//SYNCHRO
			GUI.Label(new Rect (160, y - 30, 200, 30),"SYNCHRONISATION");

			GUI.Label(new Rect (0, y, m_btnSize * 1.5f, m_btnSize),"Plus Rapide");
			if (GUI.Button (new Rect (m_btnSize * 1.5f, y, m_btnSize, m_btnSize), "-")) {
				m_deltaTimeSynchro -= 0.05f;
			}
			GUI.Label (new Rect (m_btnSize * 3.6f, y, m_btnSize, m_btnSize), "" + m_deltaTimeSynchro);		
			if (GUI.Button (new Rect (m_btnSize * 5f, y, m_btnSize, m_btnSize), "+")) {
				m_deltaTimeSynchro += 0.05f;
			}
			GUI.Label(new Rect (m_btnSize *6.5f, y, m_btnSize*2, m_btnSize),"Plus Lent");
		}
	}

	void LaunchScene(){
		m_launched = true;
		Application.LoadLevel ("test");
	}

	public void Reboot(){
		m_launched = false;
		m_generatorFound = false;
		Application.LoadLevel ("debugFirstScene");
	}
}

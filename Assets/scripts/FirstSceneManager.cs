using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FirstSceneManager : MonoBehaviour {
	
	[SerializeField] Text m_text;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (DataManager.instance.IsLoaded) {
			Application.LoadLevel( "main_menu" );
		}
	}
	
	public void Quit(){		
		Application.LoadLevel ("locker_room");
	}
	
	public void DeleteAndReset(){		
		PlayerPrefs.DeleteAll ();
		Application.LoadLevel ("first_scene");
	}

}

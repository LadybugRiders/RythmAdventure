using UnityEngine;
using System.Collections;

public class MenuMainManager : MonoBehaviour {

	public string playSceneName = "battle_scene";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPlayButtonClick(){
		Application.LoadLevel (playSceneName);
	}

	public void OnSelectionButtonClick(){
		Application.LoadLevel ("music_select_menu");
	}

	public void OnLockerRoomClick(){
		Application.LoadLevel ("locker_room");
	}
}

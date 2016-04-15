using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SongEditorTestLauncher : MonoBehaviour {
	public string songName;
	public BattleEngine.Difficulty difficulty;
	public float timeBegin = 0;
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LaunchTest(string _songName, BattleEngine.Difficulty _difficulty){
		songName = _songName;
		difficulty = _difficulty;
		Application.LoadLevel ("test");
	}

}
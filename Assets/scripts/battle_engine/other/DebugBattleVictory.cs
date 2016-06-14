using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DebugBattleVictory : MonoBehaviour {

	// Use this for initialization
	void Start () {
        EndMatch();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void EndMatch()
    {
        SceneManager.LoadScene("battle_end");
    }
}

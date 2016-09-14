using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameMenuManager : MonoBehaviour {

    [SerializeField] GameMenuParty m_menuParty;


	// Use this for initialization
	void Start () {
        m_menuParty.ActivateMenu();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnButtonPressed()
    {

    }

    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("world1");
    }
}

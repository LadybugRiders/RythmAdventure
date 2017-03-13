using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameMenuManager : MonoBehaviour {

    [SerializeField] List<GameMenu> m_menus;
    [SerializeField] GameMenu m_startMenu;

    [SerializeField] GameObject m_backButton;

    GameMenu m_currentMenu;

    // Use this for initialization
    void Start () {
        m_currentMenu = m_startMenu ;
        m_currentMenu.ActivateMenu();
        foreach( var menu in m_menus)
        {
            if (menu != m_startMenu)
                menu.DeactivateMenu();
            
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnMenuButtonClicked(string _info)
    {
        int index = int.Parse(_info);
        var menuToActivate = m_menus[index];
        if (menuToActivate == m_currentMenu)
            return;
        m_currentMenu.DeactivateMenu();
        m_currentMenu = menuToActivate;
        menuToActivate.ActivateMenu();
    }

    public void OnBackButtonClicked()
    {
        m_currentMenu.OnBackButtonClicked();
    }

    public void OnMapButtonClicked()
    {
        SceneManager.LoadScene("world1");
    }

    public void ToggleBackButton(bool toggle)
    {
        m_backButton.SetActive(toggle);
    }

}

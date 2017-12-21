using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleBeginMenuManager : MonoBehaviour {

    [SerializeField] GameObject m_root;
    [SerializeField] List<CharacterInfosUI> m_charactersInfos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate()
    {
        m_root.SetActive(true);
        m_charactersInfos.ForEach(x => x.LoadData());
    }

    public void Deactivate()
    {
        m_root.SetActive(false);
    }
}

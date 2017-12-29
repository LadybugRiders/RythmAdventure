using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleBeginMenuManager : MonoBehaviour {

    [SerializeField] GameObject m_root;
    [SerializeField] List<CharacterInfosUI> m_charactersInfos;
    [SerializeField] List<EnemyInfosUI> m_enemiesInfos;

    [SerializeField] BattleBeginCharListUI m_charaList;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate(BattleDataAsset _battleDataAsset)
    {
        m_root.SetActive(true);
        m_charactersInfos.ForEach(x => x.LoadData());

        m_charaList.Load();
    }

    public void Deactivate()
    {
        m_root.SetActive(false);
    }

    public void OnBackButtonClicked()
    {
        Deactivate();
    }

    public void OnFightButtonClicked()
    {
        Component.FindObjectOfType<MapNodesManager>().OnBeginFight();
    }
}

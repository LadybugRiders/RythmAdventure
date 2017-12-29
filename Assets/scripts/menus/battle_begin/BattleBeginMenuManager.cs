using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleBeginMenuManager : MonoBehaviour {

    [SerializeField] GameObject m_root;
    [SerializeField] List<CharacterInfosUI> m_charactersInfos;
    [SerializeField] List<EnemyInfosUI> m_enemiesInfos;

    [SerializeField] BattleBeginCharListUI m_charaList;

    BattleDataAsset m_battleDataAsset;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate(BattleDataAsset _battleDataAsset)
    {
        m_battleDataAsset = _battleDataAsset;

        m_root.SetActive(true);
        m_charactersInfos.ForEach(x => x.LoadData());

        m_charaList.Load();
        LoadEnemies();
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

    void LoadEnemies()
    {
        for(int i = 0; i < m_battleDataAsset.Enemies.Count; i++)
        {
            var enemy = m_battleDataAsset.Enemies[i];
            m_enemiesInfos[i].Load(enemy.Id);
        }
    }
}

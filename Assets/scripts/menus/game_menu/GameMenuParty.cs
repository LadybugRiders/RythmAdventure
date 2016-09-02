using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMenuParty : GameMenu {

    [SerializeField] List<CharacterInfos> m_characters;

    DataCharManager m_charManager;
    ProfileManager.Profile m_profile;

	// Use this for initialization
	void Start () {
        m_profile = ProfileManager.instance.GetProfile();
        m_charManager = DataManager.instance.CharacterManager;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected override void Activate()
    {
        m_profile = ProfileManager.instance.GetProfile();
        m_charManager = DataManager.instance.CharacterManager;
        base.Activate();
        for(int i=0; i < m_profile.CurrentTeam.Count; i++)
        {
            string charId = m_profile.CurrentTeam[i];
            var charaInfo = m_characters[i];
            if( charId == null)
            {
                charaInfo.m_build.gameObject.SetActive(false);
                continue;
            }
            //get data stored in profile
            var charaData = ProfileManager.instance.GetCharacter(charId);
            //compute all around stats from the database
            var stats = m_charManager.ComputeStats(charaData);
            charaInfo.m_build.gameObject.SetActive(true);
            charaInfo.m_stats.Load(stats);
            //load appearance
            charaInfo.m_build.Load(charaData);
        }
    }

    protected override void Deactivate()
    {
        base.Deactivate();
    }

    [System.Serializable]
    public class CharacterInfos
    {
        public CharacterBuild m_build;
        public StatsFiller m_stats;
    }
}

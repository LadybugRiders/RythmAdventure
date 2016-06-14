using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleEndManager : MonoBehaviour {

    [SerializeField] List<ScoreInfo> m_scoresInfos;

    [SerializeField] Text m_totalXpText;

    [SerializeField] List<CharacterXpInfo> m_characters;

    DataCharManager m_charManager;

    List<StoredLevelUpStats> m_storedStats = new List<StoredLevelUpStats>();
    int m_totalXp = 0;

	// Use this for initialization
	void Start () {
        m_charManager = DataManager.instance.CharacterManager;

        m_totalXp = ComputeTotalXp();
        ApplyXp(m_totalXp);
        SetTotalXp(m_totalXp);
        //
        InitCharacters();

        LaunchXpAnimation();
	}
	
    /// <summary>
    /// For all enemies, add xp
    /// </summary>
	int ComputeTotalXp()
    {
        if (DataManager.instance.BattleData == null)
            return 0;
        var enemies = DataManager.instance.BattleData.Enemies;
        int totalXp = 0;
        for(int i=0; i < enemies.Count; ++i)
        {
            if( enemies[i] != null)
            {
                var stats = DataManager.instance.EnemiesManager.GetFullStats(enemies[i].Name, enemies[i].Level);
                totalXp += stats.XpNeeded;
            }
        }
        return totalXp;
    }

    void ApplyXp(int _xp)
    {
        foreach(var charaSave in ProfileManager.instance.GetCurrentTeam())
        {
            //old values for ui
            StoredLevelUpStats data = new StoredLevelUpStats();
            m_storedStats.Add(data);

            var levelupdata = m_charManager.GetNextStatsBonus(charaSave.Category, charaSave.Xp);
            data.oldXp = charaSave.Xp;
            data.oldLevel = levelupdata.Stats.Level;
            data.oldXpRequired = levelupdata.XpNeeded - charaSave.Xp;

            //apply xp on profile
            charaSave.Xp += _xp;

            //new values for ui
            var newLevelupdata = m_charManager.GetNextStatsBonus(charaSave.Category, charaSave.Xp);
            data.newXp = charaSave.Xp;
            data.newLevel = newLevelupdata.Stats.Level;
            data.newXpRequired = newLevelupdata.XpNeeded - charaSave.Xp;
        }
        ProfileManager.instance.SaveProfile();
    }

    void SetTotalXp(int _totalXp)
    {
        m_totalXpText.text = "" + _totalXp;
    }

    void InitCharacters()
    {
        var teamMates = ProfileManager.instance.GetCurrentTeam();
        for(int i = 0; i < m_characters.Count; ++i)
        {
            CharacterXpInfo chara = m_characters[i];
            var mate = teamMates[i];
            if( mate != null)
            {
                //Set xp before battle
                chara.text.text = "" + m_storedStats[i].oldXpRequired;
                chara.gauge.SetValue((float)m_storedStats[i].oldXp / m_storedStats[i].oldXpRequired);
            }
        }
    }

    void LaunchXpAnimation()
    {
        var teamMates = ProfileManager.instance.GetCurrentTeam();
        for (int i = 0; i < m_characters.Count; ++i)
        {
            CharacterXpInfo chara = m_characters[i];
            StoredLevelUpStats storedData = m_storedStats[i];

            chara.text.GetComponent<UITextNumberScroller>().ScrollTo(storedData.newXpRequired, 1f);
        }
    }

    void OnGoToMap()
    {
        string mapSceneName = PlayerPrefs.GetString("current_map_scene");
        SceneManager.LoadScene(mapSceneName);
    }

    [System.Serializable]
    class ScoreInfo
    {
        [SerializeField] public GameObject UIObject;
        [SerializeField] public BattleScoreManager.Accuracy Accuracy;
    }

    [System.Serializable]
    class CharacterXpInfo
    {
        [SerializeField] public GameObject gameObject;
        [SerializeField] public GameObject characterObject;
        [SerializeField] public Text text;
        [SerializeField] public UIGauge gauge;
    }

    [System.Serializable]
    class StoredLevelUpStats
    {
        public int oldLevel = 0;
        public int newLevel = 0;
        public int oldXp = 0;
        public int newXp = 0;

        public int oldXpRequired = 0;
        public int newXpRequired = 0;

        public bool HasLeveledUp
        {
            get
            {
                return oldLevel != newLevel;
            }
        }
    }
}

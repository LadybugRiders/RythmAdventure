using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleEndManager : MonoBehaviour {

    [SerializeField] List<ScoreInfo> m_scoresInfos;

    [SerializeField] Text m_totalXpText;
    [SerializeField] UITextNumberScroller m_totalScoreText;

    [SerializeField] List<CharacterXpInfo> m_characters;

    [SerializeField] GameObject m_mapButton;

	[SerializeField] float m_timeDisplayScore = 0.5f;
    enum State { IDLE, SCORE, XP };
    State m_state = State.IDLE;

    DataCharManager m_charManager;
    BattleScoreManager m_scoreManager;

    List<UIXpScrollerManager.StoredLevelUpStats> m_storedStats = new List<UIXpScrollerManager.StoredLevelUpStats>();
    int m_totalXp = 0;

    //UI Value
    float m_time = 0;

    int m_count = 0;
	[SerializeField] float m_scoreTimeByUnits = 0.05f;

	// Use this for initialization
	void Start () {
        m_mapButton.SetActive(false);
        m_charManager = DataManager.instance.CharacterManager;
        m_scoreManager = FindObjectOfType<BattleScoreManager>();

        InitCharacters();

        //Get Score and multipliers
        ApplyScore();
	}

    void Update()
    {
        switch (m_state)
        {
            case State.SCORE: UpdateScore();
                break;
        }
    }

    #region XP

    void ProcessXp()
    {
        m_state = State.XP;

        m_totalXp = ComputeTotalXp();
        ApplyXp(m_totalXp);
        SetTotalXp(m_totalXp);

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

    void InitCharacters()
    {
        var teamMates = ProfileManager.instance.GetCurrentTeam();
        for (int i = 0; i < m_characters.Count; ++i)
        {
            CharacterXpInfo chara = m_characters[i];
            var mate = teamMates[i];            
            var levelUpData = m_charManager.GetNextLevelByXp(mate.Category, mate.Xp);

            if (mate != null)
            {
                //Set xp before battle
                chara.text.text = "" + mate.Xp;
                float prog = 1.0f;
                if (levelUpData.XpNeeded != 0)
                    prog = (float)mate.Xp / levelUpData.XpNeeded;
                chara.gauge.SetValue(prog);
            }
        }
    }

    void ApplyXp(int _xp)
    {
        foreach(var charaSave in ProfileManager.instance.GetCurrentTeam())
        {
            //old values for ui
            UIXpScrollerManager.StoredLevelUpStats data = new UIXpScrollerManager.StoredLevelUpStats(charaSave.Id);
            m_storedStats.Add(data);

            var levelupdata = m_charManager.GetNextLevelByXp(charaSave.Category, charaSave.Xp);
            if( levelupdata == null)
            {
                data.isMaxLevel = true;
                continue;
            }
            data.oldXp = charaSave.Xp;
            data.oldLevel = levelupdata.Stats.Level-1;
            data.oldXpRequired = levelupdata.XpNeeded;

            //apply xp on profile
            charaSave.Xp += _xp;

            //new values for ui
            var newLevelupdata = m_charManager.GetNextLevelByXp(charaSave.Category, charaSave.Xp);
            data.newXp = charaSave.Xp;
            if (newLevelupdata != null)
            {
                data.newLevel = newLevelupdata.Stats.Level;
                data.newXpRequired = newLevelupdata.XpNeeded;
            }else
            {
                data.newLevel = levelupdata.Stats.Level;
                data.isMaxLevel = true;
            }
            data.Process();
        }
        ProfileManager.instance.SaveProfile();
    }

    void SetTotalXp(int _totalXp)
    {
        m_totalXpText.text = "" + _totalXp;
    }

    void LaunchXpAnimation()
    {
        for (int i = 0; i < m_characters.Count; ++i)
        {
            CharacterXpInfo chara = m_characters[i];
            UIXpScrollerManager.StoredLevelUpStats storedData = m_storedStats[i];
            chara.xpScroller.Scroll(storedData,3.0f,OnXpScrollerEnded);
        }
    }
    #endregion

    #region SCORE

    void ApplyScore()
    {
        if (m_scoreManager != null)
        {
			m_totalScoreText.ScrollTo(m_scoreManager.m_totalScore, m_timeDisplayScore);

            foreach (var scoreInfo in m_scoresInfos)
            {
                int count = m_scoreManager.m_notesCountByAcc[scoreInfo.Accuracy];
            }
            m_state = State.SCORE;
        }else
        {
            ProcessXp();
        }
    }

    void UpdateScore()
    {
        m_time += Time.deltaTime;
        if(m_time >= m_scoreTimeByUnits)
        {
            m_time = 0;
            bool end = true;
            m_count++;
            foreach (var scoreInfo in m_scoresInfos)
            {
                int count = m_scoreManager.m_notesCountByAcc[scoreInfo.Accuracy];
                if( m_count <= count)
                {
                    end = false;
                    scoreInfo.ScoreText.text = m_count.ToString();
                }
            }
            if (end)
            {
                ProcessXp();
            }
        }
    }

    void OnXpScrollerEnded(UIXpScrollerManager _scroller)
    {
        m_mapButton.SetActive(true);
    }

    #endregion

    void OnGoToMap()
    {
        string mapSceneName = PlayerPrefs.GetString("current_map_scene");
        SceneManager.LoadScene(mapSceneName);
    }

    [System.Serializable]
    class ScoreInfo
    {
        [SerializeField] public GameObject UIObject;
        [SerializeField] public Text ScoreText;
        [SerializeField] public BattleScoreManager.Accuracy Accuracy;
    }

    [System.Serializable]
    class CharacterXpInfo
    {
        [SerializeField] public GameObject gameObject;
        [SerializeField] public GameObject characterObject;
        [SerializeField] public Text text;
        [SerializeField] public UIGauge gauge;
        [SerializeField] public UIXpScrollerManager xpScroller;
    }
        
}

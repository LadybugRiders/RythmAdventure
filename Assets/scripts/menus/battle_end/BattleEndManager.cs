﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class BattleEndManager : MonoBehaviour {

    [SerializeField] UIBattleEndScoreScrollSequence m_scoreSequence;

    [SerializeField] List<ScoreInfo> m_scoresInfos;

    [SerializeField] Text m_totalXpText;
    [SerializeField] UITextNumberScroller m_totalScoreText;

    [SerializeField] List<BattleEndCharInfoUI> m_characters;

    [SerializeField] GameObject m_mapButton;

	[SerializeField] float m_timeDisplayScore = 0.5f;
    enum State { IDLE, SCORE, XP };
    State m_state = State.IDLE;

    [SerializeField]  float m_uiCharaScale = 40;

    BattleData m_battleData;

    DataCharManager m_charManager;

    List<UIXpScrollerManager.StoredLevelUpStats> m_storedStats = new List<UIXpScrollerManager.StoredLevelUpStats>();
    
    //UI Value
    float m_time = 0;

    int m_count = 0;
	[SerializeField] float m_scoreTimeByUnits = 0.05f;

	// Use this for initialization
	void Start () {
        //m_mapButton.SetActive(false);
        m_charManager = DataManager.instance.CharacterManager;
        m_battleData = ProfileManager.instance.BattleData;

        //Debug
        if (m_battleData.Characters.Count == 0)
        {
            DebugFillBattleData();
        }

        InitCharacters();

        //Get Score and multipliers
        //ApplyScore();
        m_scoreSequence.Launch(OnAccuraciesScrollingEnd, m_battleData.NotesCountByAccuracy, 100); 
	}

    void Update()
    {
    }

    #region XP

    void ProcessXp()
    {
        m_state = State.XP;
        
        ApplyXp();
        SetTotalXp(m_battleData.TotalXp);

        //LaunchXpAnimation();
    }
    
    void InitCharacters()
    {
        var teamMates = ProfileManager.instance.GetCurrentTeam();
        for (int i = 0; i < m_characters.Count; ++i)
        {
            var chara = m_characters[i];
            var mate = teamMates[i];            
            var levelUpData = m_charManager.GetNextLevelByXp(mate.Job, mate.Xp);
            var battleCharData = m_battleData.Characters[i];

            if (mate != null)
            {
                //Set xp before battle
                chara.XpText.text = "" + battleCharData.XpStart;
                float prog = 1.0f;
                if (levelUpData.XpNeeded != 0)
                    prog = (float)battleCharData.XpStart / levelUpData.XpNeeded;
                if( chara.Gauge != null )
                    chara.SetGaugeValue(prog);
            }

            //create UI characters
            var go = GameUtils.CreateCharacterUIObject(mate, m_uiCharaScale);
            go.transform.SetParent(chara.CharacterObject.transform, false);
        }
    }

    void ApplyXp()
    {
        var teamMates = ProfileManager.instance.GetCurrentTeam();
        for (int i = 0; i < m_characters.Count; ++i)
        {
            //get ui objects
            var charaUI = m_characters[i];
            //get profile data for the character
            var charProfileData = teamMates[i];
            //Get battle data for the character
            var charBattleData = m_battleData.GetCharacter(charProfileData.Id);
            //old values for ui
            UIXpScrollerManager.StoredLevelUpStats data = new UIXpScrollerManager.StoredLevelUpStats(charProfileData.Id);
            m_storedStats.Add(data);

            var levelupdata = m_charManager.GetNextLevelByXp(charProfileData.Job, charProfileData.Xp);
            if( levelupdata == null)
            {
                data.isMaxLevel = true;
                continue;
            }
            data.oldXp = charBattleData.XpStart;
            data.oldLevel = 1;
            data.oldXpRequired = levelupdata.XpNeeded;
            
            //new values for ui
            var newLevelupdata = m_charManager.GetNextLevelByXp(charProfileData.Job, charProfileData.Xp);
            data.newXp = charBattleData.XpGained;
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
    }

    void SetTotalXp(int _totalXp)
    {
        m_totalXpText.text = "" + _totalXp;
    }

    void LaunchXpAnimation()
    {
        for (int i = 0; i < m_characters.Count; ++i)
        {
            var chara = m_characters[i];
            UIXpScrollerManager.StoredLevelUpStats storedData = m_storedStats[i];
            chara.XpScroller.Scroll(storedData,3.0f,OnXpScrollerEnded);
        }
    }
    #endregion

    #region SCORE

    void ApplyScore()
    {
	    m_totalScoreText.ScrollFromTo(0, m_battleData.TotalScore, m_timeDisplayScore);

        foreach(var scoreInfo in m_scoresInfos)
        {
            int count = m_battleData.NotesCountByAccuracy[scoreInfo.Accuracy];
            scoreInfo.Scroller.ScrollFromTo(0,count, m_timeDisplayScore);
        }
        TimerEngine.instance.AddTimer(m_timeDisplayScore + 0.5f, "OnScoreTargetReached", this.gameObject);
        m_state = State.SCORE;
    }

    void OnScoreTargetReached()
    {
        m_state = State.XP;
        ProcessXp();
    }

    void OnXpScrollerEnded(UIXpScrollerManager _scroller)
    {
        m_mapButton.SetActive(true);
    }

    public void OnAccuraciesScrollingEnd(UISequence sequence)
    {
        Debug.Log("SCrOLL END FOR ACCUREACIES");
    }

    #endregion

    void OnGoToMap()
    {
        string mapSceneName = PlayerPrefs.GetString("current_map_scene");
        SceneManager.LoadScene(mapSceneName);
    }

    /// <summary>
    /// Used only in debug when launching the scene directly
    /// </summary>
    void DebugFillBattleData()
    {
        m_battleData = new BattleData();
        foreach (var chara in ProfileManager.instance.GetCurrentTeam())
        {
            m_battleData.AddPlayerData(chara.Id, 2, 50);
        }
        m_battleData.TotalXp = 100;

        m_battleData.NotesCountByAccuracy = new Dictionary<HitAccuracy, int>();
        int totalNotes = 0;
        for(int i=0; i < Utils.EnumCount(HitAccuracy.GOOD); ++i)
        {
            HitAccuracy acc = (HitAccuracy)i;
            int count = Random.Range(0, 20);
            m_battleData.NotesCountByAccuracy[acc] = count;
            totalNotes += count;
        }
        m_battleData.NotesCount = totalNotes;
        m_battleData.TotalScore = Random.Range(0, 1000);
    }

    [System.Serializable]
    class ScoreInfo
    {
        [SerializeField] public GameObject UIObject;
        [SerializeField] public Text ScoreText;
        [SerializeField] public UITextNumberScroller Scroller;
        [SerializeField] public HitAccuracy Accuracy;
    }

    [System.Serializable]
    class CharacterXpInfo
    {
        [SerializeField] public GameObject gameObject;
        [SerializeField] public GameObject characterObject;
        [SerializeField] public Text text;
        [SerializeField] public SpriteGauge gauge;
        [SerializeField] public UIXpScrollerManager xpScroller;
    }
        
}

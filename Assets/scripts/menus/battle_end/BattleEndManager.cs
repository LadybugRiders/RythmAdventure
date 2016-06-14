using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleEndManager : MonoBehaviour {

    [SerializeField] List<ScoreInfo> m_scoresInfos;

    [SerializeField] Text m_totalXpText;

    [SerializeField] List<CharacterXpInfo> m_characters;

    List<int> m_oldXp = new List<int>();
    int m_totalXp = 0;

	// Use this for initialization
	void Start () {
        m_totalXp = ComputeTotalXp();
        ApplyXp(m_totalXp);
        SetTotalXp(m_totalXp);
        //
        InitCharacters();
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
            //keep xp before level up ( for ui )
            m_oldXp.Add(charaSave.Xp);
            //apply xp on profile
            charaSave.Xp += _xp;
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
                var NextLvlUp = DataManager.instance.CharacterManager.GetNextStatsBonus(mate.Category, m_oldXp[i]);
                if (NextLvlUp != null)
                {
                    chara.text.text = "" + (NextLvlUp.XpNeeded - m_oldXp[i]);
                    chara.gauge.SetValue((float)m_oldXp[i] / NextLvlUp.XpNeeded);
                }
            }
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
}

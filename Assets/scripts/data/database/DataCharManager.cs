using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataCharManager : DatabaseLoader
{    
    protected override void LoadDatabase()
    {
        base.LoadDatabase();

        JSONObject tempJson;
        tempJson = LoadDataJSON("characters/characters_levelup_database");
        m_database.Add("levelup", tempJson);        
    }
    
    public LevelUpData GetStatsBonus(string _category, int _xp)
    {
        JSONObject levelupDB = m_database["levelup"];
        JSONObject levelsForPlayerJSON = levelupDB[_category];

        //Get base stats from leveling (db)
        LevelUpData levelUpData = null;
        for (int i = 0; i < levelsForPlayerJSON.Count; ++i)
        {
            var lvlUp = levelsForPlayerJSON[i];
            int xpNeeded = (int)lvlUp.GetField("xp").f;
            if ( xpNeeded <= _xp )
            {
                levelUpData = new LevelUpData(lvlUp);
            }
        }
        return levelUpData;
    }

    /// <summary>
    /// Returns the level data of the next level
    /// </summary>
    public LevelUpData GetNextStatsBonus(string _category, int _xp)
    {
        JSONObject levelupDB = m_database["levelup"];
        JSONObject levelsForPlayerJSON = levelupDB[_category];

        //Get base stats from leveling (db)
        LevelUpData levelUpData = null;
        for (int i = 0; i < levelsForPlayerJSON.Count; ++i)
        {
            var lvlUp = levelsForPlayerJSON[i];
            int xpNeeded = (int)lvlUp.GetField("xp").f;
            if (xpNeeded > _xp)
            {
                levelUpData = new LevelUpData(lvlUp);
                break;
            }
        }
        return levelUpData;
    }

    #region LEVELUP

    #endregion

    /// <summary>
    /// Data used to stored levels 
    /// </summary>
    public class LevelUpData
    {
        public int XpNeeded = 0;
        public Stats Stats = new Stats();

        public LevelUpData(JSONObject json)
        {
            XpNeeded = (int)json.GetField("xp").f;
            Stats = new Stats(json);
        }
    }
}

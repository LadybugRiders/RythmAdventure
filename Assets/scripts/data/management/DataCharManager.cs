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

    public LevelUpData GetFullStats(string playerId)
    {
        JSONObject levelupDB = m_database["levelup"];
        JSONObject levelsForPlayerJSON = levelupDB[playerId];

        //Get base stats from leveling (db)
        LevelUpData levelUpData = null;
        for (int i = 0; i < levelsForPlayerJSON.Count; ++i)
        {
            var lvlUp = levelsForPlayerJSON[i];
            levelUpData = new LevelUpData(lvlUp);
            break;
        }
        return levelUpData;
    }

    public class LevelUpData
    {
        public string PlayerId;
        public int XpNeeded = 0;
        public Stats Stats = new Stats();

        public LevelUpData(JSONObject json)
        {
            XpNeeded = (int)json.GetField("xp").f;
            //stats
            Stats = new Stats();
            Stats.Level = (int)json.GetField("level").f;
            Stats.Attack = (int)json.GetField("attack").f;
            Stats.Defense = (int)json.GetField("defense").f;
            Stats.Magic = (int)json.GetField("magic").f;
        }
    }
}

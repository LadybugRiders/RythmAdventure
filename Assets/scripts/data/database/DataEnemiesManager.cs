using UnityEngine;
using System.Collections;

public class DataEnemiesManager : DatabaseLoader {

    protected override void LoadDatabase()
    {
        base.LoadDatabase();

        JSONObject tempJson;
        tempJson = LoadDataJSON("enemies/enemies_stats");
        m_database.Add("stats", tempJson);
    }

    public DataCharManager.LevelUpData GetFullStats(string _enemyId, int _level)
    {
        JSONObject statsDB = m_database["stats"];
        JSONObject statsForEnemiesJSON = statsDB[_enemyId];

        //Get base stats from leveling (db)
        DataCharManager.LevelUpData levelUpData = null;
        for (int i = 0; i < statsForEnemiesJSON.Count; ++i)
        {
            var lvlUp = statsForEnemiesJSON[i];
            if( lvlUp.GetField("levelmax").f > _level && lvlUp.GetField("level").f < _level)
            {
                levelUpData = new DataCharManager.LevelUpData(lvlUp);
                break;
            }
        }
        return levelUpData;
    }
}

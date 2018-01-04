using UnityEngine;
using System.Collections;

public class DataEnemiesManager : DatabaseLoader {

    EnemiesDataCollection m_enemies;

    protected override void LoadDatabase()
    {
        base.LoadDatabase();

        JSONObject tempJson;
        tempJson = LoadDataJSON("enemies/enemies_stats");
        m_enemies = JSONLoaderLR.LoadTable<EnemiesDataCollection>(tempJson["list"]);
    }

    public Stats GetFullStats(string _enemyId)
    {
        var enemy = m_enemies[_enemyId];
        return enemy.Stats;
    }

    public EnemyData GetEnemy(string _enemyId)
    {
        return m_enemies[_enemyId];
    }

    public class EnemyData : JSONData
    {
        public string Name { get; set; }
        public string Prefab { get; set; }
        public string Description { get; set; }

        public string Weakness { get; set; }
        public string Element { get; set; }

        public Stats Stats;

        public int Xp { get; set; }

        public EnemyData() { }
        public EnemyData(JSONObject _json) : base(_json) { }

        public override void BuildJSONData(JSONObject _json)
        {
            base.BuildJSONData(_json);
            Stats = new Stats(_json);
        }
    }

    public class EnemiesDataCollection : IJSONDataDicoCollection<EnemyData> { }
}

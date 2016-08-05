using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataCharManager : DatabaseLoader
{    
    protected override void LoadDatabase()
    {
        base.LoadDatabase();

        JSONObject tempJson;
        //levelup
        tempJson = LoadDataJSON("characters/characters_levelup_database");
        m_database.Add("levelup", tempJson);
        //equipement
        tempJson = LoadDataJSON("characters/equipement_database");
        m_database.Add("equipment", tempJson);
    }

    #region LEVEL

    public LevelUpData GetLevelByXp(string _category, int _xp)
    {
        JSONObject levelupDB = LevelUpDatabase;
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

    public LevelUpData GetLevel(string _category, int _level)
    {
        JSONObject levelupDB = LevelUpDatabase;
        JSONObject levelsForPlayerJSON = levelupDB[_category];

        //Get base stats from leveling (db)
        LevelUpData levelUpData = null;
        for (int i = 0; i < levelsForPlayerJSON.Count; ++i)
        {
            var lvlUp = levelsForPlayerJSON[i];
            int xpNeeded = (int)lvlUp.GetField("level").f;
            if (xpNeeded <= _level)
            {
                levelUpData = new LevelUpData(lvlUp);
            }
        }
        return levelUpData;
    }

    /// <summary>
    /// Returns the level data of the next level
    /// </summary>
    public LevelUpData GetNextLevelByXp(string _category, int _xp)
    {
        JSONObject levelupDB = LevelUpDatabase;
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

    /// <summary>
    /// Returns the level data of the next level
    /// </summary>
    public LevelUpData GetNextLevel(string _category, int _level)
    {
        JSONObject levelupDB = LevelUpDatabase;
        JSONObject levelsForPlayerJSON = levelupDB[_category];

        //Get base stats from leveling (db)
        LevelUpData levelUpData = null;
        for (int i = 0; i < levelsForPlayerJSON.Count; ++i)
        {
            var lvlUp = levelsForPlayerJSON[i];
            int xpNeeded = (int)lvlUp.GetField("level").f;
            if (xpNeeded > _level)
            {
                levelUpData = new LevelUpData(lvlUp);
                break;
            }
        }
        return levelUpData;
    }
    #endregion

    #region EQUIPEMENT

    public EquipementData GetEquipement( EquipmentType _type, string _id)
    {
        JSONObject database = EquipementDatabase[_type.ToString().ToLower()];
        var jsonObject = database.list.Find(x => x.GetField("id").ToString() == _id);

        if (jsonObject != null)
        {
            return new EquipementData(jsonObject, _type);
        }
        return null;
    }

    public EquipementData GetEquipement(string _type, string _id)
    {
        EquipmentType enumType = (EquipmentType) System.Enum.Parse(typeof(EquipmentType), _type);
        return GetEquipement(enumType, _id);
    }
    
    #endregion

    JSONObject LevelUpDatabase { get { return m_database["levelup"]; } }
    JSONObject EquipementDatabase { get { return m_database["equipment"]; } }

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

    public class EquipementData
    {
        public string Id;
        public string Name = "NoName_Equipment";
        public string Prefab;
        public List<EquipCompatibility> Compatibilities = new List<EquipCompatibility>();
        public int Level = 1;
        public EquipmentType type;

        public EquipementData(JSONObject _json, EquipmentType _type)
        {
            Id = _json.GetField("id").str;
            Level = (int)_json.GetField("level").f;
            Name = _json.GetField("name").str;
            Prefab = _json.GetField("prefab").str;

            //Compat
            Compatibilities.Add( (EquipCompatibility)System.Enum.Parse(typeof(EquipCompatibility), _json.GetField("compat").str.ToUpper())) ;
            var compat2 = _json.GetField("compat2");
            if( compat2 != null)
            {
                Compatibilities.Add((EquipCompatibility)System.Enum.Parse(typeof(EquipCompatibility), compat2.str.ToUpper()));
            }

            type = _type;
        }

        public bool IsCompatible(EquipCompatibility _type)
        {
            foreach (var comp in Compatibilities)
                if (comp == _type)
                    return true;
            return false;
        }
    }
}

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

    public LevelUpData GetLevelByXp(Job _job, int _xp)
    {
        JSONObject levelupDB = LevelUpDatabase;
        JSONObject levelsForPlayerJSON = levelupDB[_job.ToString().ToLower()];

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

    public LevelUpData GetLevel(Job _job, int _level)
    {
        JSONObject levelupDB = LevelUpDatabase;
        JSONObject levelsForPlayerJSON = levelupDB[_job.ToString().ToLower()];

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
    public LevelUpData GetNextLevelByXp(Job _job, int _xp)
    {
        JSONObject levelupDB = LevelUpDatabase;
        JSONObject levelsForPlayerJSON = levelupDB[_job.ToString().ToLower()];

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

    public EquipmentData GetEquipement( EquipmentType _type, string _id)
    {
        JSONObject database = EquipementDatabase[_type.ToString().ToLower()];
        var jsonObject = database.list.Find(x => x.GetField("id").ToString() == _id);

        if (jsonObject != null)
        {
            return new EquipmentData(jsonObject, _type);
        }
        return null;
    }

    public EquipmentData GetEquipement(string _type, string _id)
    {
        EquipmentType enumType = (EquipmentType) System.Enum.Parse(typeof(EquipmentType), _type);
        return GetEquipement(enumType, _id);
    }

    public LooksData GetLooks( LooksType _type, string _id)
    {
        JSONObject database = EquipementDatabase[_type.ToString().ToLower()];
        var jsonObject = database.list.Find(x => x.GetField("id").ToString() == _id);

        if (jsonObject != null)
        {
            return new LooksData(jsonObject, _type);
        }
        return null;
    }

    public LooksData GetLooks(string _type, string _id)
    {
        LooksType lookType = (LooksType)System.Enum.Parse(typeof(LooksType), _type);
        return GetLooks(lookType, _id);
    }

    #endregion

    #region COLOR

    public Color GetBodyColor(string _colorId)
    {
        Color color = new Color();
        color.a = 1.0f;
        JSONObject colorDB = EquipementDatabase["body_colors"];
        var jsonObject = colorDB.list.Find(x => x.GetField("id").ToString() == _colorId);
        if( jsonObject != null)
        {
            color.r = jsonObject.GetField("red").f / 255;
            color.g = jsonObject.GetField("green").f / 255;
            color.b = jsonObject.GetField("blue").f / 255;
        }
        return color;
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

    public class BuildData
    {
        public string Id;
        public string Name = "NoName_Equipment";
        public string Prefab;
        public List<EquipCompatibility> Compatibilities = new List<EquipCompatibility>();
        public int Level = 1;

        public BuildData(JSONObject _json)
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
        }

        public bool IsCompatible(EquipCompatibility _type)
        {
            foreach (var comp in Compatibilities)
                if (comp == _type)
                    return true;
            return false;
        }
    }

    public class EquipmentData : BuildData
    {
        public EquipmentType type;

        public EquipmentData(JSONObject _json, EquipmentType _type) : base(_json)
        {
            type = _type;
        }
    }

    public class LooksData : BuildData
    {
        public LooksType type;

        public LooksData(JSONObject _json, LooksType _type) : base(_json)
        {
            type = _type;
        }
    }
}

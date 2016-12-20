using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataCharManager : DatabaseLoader
{
    Dictionary<Job, LevelUpDataCollection> m_levelupsDB = new Dictionary<Job, LevelUpDataCollection>();
    Dictionary<EquipmentType, BuildDataCollection> m_equipmentsDB = new Dictionary<EquipmentType, BuildDataCollection>();
    Dictionary<LooksType, BuildDataCollection> m_looksDB = new Dictionary<LooksType, BuildDataCollection>();
    ColorDataCollection m_colorsDB;

    protected override void LoadDatabase()
    {
        base.LoadDatabase();

        JSONObject tempJson;
        //levelup
        //read json from database
        tempJson = LoadDataJSON("characters/characters_levelup_database");
        //foreach enum value
        for( int i=0; i< Utils.EnumCount(Job.THIEF); i++)
        {
            Job job = (Job)i;
            string jobStr = job.ToString().ToLower();
            //Convert json into classes
            var coll = JSONLoaderLR.LoadTable<LevelUpDataCollection>(tempJson[jobStr]);
            m_levelupsDB[job] = coll;
        }
        //equipement
        tempJson = LoadDataJSON("characters/equipement_database");
        for (int i = 0; i < Utils.EnumCount(EquipmentType.ACCESSORY); i++)
        {
            EquipmentType equEnum = (EquipmentType)i;
            string equStr = equEnum.ToString().ToLower();
            var coll = JSONLoaderLR.LoadTable<BuildDataCollection>(tempJson[equStr]);
            m_equipmentsDB[equEnum] = coll;
        }
        //looks
        for (int i = 0; i < Utils.EnumCount(LooksType.EYEBROWS); i++)
        {
            LooksType lookEnum = (LooksType)i;
            string lookStr = lookEnum.ToString().ToLower();
            var coll = JSONLoaderLR.LoadTable<BuildDataCollection>(tempJson[lookStr]);
            m_looksDB[lookEnum] = coll;
        }
        var ld = tempJson["body_colors"];
        m_colorsDB = JSONLoaderLR.LoadTable<ColorDataCollection>(tempJson["body_colors"]);
        //generation
        tempJson = LoadDataJSON("characters/mixi_generation_database");
        m_database.Add("generation", tempJson);
        
    }

    public Stats ComputeStats(ProfileManager.CharacterData _charData)
    {
        Stats stats = new Stats();
        //Get Class Stats
        var levelupStats = GetLevelByXp(_charData.Job, _charData.Xp).Stats;
        stats.Add(levelupStats);
        return stats;
    }

    #region LEVEL

    public LevelUpData GetLevelByXp(Job _job, int _xp)
    {
        var lvls = m_levelupsDB[_job];
        if( lvls != null)
        {
            return lvls.GetByXp(_xp);
        }
        return null;
    }

    public LevelUpData GetLevel(Job _job, int _level)
    {
        var lvls = m_levelupsDB[_job];
        if (lvls != null)
        {
            return lvls[_level];
        }
        return null;
    }

    /// <summary>
    /// Returns the level data of the next level
    /// </summary>
    public LevelUpData GetNextLevelByXp(Job _job, int _xp)
    {
        var lvls = m_levelupsDB[_job];
        if (lvls != null)
        {
            return lvls.GetNextLevelByXp(_xp);
        }
        return null;
    }

    /// <summary>
    /// Returns the level data of the next level
    /// </summary>
    public LevelUpData GetNextLevel(Job _job, int _level)
    {
        var lvls = m_levelupsDB[_job];
        if (lvls != null)
        {
            return lvls[_level+1];
        }
        return null;
    }
    #endregion

    #region EQUIPEMENT
    
    public List<BuildData> GetEquipements(EquipmentType _type, Job _job, int _tiers = -1) 
    {
        var equipments = m_equipmentsDB[_type];
        return equipments.GetCompatibleBuilds(EquipCompatibility.ALL, _tiers);
    }
        
    public BuildData GetEquipement( EquipmentType _type, string _id)
    {
        var equipments = m_equipmentsDB[_type];
        return equipments[_id];
    }

    public BuildData GetEquipement(string _type, string _id)
    {
        EquipmentType enumType = (EquipmentType) System.Enum.Parse(typeof(EquipmentType), _type);
        return GetEquipement(enumType, _id);
    }

    public List<BuildData> GetLooks(LooksType _type, Job _job, int _tiers = -1)
    {
        var equipments = m_looksDB[_type];
        return equipments.GetCompatibleBuilds(EquipCompatibility.ALL, _tiers);
    }

    public BuildData GetLook( LooksType _type, string _id)
    {
        var looks = m_looksDB[_type];
        return looks[_id];
    }

    public BuildData GetLook(string _type, string _id)
    {
        LooksType lookType = (LooksType)System.Enum.Parse(typeof(LooksType), _type);
        return GetLook(lookType, _id);
    }

    #endregion

    #region COLOR

    public Color GetColor(string _colorId)
    {
        return m_colorsDB[_colorId].Color;
    }
    
    public List<ColorData> GetColors(int _tiers)
    {
        return GameUtils.SearchByTiers<ColorData>(m_colorsDB, _tiers);
    }

    #endregion

    #region SKILLS

    /*public List<SkillGenerationData> GetSkills(Job _job, int _tiers)
    {
        string id = _job.ToString().ToLower() + "_" + "skills";
        var json = GenerationDatabase[id];
    }

    List<SkillGenerationData> GetSkills(JSONObject _skills, int _tiers)
    {

    }*/

    #endregion
        
    public JSONObject GenerationDatabase { get { return m_database["generation"]; } }


    #region LEVEL_DATA
    /// <summary>
    /// Data used to stored levels 
    /// </summary>
    public class LevelUpData
    {
        public int XpNeeded = 0;
        public Stats Stats = new Stats();

        public LevelUpData(JSONObject _json)
        {
            XpNeeded = (int)_json.GetField("xp").f;
            Stats = new Stats(_json);
        }
    }

    public class LevelUpDataCollection : IJSONDataCollection
    {
        Dictionary<int, LevelUpData> levelups = new Dictionary<int, LevelUpData>();

        public void AddElement(JSONObject _element)
        {
            LevelUpData data = new LevelUpData(_element);
            levelups.Add(data.Stats.Level, data);
        }

        public IEnumerator<LevelUpData> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public LevelUpData GetByXp(int _xp)
        {
            LevelUpData level = null;
            foreach(var lvl in levelups)
            {
                if (lvl.Value.XpNeeded > _xp)
                    return level;
                level = lvl.Value;
            }
            return level;
        }

        public LevelUpData GetNextLevelByXp(int _xp)
        {
            foreach (var lvl in levelups)
            {
                if (lvl.Value.XpNeeded > _xp)
                    return lvl.Value;
            }
            return null;
        }

        public LevelUpData this[int i]
        {
            get
            {
                if (levelups.ContainsKey(i))
                    return levelups[i];
                return null;
            }
        }
    }

    #endregion

    #region BUILD_DATA

    public class BuildData : GameUtils.WeightableData
    {
        public string Id;
        public string Name = "NoName_Equipment";
        public string Prefab;
        public List<EquipCompatibility> Compatibilities = new List<EquipCompatibility>();
        
        public Stats Stats = new Stats();

        public BuildData(JSONObject _json) : base(_json)
        {
            Id = _json.GetField("id").str;
            Name = _json.GetField("name").str;
            Prefab = _json.GetField("prefab").str;
            
            //Compat
            Compatibilities.Add( (EquipCompatibility)System.Enum.Parse(typeof(EquipCompatibility), _json.GetField("compat").str.ToUpper())) ;
            var compat2 = _json.GetField("compat2");
            if( compat2 != null)
            {
                Compatibilities.Add((EquipCompatibility)System.Enum.Parse(typeof(EquipCompatibility), compat2.str.ToUpper()));
            }

            Stats = new Stats(_json);
        }

        public bool IsCompatible(EquipCompatibility _type)
        {
            if (_type == EquipCompatibility.ALL)
                return true;
            foreach (var comp in Compatibilities)
                if (comp == EquipCompatibility.ALL || comp == _type)
                    return true;
            return false;
        }
    }
       

    public class BuildDataCollection : IJSONDataCollection
    {
        Dictionary<string, BuildData> builds = new Dictionary<string, BuildData>();
        
        public void AddElement(JSONObject _element)
        {
            BuildData data = new BuildData(_element);
            builds.Add(data.Id, data);
        }


        public IEnumerator<BuildData> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public List<BuildData> GetCompatibleBuilds(EquipCompatibility _compat,int _tiers)
        {
            var list = new List<BuildData>();
            foreach (var b in builds.Values)
            {
                if (b.IsCompatible(_compat) && ( b.Tiers <= _tiers || _tiers < 0 ))
                {
                    list.Add(b);
                }
            }
            return list;
        }
        
        public BuildData this[string i]
        {
            get
            {
                if (builds.ContainsKey(i))
                    return builds[i];
                return null;
            }
        }
    }


    public class SkillGenerationData : GameUtils.WeightableData
    {
        public string Id;
        public Job Job;
        public string SkillId;

        public SkillGenerationData(JSONObject _json, Job _job) : base(_json)
        {
            Job = _job;
            Id = _json.GetField("id").str;
            SkillId = _json.GetField("skill_id").str;
        }
    }

    #endregion

    #region COLORS_DATA

    public class ColorData : GameUtils.WeightableData
    {
        public string Id;
        public string Name;
        public Color Color;

        public ColorData(JSONObject _json) : base(_json)
        {
            Id = _json.GetField("id").ToString();
            Name = _json.GetField("name").str;
            
            Color.r = _json.GetField("red").f / 255;
            Color.g = _json.GetField("green").f / 255;
            Color.b = _json.GetField("blue").f / 255;
            Color.a = 1.0f;
        }
    }

    public class ColorDataCollection : IJSONDataCollection
    {
        Dictionary<string, ColorData> builds = new Dictionary<string, ColorData>();

        public void AddElement(JSONObject _element)
        {
            ColorData data = new ColorData(_element);
            builds.Add(data.Id, data);
        }

        public IEnumerator<ColorData> GetEnumerator()
        {
            return builds.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return builds.Values.GetEnumerator();
        }
        
        public ColorData this[string i]
        {
            get
            {
                if (builds.ContainsKey(i))
                    return builds[i];
                return null;
            }
        }
    }

    #endregion
}

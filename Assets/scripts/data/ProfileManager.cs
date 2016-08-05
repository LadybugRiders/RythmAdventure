using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProfileManager : MonoBehaviour {

    private static ProfileManager _instance;

    public Profile profile;

    [SerializeField] bool m_resetPrefsAtLaunch = false;
    
    public static ProfileManager instance{
        get{
            if(_instance == null)
            {
                GameObject go = Instantiate(Resources.Load("prefabs/Profile") as GameObject);
                _instance = go.GetComponent<ProfileManager>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;

        if( m_resetPrefsAtLaunch)
            PlayerPrefs.DeleteAll();

        LoadProfile();
        DontDestroyOnLoad(this.gameObject);
    }

    #region PROFILE_SAVE_LOAD
    public void ResetProfile()
    {
        LoadDefaultProfile();
    }

    void LoadProfile()
    {
        string json = PlayerPrefs.GetString("profile");
        if (string.IsNullOrEmpty(json))
        {
            //Load profile by default
            LoadDefaultProfile();
            return;
        }
        //PArse JSON
        profile = JsonUtility.FromJson<Profile>(json);
    }

    public void SaveProfile()
    {
        string json = JsonUtility.ToJson(profile);
        PlayerPrefs.SetString("profile", json);
        Debug.Log("[Saved Profile] " + json);
    }

#if UNITY_EDITOR
    public void SaveDefaultProfile()
    {
        string path = Application.dataPath + "/Resources/database/defaultProfile.json";
        string text = JsonUtility.ToJson(profile);

        System.IO.File.WriteAllText(path, text);
        AssetDatabase.Refresh();
    }
#endif

    void LoadDefaultProfile()
    {
        TextAsset json = Resources.Load("database/defaultProfile") as TextAsset;
        profile = JsonUtility.FromJson(json.text, typeof(Profile)) as Profile;
        Resources.UnloadAsset(json);
    }

    #endregion

    public Profile GetProfile()
    {
        return profile;
    }

    #region CHARACTERS

    public CharacterData GetCharacter(string _id)
    {
        foreach (var chara in profile.Characters)
        {
            if (chara.Id == _id)
                return chara;
        }
        return null;
    }

    public Stats GetCharacterStats(string _id)
    {
        var chara = GetCharacter(_id);
        var levelupdata = DataManager.instance.CharacterManager.GetLevelByXp(chara.Job, chara.Xp);
        return levelupdata!=null ? levelupdata.Stats : null;
    }

    public void AddCharacterXp(string _id, int _xp)
    {
        var charaSave = GetCharacter(_id);
        if( charaSave != null)
        {
            charaSave.Xp += _xp;
        }
    }

    public List<CharacterData> GetCurrentTeam()
    {
        List<CharacterData> chars = new List<CharacterData>();
        foreach (string teamMateName in profile.CurrentTeam)
        {
            var charaSave = GetCharacter(teamMateName);
            if (charaSave != null)
                chars.Add(charaSave);
        }
        return chars;
    }

    #endregion

    #region MAPS

    public Map GetMapData(string _mapName)
    {
        foreach( var map in profile.Maps)
        {
            if( map.Name.ToUpper() == _mapName.ToUpper() )
            {
                return map;
            }
        }
        return null;
    }

    #endregion

    [System.Serializable]
    public class Profile
    {
        [SerializeField] public string PlayerName = "player";
        [SerializeField] public int Xp = 0;
        [SerializeField] public bool Initialized = false;

        [SerializeField] public List<CharacterData> Characters = new List<CharacterData>();

        [SerializeField] public List<string> CurrentTeam;

        //Progression
        [SerializeField] public List<Map> Maps = new List<Map>();

        public Profile()
        {
            Characters = new List<CharacterData>()
            {
                new CharacterData("player1"),
                new CharacterData("player2"),
                new CharacterData("player3")
            };

            CurrentTeam = new List<string>() { "player1", "player3", "player1" };
        }

    }

    #region PROFILE_DATA_CLASSES

    [System.Serializable]
    public class CharacterData
    {
        public string Id;
        public string Name = "temp";
        public int Xp = 0;
        public Job Job;

        //public CharacterBuild Build; // skins

        //Equipment
        public List<EquipmentData> Equipments;
        //Appearance
        public List<LooksData> Looks;

        public Color color;

        public CharacterData(string _id)
        {
            Id = _id;
            Equipments = new List<EquipmentData>()
            {
                new EquipmentData(EquipmentType.HAT, null),
                new EquipmentData(EquipmentType.WEAPON, null),
                new EquipmentData(EquipmentType.ACCESSORY, null),
            };
            Looks = new List<LooksData>()
            {
                new LooksData(LooksType.EYES, null),
                new LooksData(LooksType.EYEBROWS, null),
                new LooksData(LooksType.FACE, null),
            };
        }

        public string GetEquipmentId(EquipmentType _equipmentType)
        {
            return Equipments.Find(x => x.EquipmentType == _equipmentType).Id;
        }

        [System.Serializable]
        public class EquipmentData
        {
            public string Id;
            public EquipmentType EquipmentType;

            public EquipmentData(EquipmentType _type, string id)
            {
                Id = id;
                EquipmentType = _type;
            }

        }

        [System.Serializable]
        public class LooksData
        {
            public string Id;
            public LooksType LooksType;

            public LooksData(LooksType _type, string id)
            {
                Id = id;
                LooksType = _type;
            }
        }
    }

    [System.Serializable]
    public class Map
    {
        public string Name = "";
        public List<Level> Levels = new List<Level>();

        [System.Serializable]
        public class Level
        {
            public string Id;
            public int Score = 0;
        }
    }

    #endregion
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProfileManager : MonoBehaviour {

    private static ProfileManager _instance;

    public Profile profile;

    [SerializeField] public bool Reset = false;

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
        LoadProfile();
    }

    [ExecuteInEditMode]
    void Update()
    {
        if( Reset == true)
        {
            Reset = false;
            ResetProfile();
        }
    }

    void ResetProfile()
    {
        profile = new Profile();
        SaveProfile();
    }

    void LoadProfile()
    {
        string json = PlayerPrefs.GetString("profile");
        if (string.IsNullOrEmpty(json))
        {
            //create blank profile save
            SaveProfile();
            return;
        }
        //PArse JSON
        profile = JsonUtility.FromJson<Profile>(json);
    }

    public void SaveProfile()
    {
        profile.Initialized = true;
        string json = JsonUtility.ToJson(profile);
        PlayerPrefs.SetString("profile", json);
        Debug.Log("[Saved Profile] " + json);
    }

    public Profile GetProfile()
    {
        return profile;
    }

    [System.Serializable]
    public class Profile
    {
        [SerializeField] public string PlayerName = "player";
        [SerializeField] public int Xp = 0;
        [SerializeField] public bool Initialized = false;

        [SerializeField] public List<CharacterData> Characters;

        [SerializeField] public List<string> CurrentTeam;

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

    [System.Serializable]
    public class CharacterData
    {
        public string Name = "char";
        public int Xp = 0;

        //Equipement
        //
        //

        public Stats AdditionalStats = new Stats();

        public CharacterData() { }

        public CharacterData(string name)
        {
            Name = name;
        }
    }
}

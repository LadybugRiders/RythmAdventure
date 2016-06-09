using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataManager : DatabaseLoader {

	private static DataManager _instance;
	private bool m_allLoaded = false;
	public string m_previousLevelName = "first_scene";

    //Battle Data
    [SerializeField] private BattleDataAsset m_battleData = null;
    
    //USER DATA
    //Global data
    JSONObject m_gameData;

	override protected void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        base.Awake();
		_instance = this;
		LoadUserData ();
		m_allLoaded = true;
	}

	public void LoadUserData(){
		string tempStr = null;
		//GLOBAL DATA
		tempStr = PlayerPrefs.GetString ("gameData");
		if (tempStr!= "" && tempStr !=null) {
			m_gameData = new JSONObject(tempStr);
		}
	}

	#region SAVE

	public void SaveAll(){
		//m_characterWrapper.Save ();
		SaveGameData ();
	}

	public void SaveGameData(){
		PlayerPrefs.SetString ("gameData", m_gameData.Print());
	}

	#endregion

	#region DATABASE
	
	protected override void LoadDatabase(){
        base.LoadDatabase();
	}

	#endregion
    
    #region PROPERTIES

    public static DataManager instance {
		get{
			if( _instance == null ){
                GameObject newGO = Instantiate(Resources.Load("prefabs/DataManager")) as GameObject; ;
                _instance = newGO.GetComponent<DataManager>();
			}
			return _instance;
		}
	}

    public BattleDataAsset BattleData
    {
        get { return m_battleData; }
        set { m_battleData = value; }
    }

	public JSONObject GameData {
		get {
			if( m_gameData == null ){			
				m_gameData = new JSONObject ();
			}
			return m_gameData;
		}
	}
    
	public bool IsLoaded {
		get {
			return m_allLoaded;
		}
	}

    public DataCharManager CharacterManager
    {
        get { return GetComponent<DataCharManager>(); }
    }

    public DataEnemiesManager EnemiesManager
    {
        get { return GetComponent<DataEnemiesManager>(); }
    }

    #endregion
}

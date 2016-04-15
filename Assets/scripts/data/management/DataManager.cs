using UnityEngine;
using System.Collections;
using System.IO;

public class DataManager : MonoBehaviour {

	private static DataManager _instance;
	private bool m_allLoaded = false;
	public string m_previousLevelName = "first_scene";

	//Manages the characters data
	private DataCharacterWrapper m_characterWrapper;

	//DATABASE
	//Object holding all the persistant data
	JSONObject m_database;

	//USER DATA
	//Global data
	JSONObject m_gameData;
	//Characters data
	JSONObject m_charactersData;


	void Awake(){
		DontDestroyOnLoad (this.gameObject);
		_instance = this;
		LoadUserData ();
		LoadDatabase ();
		m_allLoaded = true;
	}

	public void LoadUserData(){
		m_characterWrapper = new DataCharacterWrapper ();
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
	
	public void LoadDatabase(){
		m_database = new JSONObject ();
		JSONObject tempJson;
		tempJson = LoadDataJSON ("characters_database");
		m_database.AddField ("characters", tempJson);
	}

	#endregion

	public JSONObject LoadDataJSON(string _fileName){
		TextAsset json = Resources.Load ("database/"+_fileName) as TextAsset;
		//PArse JSON
		JSONObject jsonData = new JSONObject (json.text);
		Resources.UnloadAsset (json);
		return jsonData;
	}

	//============== GETTERS- SETTERS ==============================

	public static DataManager instance {
		get{
			if( _instance == null ){
				GameObject newGO = new GameObject("DataManager");
				_instance = newGO.AddComponent<DataManager>();
				_instance.LoadUserData ();
				_instance.LoadDatabase ();
			}
			return _instance;
		}
	}


	public DataActor GetCharacter(string _name){
		return null;
	}

	public JSONObject GameData {
		get {
			if( m_gameData == null ){			
				m_gameData = new JSONObject ();
			}
			return m_gameData;
		}
	}

	public JSONObject CharactersDatabase{
		get{
			return m_database.GetField("characters");
		}
	}

	public bool IsLoaded {
		get {
			return m_allLoaded;
		}
	}
	
}

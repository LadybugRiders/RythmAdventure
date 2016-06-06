using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DatabaseLoader : MonoBehaviour {

    //DATABASE
    //Object holding all the persistant data
    protected Dictionary<string, JSONObject> m_database;

    // Use this for initialization
    virtual protected void Awake () {
        LoadDatabase();
	}
	    
    protected virtual void LoadDatabase()
    {
        m_database = new Dictionary<string, JSONObject>();
    }

    protected JSONObject LoadDataJSON(string _fileName)
    {
        TextAsset json = Resources.Load("database/" + _fileName) as TextAsset;
        //PArse JSON
        JSONObject jsonData = new JSONObject(json.text);
        Resources.UnloadAsset(json);
        return jsonData;
    }
}

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

        JSONObject tempJson;
        tempJson = LoadDataJSON("other/scoring_database");
        m_database.Add("scoring", tempJson);
    }

    protected JSONObject LoadDataJSON(string _fileName)
    {
        TextAsset json = Resources.Load("database/" + _fileName) as TextAsset;
        //PArse JSON
        JSONObject jsonData = new JSONObject(json.text);
        Resources.UnloadAsset(json);
        return jsonData;
    }

    /// <summary>
    /// Try getting a specific database that has already been loaded
    /// </summary>
    public JSONObject GetDatabase(string _id)
    {
        if (m_database.ContainsKey(_id))
            return m_database[_id];
        return null;
    }
}

using UnityEngine;
using System.Collections;

public class DataInventoryManager : DatabaseLoader
{

    protected override void LoadDatabase()
    {
        base.LoadDatabase();

        JSONObject tempJson;
        tempJson = LoadDataJSON("inventory_database");
        m_database.Add("inventory", tempJson);
    }

    public ActionData GetActionData(string _id, string _type)
    {
        JSONObject actions = Database[_type];
        var actionJSON = actions.list.Find(x => x.GetField("id").str == _id);
        return new ActionData(actionJSON);
    }

    JSONObject Database { get { return m_database["inventory"]; } }

    public class ActionData
    {
        public string Id;
        public string Name;
        public string Prefab;
        public int Power;
        public string Type;

        public ActionData(JSONObject _json)
        {
            Id = _json.GetField("id").str;
            Name = _json.GetField("name").str;
            Prefab = _json.GetField("prefab").str;
            Power = (int)_json.GetField("power").f;
            Type = _json.GetField("type").str;
        }
    }
}

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
        return null;
    }

    public ActionData GetAttackActionData(string _id)
    {
        return GetActionData(_id, "attack");
    }

    public ActionData GetMagicActionData(string _id)
    {
        return GetActionData(_id, "magic");
    }

    JSONObject Database { get { return m_database["inventory"]; } }

    #region ACTIONDATA
    public class ActionData : JSONData
    {
        public string Name;
        public string Prefab;
        public int Power;
        public int MpCost;
        public bool Offense;
        public string Description;

        public override void BuildJSONData(JSONObject _json)
        {
            base.BuildJSONData(_json);
            Name = _json.GetField("name").str;
            Prefab = _json.GetField("prefab").str;
            Power = (int)_json.GetField("power").f;
            MpCost = (int)_json.GetField("mp").f;
            var desc = _json.GetField("description");
            if (desc != null)
                Description = desc.str;
            //type
            string strType = _json.GetField("type").str;
            Offense = strType == "offense" ? true : false;
        }
    }

    public class ActionDataCollection : IJSONDataDicoCollection<ActionData> { }

    #endregion

    #region ITEM_DATA
    public class ItemData : GameUtils.WeightableData
    {
        public string Name;
        public string Image;
        public string Description;

        public override void BuildJSONData(JSONObject _json)
        {
            base.BuildJSONData(_json);
            Name = _json.GetField("name").str;
            Image = _json.GetField("image").str;
            Description = _json.GetField("description").str;
        }
    }

    public class ItemDataCollection : IJSONDataDicoCollection<ItemData> { }

    #endregion
}

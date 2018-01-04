using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataGameManager : DatabaseLoader
{
    AccuracyScoringData m_accuracyScoring;
    ElementsDataCollection m_elements;
    AttackTypeDataCollection m_attackTypes;

    protected override void LoadDatabase()
    {
        base.LoadDatabase();
        var tempDatabase = LoadDataJSON("game_database");
        var accJSON = tempDatabase["accuracy_scoring"];
        m_accuracyScoring = new AccuracyScoringData(accJSON[0]);
        m_elements = JSONLoaderLR.LoadTable<ElementsDataCollection>(tempDatabase["elements"]);
        m_attackTypes = JSONLoaderLR.LoadTable<AttackTypeDataCollection>(tempDatabase["attack_type"]);
    }

    public class AccuracyScoringData : JSONData
    {
        public Dictionary<HitAccuracy, int> scores = new Dictionary<HitAccuracy, int>();

        public AccuracyScoringData(JSONObject _json) : base(_json) { }

        public override void BuildJSONData(JSONObject _json)
        {
            base.BuildJSONData(_json);
            for(int i=0; i < Utils.EnumCount(HitAccuracy.GOOD) -1; ++i)
            {
                HitAccuracy hit = ((HitAccuracy)i);
                int acc = (int)_json.GetField(hit.ToString().ToLower()).f;
                scores[hit] = acc;
            }
        }
    }

    public ElementData GetElementData(string _elementId)
    {
        return m_elements[_elementId];
    }

    public AttackTypeData GetAttackTypeData(string _elementId)
    {
        return m_attackTypes[_elementId];
    }

    #region ELEMENTS
    public class ElementData : JSONData
    {
        public string Name { get; set; }
        public string Weakness { get; set; }
        public string Prefab { get; set; }

        public ElementData() { }
        public ElementData(JSONObject _json) : base(_json) { }        
    }

    public class ElementsDataCollection : IJSONDataDicoCollection<ElementData> { }

    #endregion

    #region ATTACK_TYPE
    public class AttackTypeData : JSONData
    {
        public string Name { get; set; }
        public string Prefab { get; set; }

        public AttackTypeData() { }
        public AttackTypeData(JSONObject _json) : base(_json) { }
        
    }

    public class AttackTypeDataCollection : IJSONDataDicoCollection<AttackTypeData> { }

    #endregion

    public AccuracyScoringData AccuracyScoring { get { return m_accuracyScoring; } }
}

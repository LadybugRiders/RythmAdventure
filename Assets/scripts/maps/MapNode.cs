using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapNode : MonoBehaviour {

    [SerializeField] string m_id = "MapId";

    [SerializeField] List<MapNode> m_parents;
    [SerializeField] List<MapNode> m_children;

    [SerializeField] BattleDataAsset m_battleData;
    
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    #region GETTERS-SETTERS

    public List<MapNode> Parents
    {
        get
        {
            return m_parents;
        }        
    }

    public List<MapNode> Children
    {
        get
        {
            return m_children;
        }        
    }

    public BattleDataAsset BattleData
    {
        get
        {
            return m_battleData;
        }        
    }

    public string Id
    {
        get
        {
            return m_id;
        }
    }
    #endregion
}

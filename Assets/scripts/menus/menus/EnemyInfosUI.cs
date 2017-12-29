using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyInfosUI : MonoBehaviour {

    [SerializeField] protected bool m_autoLoad = true;
    
    [SerializeField] protected float m_uiEnemyScale;
    
    [SerializeField] protected GameObject m_elementAttribute;
    [SerializeField] protected GameObject m_attackAttribute;

    public Transform EnemyRoot;
    public Text LevelText;
    public Text NameText;

    protected DataEnemiesManager.EnemyData m_data;
    protected GameObject m_spawnedObject;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Load(string _id)
    {
        var enemiesManager = DataManager.instance.EnemiesManager;
        m_data = enemiesManager.GetEnemy(_id);

        if( NameText != null)
            NameText.text = m_data.Name;
        if(LevelText != null)
            LevelText.text = ""+m_data.Stats.Level;

        if (m_spawnedObject != null)
            Destroy(m_spawnedObject);

        var go = Resources.Load("prefabs/enemy/"+m_data.Prefab) as GameObject;
        m_spawnedObject = Instantiate(go);

        //convert to ui
        Utils.SetLayerRecursively(m_spawnedObject, LayerMask.NameToLayer("SpriteUI"));
        Utils.ConvertToUIImage(m_spawnedObject);
        //Set Parent
        GameObject container = new GameObject("Enemy_" + _id);
        Utils.SetLocalScaleXY(m_spawnedObject.transform, m_uiEnemyScale, m_uiEnemyScale);
        var rect = container.AddComponent<RectTransform>();
        m_spawnedObject.transform.SetParent(EnemyRoot, false);        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyInfosUI : MonoBehaviour {

    [SerializeField] protected bool m_autoLoad = true;
    
    [SerializeField] protected float m_uiCharaScale;
    
    [SerializeField] protected GameObject m_elementAttribute;
    [SerializeField] protected GameObject m_attackAttribute;

    public GameObject CharacterObject;
    public Text LevelText;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBeginCharListUI : MonoBehaviour {

    [SerializeField] protected Transform m_panel;
    [SerializeField] protected GameObject m_charaPrefab;
    
    List<GameObject> m_inventory;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Clean()
    {
        for(int i = m_panel.childCount - 1; i >=0; i--)
        {
            var child = m_panel.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    public void Load()
    {
        Clean();

        var profile = ProfileManager.instance.profile;
        m_inventory = new List<GameObject>();
        for (int i = 0; i < profile.Characters.Count; i++)
        {
            var charData = profile.Characters[i];
            //check if not in party
            if (!profile.CurrentTeam.Contains(charData.Id))
            {
                GameObject go = Instantiate( m_charaPrefab ) as GameObject;
                go.transform.SetParent(m_panel, false);
                m_inventory.Add(go);
                go.GetComponent<CharacterInfosUI>().Load(charData.Id);
            }
        }
    }
}

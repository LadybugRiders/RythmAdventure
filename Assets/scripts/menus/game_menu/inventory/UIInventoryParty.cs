using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInventoryParty : MonoBehaviour {

    List<GameObject> m_party;
    
    ProfileManager.Profile m_profile;

    // Use this for initialization
    void Start () { 
        m_party = new List<GameObject>() { null, null, null };
        LoadParty();
    }
	
	// Update is called once per frame
	void Update () {     
    }

    public void LoadParty()
    {
        m_profile = ProfileManager.instance.GetProfile();

        for (int i = 0; i < m_profile.CurrentTeam.Count; i++)
        {
            string charId = m_profile.CurrentTeam[i];
            if (charId == null)
            {
                if (m_party[i] != null)
                    Destroy(m_party[i].gameObject);
                continue;
            }
            GameObject character = DataManager.instance.CreateCharacter(charId);
            Utils.SetLayerRecursively(character, LayerMask.NameToLayer("SpriteUI"));
            character.transform.SetParent(transform, false);
        }
    }
    
}

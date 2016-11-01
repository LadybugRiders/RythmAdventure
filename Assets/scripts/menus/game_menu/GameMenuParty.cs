using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMenuParty : GameMenu {

    [SerializeField] List<CharacterInfos> m_characters;
    [SerializeField] Transform m_charUIParent;
    [SerializeField] float m_itemScale = 10.0f;

    DataCharManager m_charManager;
    ProfileManager.Profile m_profile;

    List<GameObject> m_charactersUI;

	// Use this for initialization
	void Start () {
        m_profile = ProfileManager.instance.GetProfile();
        m_charManager = DataManager.instance.CharacterManager;
        m_charactersUI = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected override void Activate()
    {
        m_profile = ProfileManager.instance.GetProfile();
        m_charManager = DataManager.instance.CharacterManager;
        base.Activate();
        for(int i=0; i < m_profile.CurrentTeam.Count; i++)
        {
            //Create character
            GameObject character = DataManager.instance.CreateCharacter(m_profile.CurrentTeam[i]);
            character.name = "Char_" + i;
            //convert to ui
            Utils.SetLayerRecursively(character, LayerMask.NameToLayer("SpriteUI"));
            Utils.ConvertToUIImage(character);
            //Set Parent
            GameObject container = new GameObject("Char_" + i);
            Utils.SetLocalScaleXY(character.transform, m_itemScale, m_itemScale);
            container.AddComponent<RectTransform>();
            container.transform.SetParent(m_charUIParent, false);
            character.transform.SetParent(container.transform, false);
            //Set Draggable
            container.AddComponent<UIInventoryDraggableItem>();
        }
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        Clear();
    }
    
    void Clear()
    {
        foreach(var chara in m_charactersUI)
        {
            Destroy(chara);
        }
        m_charactersUI.Clear();
    }

    [System.Serializable]
    public class CharacterInfos
    {
        public CharacterBuild m_build;
        public StatsFiller m_stats;
    }
}

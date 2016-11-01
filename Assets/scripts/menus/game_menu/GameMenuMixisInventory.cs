using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMenuMixisInventory : GameMenu {

    [SerializeField] Transform m_partyTransform;
    [SerializeField] Transform m_inventoryTransform;

    [SerializeField] float m_partyItemScale = 10.0f;
    [SerializeField] float m_inventoryItemScale = 10.0f;

    DataCharManager m_charManager;
    ProfileManager.Profile m_profile;

    List<GameObject> m_party;
    List<GameObject> m_inventory;

    // Use this for initialization
    void Start () {
        LoadParty();
        LoadInventory();
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
            GameObject go = CreateCharacterUIObject(charId, m_partyItemScale);
            go.GetComponent<UIInventoryDraggableItem>().IsDraggable = false;
            go.transform.SetParent(m_partyTransform, false);
        }
    }

    void LoadInventory()
    {
        for (int i = 0; i < m_profile.Characters.Count; i++)
        {
            var charData = m_profile.Characters[i];
            //check if not in party
            if (!m_profile.CurrentTeam.Contains(charData.Id))
            {
                GameObject go = CreateCharacterUIObject(charData.Id, m_inventoryItemScale);
                go.transform.SetParent(m_inventoryTransform, false);
            }
        }
    }

    GameObject CreateCharacterUIObject(string _id, float _scale)
    {
        //Create character
        GameObject character = DataManager.instance.CreateCharacter(_id);
        character.name = _id;
        //convert to ui
        Utils.SetLayerRecursively(character, LayerMask.NameToLayer("SpriteUI"));
        Utils.ConvertToUIImage(character);
        //Set Parent
        GameObject container = new GameObject("Char_" + _id);
        Utils.SetLocalScaleXY(character.transform, _scale, _scale);
        container.AddComponent<RectTransform>();
        character.transform.SetParent(container.transform, false);
        //Set Draggable
        container.AddComponent<UIInventoryDraggableItem>();
        return container;
    }
}

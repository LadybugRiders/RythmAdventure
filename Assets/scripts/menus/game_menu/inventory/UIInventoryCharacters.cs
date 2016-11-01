using UnityEngine;
using System.Collections;

public class UIInventoryCharacters : MonoBehaviour {
    [SerializeField] float m_itemScale = 10.0f;
    DataCharManager m_charManager;
    ProfileManager.Profile m_profile;

    // Use this for initialization
    void Start () {
        LoadCharacters();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadCharacters()
    {
        m_profile = ProfileManager.instance.GetProfile();
        m_charManager = DataManager.instance.CharacterManager;

        for (int i = 0; i < m_profile.Characters.Count; i++)
        {
            var charData = m_profile.Characters[i];
            //check if not in party
            if( !m_profile.CurrentTeam.Contains(charData.Id))
            {
                //Create character
                GameObject character = DataManager.instance.CreateCharacter(charData.Id);
                character.name = "Char_" + i;
                //convert to ui
                Utils.SetLayerRecursively(character, LayerMask.NameToLayer("SpriteUI"));
                Utils.ConvertToUIImage(character);
                //Set Parent
                GameObject container = new GameObject("Char_" + i);
                Utils.SetLocalScaleXY(character.transform, m_itemScale, m_itemScale);
                container.AddComponent<RectTransform>();
                container.transform.SetParent(transform, false);
                character.transform.SetParent(container.transform, false);
                //Set Draggable
                container.AddComponent<UIInventoryDraggableItem>();
            }
        }
    }
}

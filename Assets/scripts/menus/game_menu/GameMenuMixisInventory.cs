using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameMenuMixisInventory : GameMenu {

    [SerializeField] Transform m_partyTransform;
    [SerializeField] Transform m_inventoryTransform;

    [SerializeField] float m_partyItemScale = 10.0f;
    [SerializeField] float m_inventoryItemScale = 10.0f;
    
    [SerializeField] StatsFiller m_statsFiller;
    [SerializeField] StatsFiller m_secondStatsFiller;

    DataCharManager m_charManager;
    ProfileManager.Profile m_profile;

    List<GameObject> m_party;
    List<GameObject> m_inventory;

    Stats m_mainStats;
    	
	// Update is called once per frame
	void Update () {
	
	}

    protected override void Activate()
    {
        base.Activate();
        if (m_alreadyActivated == false) {
            LoadParty();
            LoadInventory();
        }
    }

    protected override void Deactivate()
    {
        base.Deactivate();
    }

    #region LOADING

    public void LoadParty()
    {
        m_profile = ProfileManager.instance.GetProfile();
        m_party = new List<GameObject>();

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
            go.GetComponent<UIInventoryDraggableItem>().Menu = this;
            go.transform.SetParent(m_partyTransform, false);

            m_party.Add(go);
        }
    }

    void LoadInventory()
    {
        m_inventory = new List<GameObject>();
        for (int i = 0; i < m_profile.Characters.Count; i++)
        {
            var charData = m_profile.Characters[i];
            //check if not in party
            if (!m_profile.CurrentTeam.Contains(charData.Id))
            {
                GameObject go = CreateCharacterUIObject(charData.Id, m_inventoryItemScale);
                go.transform.SetParent(m_inventoryTransform, false);
                go.GetComponent<UIInventoryDraggableItem>().Menu = this;
                m_inventory.Add(go);
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
        var rect = container.AddComponent<RectTransform>();
        character.transform.SetParent(container.transform, false);
        //Set Draggable
        var uiItem = container.AddComponent<UIInventoryDraggableItem>();
        uiItem.ItemParentTransform = character.transform;
        uiItem.CharId = _id;
        return container;
    }

    #endregion

    #region DRAG
    public void OnInventoryItemDrag(UIInventoryDraggableItem _item)
    {
        LoadMainStats(_item.CharId);
        var go = GetPartyItemOvered(_item.gameObject);
        if( go != null)
        {
            LoadCompareStats(go.GetComponent<UIInventoryDraggableItem>().CharId);
        }        
    }

    public bool OnInventoryItemDrop(UIInventoryDraggableItem _item)
    {
        var go = GetPartyItemOvered(_item.gameObject);
        if (go != null)
        {
            SwitchPartyMember(go.GetComponent<UIInventoryDraggableItem>(), _item);
            return true;
        }
        return false;
    }

    float ItemsDistance(GameObject obj1, GameObject obj2)
    {
        RectTransform rt1 = obj1.GetComponent<RectTransform>();
        RectTransform rt2 = obj2.GetComponent<RectTransform>();

        var mag = (rt1.position - rt2.position).magnitude;
        return mag;
    }

    GameObject GetPartyItemOvered(GameObject _item)
    {
        float mindistance = float.MaxValue;
        GameObject go = null;
        foreach (var chara in m_party)
        {
            float dist = ItemsDistance(_item.gameObject, chara.gameObject);
            if (dist < 50.0f && dist < mindistance)
            {
                go = chara;
                mindistance = dist;
            }
        }
        return go;
    }
    #endregion

    void SwitchPartyMember(UIInventoryDraggableItem _partyItem, UIInventoryDraggableItem _inventoryItem)
    {
        string partyItemId = _partyItem.CharId;
        string invItemId = _inventoryItem.CharId;
        Transform tempPartyTransform = _partyItem.ItemParentTransform;

        //Set party member into inventory
        Utils.SetLocalScaleXY(_partyItem.ItemParentTransform, m_inventoryItemScale, m_inventoryItemScale);
        _partyItem.ItemParentTransform.SetParent(_inventoryItem.transform, false);
        _partyItem.ItemParentTransform = _inventoryItem.ItemParentTransform;
        _partyItem.CharId = invItemId;

        //Set inventory member into party
        Utils.SetLocalScaleXY(_inventoryItem.ItemParentTransform, m_partyItemScale, m_partyItemScale);
        _inventoryItem.ItemParentTransform.SetParent(_partyItem.transform, false);
        _inventoryItem.ItemParentTransform = tempPartyTransform;
        _inventoryItem.CharId = partyItemId;

        ProfileManager.instance.ReplacePartyCharacter(partyItemId, invItemId);
    }

    void LoadMainStats(string _charId)
    {
        m_mainStats = ProfileManager.instance.GetCharacterStats(_charId);
        m_statsFiller.Load(m_mainStats);
    }

    void LoadCompareStats(string _charId)
    {
        var charStats = ProfileManager.instance.GetCharacterStats(_charId);
        m_secondStatsFiller.Load(charStats,m_mainStats);
    }
}

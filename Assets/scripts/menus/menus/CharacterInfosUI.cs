using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfosUI : MonoBehaviour {

    [SerializeField] protected int m_characterIndexInProfile;
    [SerializeField] protected float m_uiCharaScale;

    [SerializeField] protected UITextAndImage m_metaValue;
    [SerializeField] protected GameObject m_elementAttribute;
    [SerializeField] protected GameObject m_attackAttribute;
    [SerializeField] protected GameObject m_attribute;

    public GameObject CharacterObject;
    public Text LevelText;

    // Use this for initialization
    void Start () {
        LoadData();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadData()
    {
        var m_charManager = DataManager.instance.CharacterManager;

        //get infos for the character
        var teamMates = ProfileManager.instance.GetCurrentTeam();
        var mate = teamMates[m_characterIndexInProfile];
        var levelUpData = m_charManager.GetNextLevelByXp(mate.Job, mate.Xp);

        var go = GameUtils.CreateCharacterUIObject(mate, m_uiCharaScale);
        go.transform.SetParent( CharacterObject.transform, false);

        LevelText.text = "" + levelUpData.Stats.Level; 
    }
}

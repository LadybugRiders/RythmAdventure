using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameMenuMixiGenerator : GameMenu
{
    [SerializeField] GameObject m_buttonGen;
    [SerializeField] GameObject m_mixiGO;
    GameObject m_currentMixi;

    [SerializeField] float m_mixiScale = 5;

    MixiGenerator generator;

	// Use this for initialization
	void Start () {
        generator = new MixiGenerator();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected override void Activate()
    {
        base.Activate();
        m_mixiGO.SetActive(false);
        m_buttonGen.SetActive(true);
    }

    public void OnGenerateMixiButtonClicked()
    {
        m_buttonGen.SetActive(false);
        m_mixiGO.SetActive(true);

        //generate the data for the mixi
        var chara = generator.Generate(1);
        ProfileManager.instance.AddCharacter(chara);
        
        //Create the ui object to display
        m_currentMixi = GameUtils.CreateCharacterUIObject(chara, m_mixiScale, false);
        Destroy(m_currentMixi.GetComponent<UIInventoryDraggableItem>());
        m_currentMixi.transform.SetParent(m_mixiGO.transform, false);

    }

    public void OnMixiClicked()
    {
        //Delete current Mixi
        if (m_currentMixi != null)
        {
            Destroy(m_currentMixi);
        }
        m_mixiGO.SetActive(false);
        m_buttonGen.SetActive(true);
    }
}

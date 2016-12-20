using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameMenuMixiGenerator : GameMenu
{
    [SerializeField] GameObject m_buttonGen;
    [SerializeField] GameObject m_mixiGO;

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

        var chara = generator.Generate(1);

        var go = GameUtils.CreateCharacterUIObject(chara, m_mixiScale, false);
        go.transform.SetParent(m_mixiGO.transform, false);
    }
}

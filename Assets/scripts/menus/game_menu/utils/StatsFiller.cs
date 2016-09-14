using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatsFiller : MonoBehaviour {
                              
    [SerializeField] Text m_hpText;
    [SerializeField] Text m_mpText;
    [SerializeField] Text m_attackText;
    [SerializeField] Text m_defenseText;
    [SerializeField] Text m_magicText;
    [SerializeField] Text m_speedText;

    // Use this for initialization
    void Start () {
	
	}

    public void Load(Stats _stats)
    {
        FillStat( m_hpText , _stats.HP);
        FillStat( m_mpText , _stats.MP);
        FillStat(m_attackText, _stats.Attack);
        FillStat(m_defenseText, _stats.Defense);
        FillStat(m_magicText, _stats.Magic);
        FillStat(m_speedText, _stats.Speed);
    }

    void FillStat( Text _text, object _value)
    {
        if (_text != null)
            _text.text = _value.ToString();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

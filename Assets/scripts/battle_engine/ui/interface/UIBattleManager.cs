using UnityEngine;
using System.Collections;

public class UIBattleManager : MonoBehaviour {
	
	[SerializeField] UIBattlePhaseTitle m_phaseTitle;

	// Use this for initialization
	void Start () {		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SwitchPhase(bool _attack){
		m_phaseTitle.Switch (_attack);
	}
}

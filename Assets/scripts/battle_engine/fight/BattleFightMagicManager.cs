using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFightMagicManager : MonoBehaviour {

	[SerializeField] protected BattleFightManager m_fightManager;
	[SerializeField] protected List<BattleFightMagic> m_simpleMagics;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMagicEnded( BattleFightMagic _magic){
		m_fightManager.OnMagicEnded (_magic);
	}

	public BattleFightMagic GetFreeMagic( List<BattleFightMagic> _magics){
		for( int i=0; i < _magics.Count; i ++ ){
			if( ! _magics[i].IsLaunched ){
				return _magics[i];
			}
		}
		return null;
	}
}

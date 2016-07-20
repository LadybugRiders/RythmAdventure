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
    /*
	// Creates and launch a magic, or make an existing magic attack 
	public BattleFightMagic LaunchMagic(BattleActor _caster, BattleActor _target, int _duelID){
		//if the caster is already casting
		if (_caster.isCasting) {
			return null;
		}
		BattleFightMagic magic = GetFreeMagic (m_simpleMagics);
		if (magic == null) 
			return null;
		magic.Launch (_caster, _target,_duelID);
		return magic;
	}
    */
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

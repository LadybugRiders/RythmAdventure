using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleScoreManager : MonoBehaviour {

	public enum Accuracy { PERFECT, GREAT, GOOD, BAD, MISS };
	
	//Accuracy variables
	[SerializeField] private float m_accuPerfect = 80f;
	[SerializeField] private float m_accuGreat = 50f;

	//COUNT
	private int m_notesCount = 0;
	private List<int> m_notesCountByAcc;

	//SCORE
	private int m_totalScore = 0;
	[SerializeField] private List<int> m_baseScoreByAcc;
	private List<int> m_currentScoreByAcc;

	// Use this for initialization
	void Start () {
		//init Counts
		m_notesCountByAcc = new List<int> ();
		for (int i = 0; i < 4; i++) {
			m_notesCountByAcc.Add( 0 );
		}
		//init scores values
		m_currentScoreByAcc = new List<int>();
		for (int i = 0; i < m_baseScoreByAcc.Count; i++) {
			m_currentScoreByAcc.Add( m_baseScoreByAcc[i] );
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/** Adds Note to the scoreManager and returns accuracy (BattleScoreManager.Accuracy) */
	public BattleScoreManager.Accuracy AddNote ( float _accuracy ){
		m_notesCount ++;
		if (_accuracy > m_accuPerfect) {
			m_notesCountByAcc [0] ++;
			m_totalScore += m_currentScoreByAcc[0];
			return Accuracy.PERFECT;
		} else if (_accuracy > m_accuGreat) {			
			m_notesCountByAcc[1] ++;
			m_totalScore += m_currentScoreByAcc[1];
			return Accuracy.GREAT;
		} else if (_accuracy > 0) {
			m_notesCountByAcc[2] ++;
			m_totalScore += m_currentScoreByAcc[2];
			return Accuracy.GOOD;
		}
		m_notesCountByAcc [3] ++;
		return Accuracy.BAD;
	}
}

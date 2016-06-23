using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleScoreManager : MonoBehaviour {

	public enum Accuracy { PERFECT, GREAT, GOOD, MISS };
	
	//Accuracy variables
	[SerializeField] private float m_accuPerfect = 80f;
	[SerializeField] private float m_accuGreat = 50f;

	//COUNT
	private int m_notesCount = 0;
	private Dictionary<Accuracy,int> m_notesCountByAcc;

	//SCORE
	private int m_totalScore = 0;
	[SerializeField] private Dictionary<Accuracy, int>  m_baseScoreByAcc;
	private Dictionary<Accuracy, int> m_currentScoreByAcc;

	// Use this for initialization
	void Start () {
        //init Counts
        m_notesCountByAcc = new Dictionary<Accuracy, int>();
        //init scores values
        m_currentScoreByAcc = new Dictionary<Accuracy, int>();

        for (int i = 0; i < Utils.EnumCount(Accuracy.GOOD); i++) {
            Accuracy acc = (Accuracy)i;
			m_notesCountByAcc.Add(acc, 0 );
            m_currentScoreByAcc.Add(acc, 0);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/** Adds Note to the scoreManager and returns accuracy (BattleScoreManager.Accuracy) */
	public BattleScoreManager.Accuracy AddNote ( float _accuracyValue ){
		m_notesCount ++;
        Accuracy acc = GetAccuracyByValue(_accuracyValue);

        m_totalScore += m_currentScoreByAcc[acc];
        m_notesCountByAcc[acc]++;
		return acc;
	}

    public Accuracy GetAccuracyByValue(float _accuracyValue)
    {
        Accuracy acc;
        if (_accuracyValue > m_accuPerfect)
        {
            acc = Accuracy.PERFECT;
        }
        else if (_accuracyValue > m_accuGreat)
        {
            acc = Accuracy.GREAT;
        }
        else if (_accuracyValue > 0)
        {
            acc = Accuracy.GOOD;
        }
        else
        {
            acc = Accuracy.MISS;
        }
        return acc;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleScoreManager : MonoBehaviour {

	public enum Accuracy { PERFECT, GREAT, GOOD, MISS };
	
	//Accuracy variables
	[SerializeField] private float m_accuPerfect = 80f;
	[SerializeField] private float m_accuGreat = 50f;

	//COUNT
	public int m_notesCount = 0;
    /// <summary>
    /// Notes sorted by accuracy
    /// </summary>
	public Dictionary<Accuracy,int> m_notesCountByAcc;

	//SCORE
	public int m_totalScore = 0;
	[SerializeField] public Dictionary<Accuracy, int>  m_baseScoreByAcc;

	// Use this for initialization
	void Start () {
        //init Counts
        m_notesCountByAcc = new Dictionary<Accuracy, int>();

        for (int i = 0; i < Utils.EnumCount(Accuracy.GOOD); i++) {
            Accuracy acc = (Accuracy)i;
			m_notesCountByAcc.Add(acc, 0 );
        }

        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/** Adds Note to the scoreManager and returns accuracy (BattleScoreManager.Accuracy) */
	public BattleScoreManager.Accuracy AddNote ( float _accuracyValue ){
        //increase total
		m_notesCount ++;
        //Compute Accuracy
        Accuracy acc = GetAccuracyByValue(_accuracyValue);        
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

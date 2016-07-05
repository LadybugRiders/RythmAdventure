using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DebugBattleVictory : MonoBehaviour {

    bool m_ended = false;
    	
	// Update is called once per frame
	void Update () {
        if (!m_ended)
            EndMatch();
	}

    void EndMatch()
    {
        m_ended = true;

        BattleScoreManager scoreManager = Component.FindObjectOfType<BattleScoreManager>();

        /*foreach (var acc in scoreManager.m_notesCountByAcc.Keys)
        {
            int r = Random.Range(0, 30);
            scoreManager.m_notesCountByAcc[acc] = r;
            scoreManager.m_notesCount += r;
        }*/

        SceneManager.LoadScene("battle_end");
    }
}

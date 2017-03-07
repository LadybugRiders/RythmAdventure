using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBattleEndScoreScrollSequence : UISequence {

    [SerializeField] UITextNumberScroller m_scoreScroller;

    int m_totalScore;

    public void Launch(OnSeqenceEndDelegate del, Dictionary<HitAccuracy,int> _acurracies, int _totalScore)
    {
        base.Launch(del,false);
        (CurrentStep as UIBattleEndScoreScrollStep).Launch(OnStepEnd, _acurracies);
        m_totalScore = _totalScore;
    }
    
    public override void Skip()
    {
        base.Skip();
    }

    public override void OnStepEnd(UIStep step)
    {
        switch (step.Id)
        {
            case "accuracies":
                m_currentStepIndex++;
                (CurrentStep as UIBattleEndStarsStep).Launch(OnStepEnd);
                break;
            case "stars":
                Stop();
                break;
        }
    }
        
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBattleEndXpSequence : UISequence {

    int m_totalXp;

    public void Launch(OnSeqenceEndDelegate del, Dictionary<HitAccuracy, int> _acurracies, int _totalXp)
    {
        base.Launch(del, false);
        (CurrentStep as UIBattleEndScoreScrollStep).Launch(OnStepEnd, _acurracies);
        m_totalXp = _totalXp;
    }

    public override void Skip()
    {
        base.Skip();
    }

    public override void OnStepEnd(UIStep step)
    {
        switch (step.Id)
        {
        }
    }
}

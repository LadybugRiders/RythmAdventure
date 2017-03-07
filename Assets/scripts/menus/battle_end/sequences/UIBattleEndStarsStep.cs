using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattleEndStarsStep : UIStep {

    [SerializeField] Transform m_starsParent;

    int m_currentStarId = -1;
    Transform m_currentStar;
    Animator m_currentAnimator;
    string m_animName = "";

    int m_starCount = 3;
    
	void Start () {
        //deactivate all star images
		for(int i =0; i < m_starsParent.childCount; ++i)
        {
            GetStarImage(m_starsParent.GetChild(i)).gameObject.SetActive(false);
        }
	}

    protected override void UpdateStep()
    {
        base.UpdateStep();    
        if (m_currentStar != null)
        {
            var animRunning = Utils.IsAnimationStateRunning(m_currentAnimator, m_animName, false);
            if (animRunning == false)
                LaunchNextStar();
        }    
    }

    public override void Skip()
    {
        m_currentStarId = m_starCount;
        if( m_currentAnimator != null)
        {
            m_currentAnimator.Play("idle");
        }
        for(int i = 0; i < m_starCount; i++)
        {
            var t = m_starsParent.GetChild(i);
            GetStarImage(t).gameObject.SetActive(true);
        }
        base.Skip();
    }

    public override void Launch(OnStepEndDelegate _del)
    {
        base.Launch(_del);
        LaunchNextStar();
    }

    void LaunchNextStar()
    {
        m_currentStarId++;
        if( m_currentStarId >= m_starCount )
        {
            m_currentStar = null;
            m_currentAnimator = null;
            Stop();
            return;
        }

        //Get star object
        m_currentStar = m_starsParent.GetChild(m_currentStarId);
        //Get Filled star object
        var starImg = GetStarImage(m_currentStar);
        starImg.gameObject.SetActive(true);
        m_currentAnimator = starImg.GetComponent<Animator>();

        //The animation is different it is the last star
        m_animName = (m_currentStarId == m_starCount -1) ? "last_scale" : "normal_scale";
        m_currentAnimator.SetTrigger(m_animName);
    }

    Transform GetStarImage(Transform _starTransform)
    {
        return _starTransform.FindChild("star_img");
    }
}

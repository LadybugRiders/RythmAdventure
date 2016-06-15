using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITextNumberScroller : MonoBehaviour {

    protected Text m_text;

    protected bool m_scrolling = false;

    protected int m_targetNumber = 0;

    protected float m_timeByUnit = 1;
    protected float m_time = 0;
    protected int m_direction = 1;

	// Use this for initialization
	void Awake () {
        m_text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
	    if( m_scrolling )
        {
            m_time += Time.deltaTime;
            if( m_time >= m_timeByUnit)
            {
                m_time = 0;
                int current = int.Parse( m_text.text );
                current += m_direction;
                m_text.text = "" + current;
                if (current == m_targetNumber)
                    m_scrolling = false;
            }
        }
	}

    protected virtual void _ZeroReached()
    {

    }

    public virtual void ScrollTo(int _targetNumber, float _duration)
    {
        m_targetNumber = _targetNumber;
        m_scrolling = true;

        var current = int.Parse(m_text.text);
        //Get direction of the scroll
        int delta = _targetNumber - current;
        m_direction = delta < 0 ? -1 : 1;
        //compute the speed
        m_timeByUnit = _duration / Mathf.Abs(delta);
        m_time = 0;
    }
}

using UnityEngine;
using System.Collections;

public class UIGauge : MonoBehaviour {

	[SerializeField] protected float m_currentValue = 1.0f;

	public enum ORIENTATION { HORIZONTAL, VERTICAL};
	[SerializeField] protected ORIENTATION m_orientation;

	public enum ALIGN { TOP, BOTTOM, LEFT, RIGHT, CENTER };
	[SerializeField] protected ALIGN m_align;

	[SerializeField] protected SpriteRenderer m_gaugeSpr;

	[SerializeField] protected float m_maxScale = 1.0f;
	[SerializeField] protected bool m_useSprScaleAsMax = true;
    
	private Vector3 m_initPos;

    private float m_posAtZero = 0;
    private float m_posRange = 0;

	protected Transform m_gaugeTransform;

	// Use this for initialization
	virtual protected void Awake () {

        if (m_gaugeSpr != null)
        {
            m_gaugeTransform = m_gaugeSpr.transform;

            //Override max scale if needed
            if (m_useSprScaleAsMax)
            {
                m_maxScale = m_orientation == ORIENTATION.HORIZONTAL ? m_gaugeTransform.localScale.x : m_gaugeTransform.localScale.y;
            }
            ComputeEdge();
        }
        
		SetValue (m_currentValue);
	}

    public void ChangeOrientation(ORIENTATION _orientation, ALIGN _align)
    {
        m_orientation = _orientation;
        m_align = _align;
        m_gaugeTransform.localPosition = m_initPos;
        ComputeEdge();
    }

    //Scale the sprite to max value, then compute the edge according to the orientation and alignement
    //The edge is the position of the sprite when the gauge is at 0
    void ComputeEdge()
    {
        float posAtMax = 0.0f;
        //change scale to max to have the entire gauge full
        if (m_orientation == ORIENTATION.HORIZONTAL)
        {
            Utils.SetLocalScaleX(m_gaugeTransform, m_maxScale);
            var worldToLocal = m_gaugeTransform.localPosition - m_gaugeTransform.position;
            if ( m_align == ALIGN.LEFT)
            {
                m_posAtZero = m_gaugeSpr.bounds.min.x + worldToLocal.x;
            }else if(m_align == ALIGN.RIGHT)
            {
                m_posAtZero = m_gaugeSpr.bounds.max.x + worldToLocal.x;
            }
            posAtMax = m_gaugeSpr.bounds.center.x + worldToLocal.x;
        }
        else
        {
            Utils.SetLocalScaleY(m_gaugeTransform, m_maxScale);
            var worldToLocal = m_gaugeTransform.localPosition - m_gaugeTransform.position;
            if (m_align == ALIGN.BOTTOM)
            {
                m_posAtZero = m_gaugeSpr.bounds.min.y + worldToLocal.y;
            }
            else if (m_align == ALIGN.TOP)
            {
                m_posAtZero = m_gaugeSpr.bounds.max.y + worldToLocal.y;
            }
            posAtMax = m_gaugeSpr.bounds.center.y + worldToLocal.y;
        }
        //Range from minimum position to max ( length of the demi gauge )
        m_posRange = posAtMax - m_posAtZero;
        m_initPos = m_gaugeTransform.localPosition;
    }
    
	/** Set value between 0 and 1*/
	virtual public void SetValue(float _value){

        if (m_gaugeTransform == null)
        {
            return;
        }

        //Cap values
		if (_value < 0.0f) _value = 0.0f;
		if (_value > 1.0f) _value = 1.0f;

        m_currentValue = _value;

        float newScale = _value * m_maxScale;
        float newPos = m_posAtZero + m_posRange * m_currentValue;

        //change scale
        if ( m_orientation == ORIENTATION.HORIZONTAL){
			Utils.SetLocalScaleX( m_gaugeTransform, newScale );
            Utils.SetLocalPositionX(m_gaugeTransform, newPos);
        }
        else{			
			Utils.SetLocalScaleY( m_gaugeTransform, newScale );
            Utils.SetLocalPositionY(m_gaugeTransform, newPos);
        }
    }
    
}

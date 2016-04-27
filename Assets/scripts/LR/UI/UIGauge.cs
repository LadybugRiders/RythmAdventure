using UnityEngine;
using System.Collections;

public class UIGauge : MonoBehaviour {

	[SerializeField] protected float m_currentValue = 1.0f;

	public enum Orientation { HORIZONTAL, VERTICAL};
	[SerializeField] protected Orientation m_orientation;

	public enum ALIGN { UP, DOWN, LEFT, RIGHT, CENTER };
	[SerializeField] protected ALIGN m_align;

	[SerializeField] protected SpriteRenderer m_gaugeSpr;
	[SerializeField] SpriteRenderer m_backgroundSpr;

	[SerializeField] protected float m_maxScale = 1.0f;
	[SerializeField] protected bool m_useSprScaleAsMax = false;

	/** Origin position value of the edge of the gauge */
	private float m_initPos = 0;

	protected Transform m_gaugeTransform;
	//protected float m_gaugeLength;

	// Use this for initialization
	virtual protected void Awake () {

        if (m_gaugeSpr != null)
        {
            m_gaugeTransform = m_gaugeSpr.transform;

            //Override max scale if needed
            if (m_useSprScaleAsMax)
            {
                m_maxScale = m_orientation == Orientation.HORIZONTAL ? m_gaugeTransform.localScale.x : m_gaugeTransform.localScale.y;
            }
            m_initPos = m_orientation == Orientation.HORIZONTAL ? m_gaugeTransform.localPosition.x : m_gaugeTransform.localPosition.y;
        }

		//m_gaugeLength = m_gaugeSpr.sprite.bounds.extents.x * 2;

		SetValue (m_currentValue);
	}

    int count = 0;
	// Update is called once per frame
	void Update () {
        count++;
        if (count > 30)
        {
            SetValue(m_currentValue - 0.03f);
            count = 0;
        }
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

        //keep old scale
        //float oldScale = m_orientation == Orientation.HORIZONTAL ? m_gaugeTransform.localScale.x : m_gaugeTransform.localScale.y;

        float newScale = _value * m_maxScale;

        //change scale
        if ( m_orientation == Orientation.HORIZONTAL){
			Utils.SetLocalScaleX( m_gaugeTransform, newScale );
            SetHorizontal(newScale);
		}else{			
			Utils.SetLocalScaleY( m_gaugeTransform, newScale );
            SetVertical(newScale);
		}
	}

	void SetHorizontal(float newScale)
    {
        var offset = (m_maxScale - newScale);
        if ( m_align == ALIGN.LEFT) {
            Utils.SetLocalPositionX(m_gaugeTransform ,m_initPos - offset);
		}else if( m_align == ALIGN.RIGHT)
        {
            Utils.SetLocalPositionX(m_gaugeTransform, m_initPos + offset);
        }
	}

    void SetVertical(float newScale)
    {
        var offset = (m_maxScale - newScale);
        if (m_align == ALIGN.UP)
        {
            Utils.SetLocalPositionY(m_gaugeTransform, m_initPos + offset);
        }else if (m_align == ALIGN.DOWN )
        {
            Utils.SetLocalPositionY(m_gaugeTransform, m_initPos + offset);
        }
    }
}

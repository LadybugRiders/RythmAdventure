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
	[SerializeField] protected bool m_useGaugeScale = false;

	/** Origin position value of the edge of the gauge */
	//private float m_initPos = 0;
	protected Transform m_gaugeTransform;
	//protected float m_gaugeLength;

	// Use this for initialization
	virtual protected void Awake () {
		if (m_gaugeSpr)
			m_gaugeTransform = m_gaugeSpr.transform;
		if (m_useGaugeScale) {
			m_maxScale = m_orientation == Orientation.HORIZONTAL ? m_gaugeSpr.transform.localScale.x : m_gaugeSpr.transform.localScale.y;
		}

		//m_gaugeLength = m_gaugeSpr.sprite.bounds.extents.x * 2;

		SetValue (m_currentValue);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/** Set value between 0 and 1*/
	virtual public void SetValue(float _value){

		if (m_gaugeTransform == null)
			Debug.Log ("bite");

		if (_value < 0.0f) _value = 0.0f;
		if (_value > 1.0f) _value = 1.0f;

		if( m_orientation == Orientation.HORIZONTAL){
			Utils.SetLocalScaleX(m_gaugeTransform,  _value * m_maxScale);
		}else{			
			Utils.SetLocalScaleY(m_gaugeTransform,  _value * m_maxScale);
		}
	}

	void SetHorizontal(){
		if ( m_align == ALIGN.LEFT) {

		}
	}
}

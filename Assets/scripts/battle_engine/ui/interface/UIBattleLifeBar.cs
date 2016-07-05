using UnityEngine;
using System.Collections;

public class UIBattleLifeBar : UIGauge {

	[SerializeField] bool m_isMana = false;
    [SerializeField] SpriteRenderer m_aroundSprite;
    float m_aroundRatio = 1.1f;

	Color m_baseColor;
	TweenEngine.Tween m_tweenBlink;

	override protected void Awake(){
		m_baseColor = m_gaugeSpr.color;
        if( m_aroundSprite)
        {
            if( m_orientation == ORIENTATION.HORIZONTAL)
				m_aroundRatio = m_aroundSprite.transform.localScale.x / m_gaugeSpr.transform.localScale.x;
            else
				m_aroundRatio = m_aroundSprite.transform.localScale.y / m_gaugeSpr.transform.localScale.y;
		}
		base.Awake ();
	}

	override public void SetValue(float _value, bool _fill = false, float _fillDuration = 1.0f, int _fillCount = 0)
    {
		base.SetValue (_value);

        //blinking
		if (_value >= 1.0f && m_isMana) {
			m_tweenBlink = TweenEngine.instance.ColorTo (m_gaugeSpr, Color.white, 0.5f, true, int.MaxValue, null);
		} else if (m_tweenBlink != null) {
			m_tweenBlink.Stop(false);
			m_tweenBlink = null;
			m_gaugeSpr.color = m_baseColor;
		}
        if( m_aroundSprite != null)
        {
            if (m_orientation == ORIENTATION.HORIZONTAL)
            {
                Utils.SetLocalScaleX(m_aroundSprite.transform, m_gaugeSpr.transform.localScale.x * m_aroundRatio);
				Utils.SetLocalPositionX (m_aroundSprite.transform, m_gaugeSpr.transform.localPosition.x);
            }
            else
            {
				Utils.SetLocalScaleY(m_aroundSprite.transform, m_gaugeSpr.transform.localScale.y * m_aroundRatio);
				Utils.SetLocalPositionY (m_aroundSprite.transform, m_gaugeSpr.transform.localPosition.y);
            }
        }
    }


    public bool IsMana
    {
        get { return m_isMana; }
    }
}

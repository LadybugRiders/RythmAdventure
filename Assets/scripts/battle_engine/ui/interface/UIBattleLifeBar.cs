using UnityEngine;
using System.Collections;

public class UIBattleLifeBar : UIGauge {

	[SerializeField] bool m_isMana = false;
	Color m_baseColor;
	TweenEngine.Tween m_tweenBlink;

	override protected void Awake(){
		base.Awake ();
		m_baseColor = m_gaugeSpr.color;
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
	}


    public bool IsMana
    {
        get { return m_isMana; }
    }
}

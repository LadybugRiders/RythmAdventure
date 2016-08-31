using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleSlotExplosion : MonoBehaviour {

	[SerializeField] SpriteRenderer m_explosion;

	[SerializeField] float m_minScale = 1.0f;
	[SerializeField] float m_maxScale = 2.5f;
	[SerializeField] float m_longScale = 2.0f;

	TweenEngine.Tween m_currentLongTween;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play(BattleNote _note)
    {
        Stop();

        if (_note.Type == NoteData.NoteType.SIMPLE) {
			PlaySimple();
		} else if (_note.Type == NoteData.NoteType.LONG) {
			BattleNoteLong nL = _note as BattleNoteLong;
			if( nL.IsHead ){
				PlayLong();
			}else{		
				PlaySimple();
			}
		}

	}

	void PlaySimple(){
        Debug.Log("PLAYSIMPLE");
        //Reset transform
        m_explosion.transform.localScale = new Vector3 (m_minScale, m_minScale, 1);
		Utils.SetLocalAngleZ (m_explosion.transform, 0.0f);
		//Tween
		TweenEngine.Tween tween = TweenEngine.instance.ScaleTo (m_explosion.transform, new Vector3(m_maxScale,m_maxScale,1), 0.25f,"OnTweenEnd" );
		TweenEngine.instance.RotateAroundZTo (m_explosion.transform, 30.0f, 0.3f );
		tween.CallbackObject = this.gameObject;
	}

	void PlayLong()
    {
        Debug.Log("PLAYLONG");
        //Reset transform
        m_explosion.transform.localScale = new Vector3 (m_minScale, m_minScale, 1);
		Utils.SetLocalAngleZ (m_explosion.transform, 0.0f);
		//Tween
		//TweenEngine.instance.ScaleTo (m_explosion.transform, new Vector3(m_longScale,m_longScale,1), 0.1f );

		m_currentLongTween = TweenEngine.instance.RotateAroundZTo (m_explosion.transform, 360.0f, 0.8f,false,int.MaxValue,null );
		m_currentLongTween.CallbackObject = this.gameObject;
	}

	public void Stop(){
        Debug.Log(name + " slot STOP. tween:" + m_currentLongTween );
		if (m_currentLongTween != null) {            
			m_currentLongTween.Stop(true);
            m_currentLongTween = null;
        }
        m_explosion.transform.localScale = new Vector3(0, 0, 1);
        Debug.Log("TransformSet" + m_explosion.transform.localScale.ToString());
    }
    
    void OnTweenEnd(object _o){
        Stop();
	}
}

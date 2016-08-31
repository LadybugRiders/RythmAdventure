using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleSlotExplosion : MonoBehaviour {

	//[SerializeField] SpriteRenderer m_renderer;

	[SerializeField] float m_minScale = 1.0f;
	[SerializeField] float m_maxScale = 2.5f;
	[SerializeField] float m_longScale = 2.0f;

	TweenEngine.Tween m_currentLongTween;
    Transform m_transform;

    void Awake()
    {
        m_transform = transform;
    }

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
        //Reset transform
        transform.localScale = new Vector3 (m_minScale, m_minScale, 1);
		Utils.SetLocalAngleZ (transform, 0.0f);
		//Tween
		TweenEngine.Tween tween = TweenEngine.instance.ScaleTo (m_transform, new Vector3(m_maxScale,m_maxScale,1), 0.25f,"OnTweenEnd" );
		TweenEngine.instance.RotateAroundZTo (m_transform, 30.0f, 0.3f );
		tween.CallbackObject = this.gameObject;
	}

	void PlayLong()
    {
        //Reset transform
        m_transform.localScale = new Vector3 (m_minScale, m_minScale, 1);
		Utils.SetLocalAngleZ (m_transform, 0.0f);
		//Tween
		//TweenEngine.instance.ScaleTo (m_explosion.transform, new Vector3(m_longScale,m_longScale,1), 0.1f );

		m_currentLongTween = TweenEngine.instance.RotateAroundZTo (m_transform, 360.0f, 0.8f,false,int.MaxValue,null );
		m_currentLongTween.CallbackObject = this.gameObject;
	}

	public void Stop(){
		if (m_currentLongTween != null) {            
			m_currentLongTween.Stop(true);
            m_currentLongTween = null;
        }
        m_transform.localScale = new Vector3(0, 0, 1);
    }
    
    void OnTweenEnd(object _o){
        Stop();
	}
}

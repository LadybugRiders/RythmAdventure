using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleSlotExplosion : MonoBehaviour {

	[SerializeField] List<SpriteRenderer> m_explosions;
	List<bool> m_launchedList;

	[SerializeField] float m_minScale = 1.0f;
	[SerializeField] float m_maxScale = 2.5f;
	[SerializeField] float m_longScale = 2.0f;

	TweenEngine.Tween m_currentLongTween;

	// Use this for initialization
	void Start () {
		m_launchedList = new List<bool> ();
		for (int i=0; i < m_explosions.Count; i++) {
			m_launchedList.Add(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play(BattleNote _note){
		int i = GetFreeExplosionIndex ();
		Transform t = m_explosions [i].transform;

		if (_note.Type == NoteData.NoteType.SIMPLE) {
			PlaySimple(t);
		} else if (_note.Type == NoteData.NoteType.LONG) {
			BattleNoteLong nL = _note as BattleNoteLong;
			if( nL.IsHead ){
				PlayLong(t);
			}else{		
				if( m_currentLongTween != null)
					Stop();
				PlaySimple(t);
			}
		}

	}

	void PlaySimple(Transform _t){		
		//Reset transform
		_t.localScale = new Vector3 (m_minScale, m_minScale, 1);
		Utils.SetLocalAngleZ (_t, 0.0f);
		//Tween
		TweenEngine.Tween tween = TweenEngine.instance.ScaleTo (_t, new Vector3(m_maxScale,m_maxScale,1), 0.25f,"OnTweenEnd" );
		TweenEngine.instance.RotateAroundZTo (_t, 30.0f, 0.3f );
		tween.CallbackObject = this.gameObject;
	}

	void PlayLong(Transform _t){
		//Reset transform
		_t.localScale = new Vector3 (m_minScale, m_minScale, 1);
		Utils.SetLocalAngleZ (_t, 0.0f);
		//Tween
		TweenEngine.instance.ScaleTo (_t, new Vector3(m_longScale,m_longScale,1), 0.1f );

		m_currentLongTween = TweenEngine.instance.RotateAroundZTo (_t, 360.0f, 0.8f,false,int.MaxValue,"OnTweenEnd" );
		m_currentLongTween.CallbackObject = this.gameObject;
	}

	public void Stop(){
		if (m_currentLongTween != null) {
			m_currentLongTween.Stop(true);
			m_currentLongTween = null;
		}
	}

	int GetFreeExplosionIndex(){
		return 0;
	}

	void OnTweenEnd(object _o){
		GameObject go = (GameObject)_o;
		go.transform.localScale = new Vector3 (m_minScale, m_minScale, 1);
	}
}

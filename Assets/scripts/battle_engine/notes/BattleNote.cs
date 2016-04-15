using UnityEngine;
using System.Collections;

public class BattleNote : MonoBehaviour {
	protected NoteData m_data;
	protected float m_speed;

	public enum State { LAUNCHED, HIT, MISS, DEAD };
	protected State m_state = State.DEAD;

	/** If set to false, the note is hit by releasing the input */
	[SerializeField] protected bool m_hitPress = true;

	[SerializeField] protected NoteData.NoteType m_type = NoteData.NoteType.SIMPLE;

	[SerializeField] protected Sprite m_attackSprite;
	[SerializeField] protected Sprite m_defendSprite;
	
	/** Sprite that follow the note if it is a magic note */
	[SerializeField] protected SpriteRenderer m_magicEffect;

	protected BattleTrack m_track;

	//references to components used in updates
	protected SpriteRenderer m_renderer;
	protected Transform m_transform;

	/** Distance done by the note from its starting point */
	protected float m_distanceDone = 0;

	/** Distance where the alpha of the note will reach 1.Of */
	protected float m_alphaDist = 5.0f;

	protected Vector3 m_startPos;

	protected bool m_paused = false;

	// Use this for initialization
	virtual protected void Start () {
		m_renderer = GetComponent<SpriteRenderer> ();
		m_transform = transform;
		Die ();
	}
	
	// Update is called once per frame
	virtual protected void Update () {
		if (m_paused)
			return;
		switch (m_state) {
			case State.LAUNCHED : 
					UpdateSpeed();
					UpdateAlpha();
				break;
		}
	}

	#region UPDATES

	protected void UpdateSpeed(){
		Vector3 pos = m_transform.localPosition;

		//make the note advance
		float stepX = m_speed * Time.deltaTime;
		pos.x += stepX ;
		//compute total distance done
		m_distanceDone += Mathf.Abs( stepX );

		m_transform.localPosition = pos;

		//Effect
		if (m_magicEffect != null && m_data.Subtype == NoteData.NoteSubtype.MAGIC) {
			m_magicEffect.transform.position = m_transform.position;
			Utils.SetPositionZ( m_magicEffect.transform, m_transform.position.z +1);
		}
	}

	protected void UpdateAlpha(){
		//compute alpha from the beginning
		float newAlpha = (m_distanceDone / m_alphaDist) * 1.0f;
		Utils.SetAlpha (m_renderer, newAlpha);

		//Apply on Magic Effect
		if (m_magicEffect != null && m_data.Subtype == NoteData.NoteSubtype.MAGIC) {
			Utils.SetAlpha( m_magicEffect,newAlpha );
		}
	}


	#endregion

	public bool Launch(float _speed, Vector3 _startPos, BattleTrack _track){	
		m_track = _track;
		transform.position = _startPos;
		m_startPos = m_transform.localPosition;
		m_speed = _speed;
		m_distanceDone = 0;
		return Launch ();
	}

	virtual protected bool Launch(){
		this.CurrentState = State.LAUNCHED;
		//set sprite & color
		if (m_track.TracksManager.IsAttacking) {
			m_renderer.sprite = m_attackSprite;
			m_renderer.color = m_track.TracksManager.attackColor;
		} else {
			m_renderer.sprite = m_defendSprite;
			m_renderer.color = m_track.TracksManager.defendColor;
		}
		//magic note
		if (m_data.Subtype == NoteData.NoteSubtype.MAGIC) {
			//m_renderer.color = Color.yellow;
		}

		Utils.SetAlpha (m_renderer, 0.0f);
		return true;
	}

	/** Hit the note */
	virtual public void Hit(BattleSlot _slot){
		this.CurrentState = State.HIT;
		Die ();
	}

	virtual public BattleNote[] Miss(){
		this.CurrentState = State.MISS;
		Die ();
		return new BattleNote[]{ this };
	}

	/** Makes the note die if needs to be. If the note can be killed, return true */
	virtual public bool Die(){
		this.CurrentState = State.DEAD;
		Utils.SetLocalPositionY (m_transform,-10000);
		Utils.SetAlpha (m_renderer, 0.0f);
		if (m_magicEffect)
			Utils.SetAlpha (m_magicEffect, 0.0f);
		return true;
	}

	public void Pause(){
		if (m_state == State.LAUNCHED) {
			m_paused = true;
		}
	}

	public void Resume(){
		m_paused = false;
	}

	void EnableMagicEffect(){

	}

	#region GETTERS_SETTERS

	public NoteData.NoteType Type {
		get {
			return m_type;
		}
	}

	public BattleNote.State CurrentState {
		get {
			return m_state;
		}
		set {
			m_state = value;
		}
	}

	virtual public bool IsOnTrack {
		get{
			return m_state == State.LAUNCHED;
		}
	}

	virtual public bool IsHittable {
		get{
			return m_state == State.LAUNCHED;
		}
	}

	public bool IsDead{
		get{
			return m_state == State.DEAD;
		}
	}

	public bool HitPress {
		get {
			return m_hitPress;
		}
	}

	public float Speed{
		get{
			return m_speed;
		}
	}

	virtual public BattleNote GetPairNote(){
		return null;
	}

	public NoteData Data{
		get{
			return m_data;
		}
		set{
			m_data = value;
		}
	}

	#endregion
}

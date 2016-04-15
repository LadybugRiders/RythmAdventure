using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleSlot : MonoBehaviour {

	public bool m_attack = false;
	public bool m_active = true;

	[SerializeField] protected BattleTrack m_track;
	[SerializeField] protected List<BattleSlotTextAccuracy> m_textsAcc;
	[SerializeField] protected BattleSlotExplosion m_explosion;

	protected float m_diameter;

	/** Notes currently colliding with the slot */
	private List<BattleNote> m_collidingNotes;

	//Accuacy Text
	protected SpriteRenderer m_textSprite;

	//Long note
	protected BattleNoteLong m_currentLongNote;

	// Use this for initialization
	void Start () {
		m_collidingNotes = new List<BattleNote> ();
		ComputeDiameter ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Activate(){
		m_active = true;
	}

	public void Deactivate(){
		m_active = false;
		m_collidingNotes.Clear ();
	}

	public void OnInputHit( bool _down){
		if (m_active == false ) 
			return;
		//if no note is colliding, send an error to BattleTrack
		if (m_collidingNotes.Count <= 0) {
			m_track.OnInputError(_down);
			m_explosion.Stop();
			return;
		}

		//else we hit the first note that has collided
		BattleNote note = m_collidingNotes [0];
		//check input method of the note
		if ( note.HitPress != _down) {
			m_track.OnInputError(_down);
			m_explosion.Stop();
			return;
		}

		//hit note and compute accuracy
		float accuracy = ComputeAccuracy (note);
		m_track.OnNoteHit (note, accuracy, this);
		m_collidingNotes.RemoveAt (0);

		//play explosion
		m_explosion.Play (note);
	}

	#region COLLISIONS

	void OnTriggerEnter2D(Collider2D _collider){
		if (m_active == false ) 
			return;
		if( _collider.gameObject.layer == 8 ){
			BattleNote note = _collider.gameObject.GetComponent<BattleNote>();
			if( note ){
				//Debug.Log( "Adding New note");
				m_collidingNotes.Add(note);
			}
		}
	}

	void OnTriggerExit2D(Collider2D _collider){
		if (m_active == false ) 
			return;
		if( _collider.gameObject.layer == 8 ){
			BattleNote note = _collider.gameObject.GetComponent<BattleNote>();
			if( note && note.IsHittable && m_collidingNotes.Contains(note)){
				m_track.OnNoteMiss(note);
				m_collidingNotes.Remove(note);
			}
		}
	}

	#endregion

	public void PlayTextAccuracy(BattleScoreManager.Accuracy _accuracy){
		//Find dead text
		for (int i=0; i < m_textsAcc.Count; i++) {
			if( m_textsAcc[i].IsAvailable ){
				m_textsAcc[i].Play(_accuracy);
				return;
			}
		}
	}

	float ComputeAccuracy(BattleNote _note){
		//distance between the note and slot centers
		float diff = _note.transform.position.x - transform.position.x;
		float delta = Mathf.Abs( diff );

		//add accuracy for notes past the slot. This prevents illusions for the eye because of the speed
		bool attacking = m_track.TracksManager.IsAttacking;
		if( (attacking && diff > 0) || ( !attacking && diff < 0) ){
			delta = delta - (m_diameter * 0.1f );
		}

		//compute accuracy
		float percent = delta / m_diameter;

		return 100 - (percent * 100);
	}

	/** Compute Accuracy Values */
	void ComputeDiameter(){		
		CircleCollider2D coll = GetComponent<CircleCollider2D> ();
		m_diameter = coll.radius * transform.localScale.x * 2 ;
	}
}

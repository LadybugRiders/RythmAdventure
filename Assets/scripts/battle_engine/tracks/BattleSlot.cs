using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleSlot : MonoBehaviour {

	public bool m_attack = false;
	public bool m_active = true;

	[SerializeField] protected BattleTrack m_track;
	[SerializeField] protected List<BattleSlotTextAccuracy> m_textsAcc;
	[SerializeField] protected BattleSlotExplosion m_explosion;

    [SerializeField] protected float m_slideErrorDelay = 0.6f;
    bool m_errorPending = false;
    BattleNote.HIT_METHOD m_errorInputMethod;

	/// <summary>
	/// Note on which the error is pending
	/// </summary>
	BattleNote m_errorPendingNote; 

    BattleNote.HIT_METHOD m_lastInputMethod = BattleNote.HIT_METHOD.RELEASE;    

	protected float m_diameter;

	/** Notes currently colliding with the slot */
	private List<BattleNote> m_collidingNotes;

	//Accuacy Text
	protected SpriteRenderer m_textSprite;
    
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

	public void ResetInput(){
		m_lastInputMethod = BattleNote.HIT_METHOD.NONE;
	}

	public void OnInputTriggered(BattleNote.HIT_METHOD _method){

		if (m_active == false ) 
			return;
		//if no long note is currently being hit, an error shouldn't be send ( just releasing after a hit/swipe )
		if (_method == BattleNote.HIT_METHOD.RELEASE && CurrentLongNote == null)
			return;
		//if a slide is done but no press, this is a remain from a previous track input
		if (_method == BattleNote.HIT_METHOD.SLIDE && m_lastInputMethod == BattleNote.HIT_METHOD.NONE) {
			Debug.Log ("SLIDE NOT PRESS");
			return;
		}

		//if no note is colliding (miss)
		if (m_collidingNotes.Count <= 0) {            
            //send an error to BattleTrack
			Debug.Log("ERROR no note");
			ApplyError(_method);
			m_lastInputMethod = _method;
			return;
		}

		//else we hit the first note that has collided
		BattleNote note = m_errorPendingNote != null ? m_errorPendingNote : GetFirstAliveCollidingNote();
		//Return if not is not active (launched)
		if (note.CurrentState != BattleNote.State.LAUNCHED) {
			return;
		}

        //check input method of the note
        if ( note.HitMethod != _method ) {
            // If we expect a slide and a press is done, wait a bit for a potential slide before erroring
            if( note.HitMethod == BattleNote.HIT_METHOD.SLIDE && _method == BattleNote.HIT_METHOD.PRESS)
            {
                LaunchPendingError(_method,note);
            }
            else
            {
                ApplyError(_method);
			}
			Debug.Log ("1" + note.ToString() +" "+ note.HitMethod.ToString() + " " + _method);
			m_lastInputMethod = _method;
            return;
		}

        //Check slide, aborting an error if a hit is pending
        //=> a hit has been done, but we didn't know if it was the beginning of a slide or not. Now we're sure, so NO need to raise an error
        if( m_errorPending && _method == BattleNote.HIT_METHOD.SLIDE )
        {
            AbortPendingError();
        }

		//hit note and compute accuracy
		float accuracy = ComputeAccuracy (note);
		m_track.OnNoteHit (note, accuracy, this);
		m_collidingNotes.RemoveAt (0);

		//play explosion
		m_explosion.Play (note);
		m_errorPendingNote = null;
		m_lastInputMethod = _method;
	}

    #region ERROR_HANDLING

	public void LaunchPendingError(BattleNote.HIT_METHOD _method, BattleNote _note)
    {        
        //clean just in case
        TimerEngine.instance.StopAll("OnPendingErrorTimerOver", this.gameObject);
        m_errorPending = true;
		m_errorPendingNote = _note;
        m_errorInputMethod = _method;
        TimerEngine.instance.AddTimer(m_slideErrorDelay, "OnPendingErrorTimerOver", this.gameObject);
    }    

    public void OnPendingErrorTimerOver()
    {
        if (m_errorPending)
        {
			ApplyError(m_errorInputMethod,m_errorPendingNote);
        }
    }

    public void AbortPendingError()
    {
        m_errorPending = false;
		m_errorPendingNote = null;
        //clean timers
        TimerEngine.instance.StopAll("OnPendingErrorTimerOver", this.gameObject);
    }

	public void ApplyError(BattleNote.HIT_METHOD _method, BattleNote _note = null)
	{
		m_errorPending = false;
		Debug.Log ("error"+ this.gameObject.name+ " " + _method + m_lastInputMethod + " " + ( _note != null ? _note.HitMethod.ToString() : "") );

		m_track.OnInputError(m_errorInputMethod, _note);

		//Reset
		m_lastInputMethod = BattleNote.HIT_METHOD.NONE;
		m_errorPendingNote = null;

        m_explosion.Stop();
    }

    #endregion

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

	BattleNote GetFirstAliveCollidingNote(){
		foreach (var note in m_collidingNotes) {
			if (!note.IsDead) {
				return note;
			}
		}
		return null;
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
    ///<summary>
	/// Compute Accuracy Values 
    /// </summary>
	void ComputeDiameter(){		
		CircleCollider2D coll = GetComponent<CircleCollider2D> ();
		m_diameter = coll.radius * transform.localScale.x * 2 ;
	}

    public BattleNoteLong CurrentLongNote
    {
        get { return m_track.CurrentLongNote; }
    }
}

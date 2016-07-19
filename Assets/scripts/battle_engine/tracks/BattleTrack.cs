using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleTrack : MonoBehaviour {

	//SLOTS
	public BattleSlot m_slotDefend;
	public BattleSlot m_slotAttack;
	protected BattleSlot m_currentSlot;

	[SerializeField] protected BattleTracksManager m_manager;
	[SerializeField] protected int m_id = 0;

	//Notes currently alive in the track
	protected List<BattleNote> m_notes;

	// Length of the track, in units (distance between slots)
	protected float m_length;

	//Current long playing note
	protected BattleNoteLong m_currentLongNote = null;

	private int m_iteration = 1;
	protected bool m_enabled = true;
	protected bool m_suspended = false;

	/* notes under this time won't be launched while the track is suspended */
	protected float m_suspendedTimeLimit = 0.0f;

	//AUDIO
	List<AudioSource> m_audioNoteHitSources;

	public void Awake(){		
		m_currentSlot = m_slotDefend;
		CreateAudio ();
		m_length = Mathf.Abs(m_slotAttack.transform.position.x - m_slotDefend.transform.position.x );
	}

	// Use this for initialization
	void Start () {
		m_notes = new List<BattleNote>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SwitchPhase(){
		if (m_manager.PhaseState == BattleTracksManager.BattleState.ATTACK) {
			m_slotAttack.Activate();
			m_slotDefend.Deactivate();
			m_currentSlot = m_slotAttack;
		} else {
			m_currentSlot = m_slotDefend;
			m_slotDefend.Activate();
			m_slotAttack.Deactivate ();
		}
	}

	/** Launch note on the track */
	public bool LaunchNote(BattleNote _note, float _speed){
		if (m_suspended) {
			if( m_suspendedTimeLimit >= _note.Data.TimeBegin){
				return false;
			}
		}
		Vector3 pos = new Vector3 ();
		//place the note at the right x according to state
		if (m_manager.IsAttacking) {
			pos.x = m_slotDefend.transform.position.x;
		} else {
			pos.x = m_slotAttack.transform.position.x;
		}
		//place y
		pos.y = this.transform.position.y;
		pos.z = -1;

		bool success = _note.Launch (_speed, pos, this);
		if( success )
			m_notes.Add (_note);
		return success;
	}
    
    public void OnInputTriggered(BattleNote.HIT_METHOD method)
    {
        m_currentSlot.OnInputTriggered(method);
    }

	#region NOTE_EVENTS

	/// <summary>
	/// Hit a note. Use forceRemove to force the removal from the track list
	/// </summary>
	public void OnNoteHit(BattleNote _note, BattleSlot _slot, bool _forceRemove = false){
		if (_forceRemove) {
			OnNoteKill (_note, _slot);
		}
		//add note to the manager
		BattleScoreManager.Accuracy acc = m_manager.AddNote (_note);
		//kill note
		_note.Hit (_slot);
		//play text on slot
		m_currentSlot.PlayTextAccuracy (acc);
		
		CheckLongNoteHit (_note);

		//Audio
		PlayAudio (_note);

		var noteEvent = new BattleTracksManager.NoteEventInfo (_note.Data, true, acc, _note.IsFinal);
		m_manager.RaiseNoteEvent(noteEvent);
    }

	/// <summary>
	/// Use this trigger the action induced by hitting the note
	/// </summary>
	public void OnNoteTriggerAction(BattleNote _note, BattleSlot _slot, bool _isMagic){
		//add note to the manager
		BattleScoreManager.Accuracy acc = m_manager.AddNote (_note);

		var noteEvent = new BattleTracksManager.NoteEventInfo (_note.Data, true, acc, _note.IsFinal);
		noteEvent.IsMagic = _isMagic;		
		m_manager.RaiseNoteActionEvent (noteEvent);

		OnNoteKill (_note, _slot);
	}

	/// <summary>
	/// Use this to kill a note
	/// </summary>
	public void OnNoteKill(BattleNote _note, BattleSlot _slot){		
		m_notes.Remove (_note);
		_note.Die ();
	}
		
	/// <summary>
	/// Called directly by BattleSlot if a note is missed ( went past the slot )
	/// Or from BattleTrack.OnInputError ( before the note hits the slot) 
	/// </summary>
	public void OnNoteMiss(BattleNote _note){
		//miss note and gather notes to delete with it
		BattleNote[] notesToDelete = _note.Miss ();

        //remove note induced by the miss (in case of a long note, we want to delete its head & tail)
        foreach(var note in notesToDelete)
            m_notes.Remove (note);

		//add note to the manager
		BattleScoreManager.Accuracy acc = m_manager.AddNote (_note);

		//raise note miss event
		m_manager.RaiseNoteEvent(new BattleTracksManager.NoteEventInfo(_note.Data, false, BattleScoreManager.Accuracy.MISS,_note.IsFinal));

		//play text on slot
		m_currentSlot.PlayTextAccuracy (acc);
		m_currentLongNote = null;
	}
    
    ///<summary>
	/// Called from a slot when the input is pressed but no note is hit
	///</summary>
	public void OnInputError(BattleNote.HIT_METHOD method, BattleNote _note){
		if (_note != null && _note.IsDead) {
			return;
		}
		bool noteMissed = false;
		//input is released
		if (method == BattleNote.HIT_METHOD.RELEASE) {
			//and a long note is ongoing ( but it didn't get hit )
			if (m_currentLongNote) {
				noteMissed = true;
			}
		//input is down but nothing gets hit => miss
		} else {
			noteMissed = true;
		}

		//if there are notes on the track ( the other case shouldn't happen => we would have changed tracks )
        if (m_notes.Count > 0)
        {
			//if we dont have an active note to error, take the next one on the track
			if (_note == null)
				_note = m_notes [0];
			
            if (noteMissed)
			{
				//m_manager.RaiseNoteEvent(new BattleTracksManager.NoteEventInfo(m_notes[0].Data, false));
				OnNoteMiss(_note);
            }
            else
            {
				m_manager.RaiseNoteEvent(new BattleTracksManager.NoteEventInfo(_note.Data, false));
            }
        }            
        
    }

	public void OnLaunchAction( BattleTracksManager.NoteEventInfo _noteInfo ){

	}

	#endregion

	#region LONG_NOTES

	/** Check current Long Note and affect m_currentLongNote */
	void CheckLongNoteHit(BattleNote _note){
		//keep note if the note is long and the head
		if (_note.Type == NoteData.NoteType.LONG) {
			BattleNoteLong noteLong = (BattleNoteLong)_note;
			if (noteLong.IsHead)
				m_currentLongNote = noteLong;
		} else {
			m_currentLongNote = null;
		}
	}

	#endregion

	#region AUDIO

	void CreateAudio(){
		m_audioNoteHitSources = new List<AudioSource>();
		for(int i=0; i < 2; i++){
			AudioSource src = gameObject.AddComponent<AudioSource> ();
			src.playOnAwake = false;
			m_audioNoteHitSources.Add( src );
		}
	}

	void PlayAudio(BattleNote _note){
		AudioSource src = m_audioNoteHitSources [0];
		if (_note.Data.Type == NoteData.NoteType.LONG && !_note.Data.Head) {			
			src.clip = BattleSoundEngine.instance.noteLongTail.clip;
			src.volume = BattleSoundEngine.instance.noteLongTail.volume;
		} else {			
			src.clip = BattleSoundEngine.instance.noteSimple.clip;
			src.volume = BattleSoundEngine.instance.noteSimple.volume;
		}
		src.Play ();
	}

	#endregion

	#region PAUSE

	public void Pause(){
		for (int i=0; i< m_notes.Count; i++) {
			m_notes[i].Pause();
		}
	}

	public void Resume(){
		for (int i=0; i< m_notes.Count; i++) {
			m_notes[i].Resume();
		}
	}

	#endregion

	public void ResetInput(){
		m_slotAttack.ResetInput ();
		m_slotDefend.ResetInput ();
	}

	/// <summary>
	/// disable the track by setting all current notes to final mode. Returns true if the track is clear of all notes.
	/// </summary>
	public bool Disable(){
		if (m_enabled) {
			m_enabled = false;
			for (int i = m_notes.Count - 1; i >= 0; i--) {
				m_notes [i].IsFinal = true;
			}
		}
		return IsEmpty;
	}

	public BattleTracksManager TracksManager {
		get {
			return m_manager;
		}
	}

	public List<BattleNote> NotesOnTrack{
		get{
			return m_notes;
		}
	}

	public bool IsEmpty{
		get{ return NotesOnTrack.Count == 0; }
	}

    /// <summary>
    /// The current Note of the track. This note cannot be Dead (hit).
    /// </summary>
	public BattleNote CurrentNote{
		get{
			if( m_notes == null || m_notes.Count <= 0 )
				return null;
            foreach (var note in m_notes)
                if (!note.IsDead)
                    return note;
            return null;
        }
	}

    public BattleNoteLong CurrentLongNote
    {
        get { return m_currentLongNote; }
    }

	public int Id {
		get {
			return m_id;
		}
		set{
			m_id = value;
		}
	}

	public int Iteration {
		get {
			return m_iteration;
		}
		set {
			m_iteration = value;
		}
	}

	public float Length {
		get {
			return m_length;
		}
	}

	public bool IsActive {
		get {
			return m_enabled;
		}
	}

}

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
    
    public void OnInputHit(BattleNote.HIT_METHOD method)
    {
        m_currentSlot.OnInputHit(method);
    }

	#region NOTE_EVENTS

	public void OnNoteHit(BattleNote _note, float _accuracy, BattleSlot _slot){
		m_notes.Remove (_note);
		//add note to the manager
		BattleScoreManager.Accuracy acc = m_manager.AddNote (_note, _accuracy);

		_note.Hit (_slot);
		//play text on slot
		m_currentSlot.PlayTextAccuracy (acc);
		
		CheckLongNoteHit (_note);

		//Audio
		PlayAudio (_note);
	}

	/** Called directly by BattleSlot if a note is missed ( went past the slot )
	 * Or from BattleTrack.OnInputError ( before the note hits the slot) */
	public void OnNoteMiss(BattleNote _note){
		//miss note and gather notes to delete with it
		BattleNote[] toDelete = _note.Miss ();
		//remove notes induced by the miss
		for(int i=0; i < toDelete.Length; i ++)
			m_notes.Remove (toDelete[i]);

		//add note to the manager
		BattleScoreManager.Accuracy acc = m_manager.AddNote (_note, -100);
		//play text on slot
		m_currentSlot.PlayTextAccuracy (acc);
		m_currentLongNote = null;
	}

	/** Called from a slot when the input is pressed but no note is hit
	 * Param _down tells if the input is pressed down */
	public void OnInputError(BattleNote.HIT_METHOD method){
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

		//Delete if needed
		if (noteMissed && m_notes.Count > 0) {
			OnNoteMiss (m_notes [0]);
		}
	}

	#endregion

	#region LONG_NOTES

	/** Check current Long Note and affect m_currentLongNote */
	void CheckLongNoteHit(BattleNote _note){
		m_currentLongNote = null;
		//keep note if the note is long and the head
		if (_note.Type == NoteData.NoteType.LONG) {
			BattleNoteLong noteLong = (BattleNoteLong) _note;
			if( noteLong.IsHead )
				m_currentLongNote = noteLong;
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

	public void Disable(){
		if (m_enabled == false)
			return;
		m_enabled = false;
		for( int i=m_notes.Count-1; i >= 0 ; i--){
			m_notes[i].Die();
			m_notes.RemoveAt(i);
		}
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

	public BattleNote CurrentNote{
		get{
			if( m_notes == null || m_notes.Count <= 0 )
				return null;
			return m_notes[0];
		}
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

	public bool Active {
		get {
			return m_enabled;
		}
	}

}

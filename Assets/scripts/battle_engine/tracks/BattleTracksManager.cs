﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleTracksManager : MonoBehaviour {

	static BattleTracksManager _instance;

	[SerializeField] BattleEngine m_engine;
	//State
	public enum BattleState { ATTACK, DEFEND, SWITCHING };
	private BattleState m_state = BattleState.DEFEND;
	private BattleState m_switchState = BattleState.ATTACK;

	//Tracks
	public List<BattleTrack> m_tracks;
	private int m_currentTrackID = 0;

	/** GameObject containing all simple notes */
	[SerializeField] public GameObject m_simpleNotesGroup;
    [SerializeField] public GameObject m_longNotesGroup;
    [SerializeField] public GameObject m_slideNotesGroup;
    /** List of notes scripts */
    private List<BattleNote> m_simpleNotes;
	private List<BattleNote> m_longNotes;
    private List<BattleNote> m_slideNotes;

    //COLOR
    public Color attackColor;
	public Color defendColor;

	[SerializeField] int m_magicNoteRate = 20;

	private BattleNote m_lastNoteLaunched = null;
	/** How many loops we have done */
	private int m_iteration = 1;

	private float m_currentSpeed = 5.0f;

    //EVENTS
	public event System.EventHandler<NoteEventInfo> noteEventHandler;
    public class NoteEventInfo : System.EventArgs{
        public NoteData NoteHit { get; private set; }
        public NoteData NextNote { get; set; }
        public bool Success { get; private set; }
        public BattleScoreManager.Accuracy Accuracy { get; private set; }
		/// <summary>
		/// Tells if the note is on a disabled track
		/// </summary>
		public bool IsFinal { get; set; }

		public NoteEventInfo(NoteData _notehit, bool _success, BattleScoreManager.Accuracy _acc = BattleScoreManager.Accuracy.MISS, bool _isFinal = false)
        {
            NoteHit = _notehit;
            Success = _success;
            Accuracy = _acc;
			IsFinal = _isFinal;
        }
    }

	void Awake(){
		_instance = this;
	}

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 ();
		m_simpleNotesGroup.transform.localPosition = new Vector3 ();
		m_longNotesGroup.transform.localPosition = new Vector3 ();
		FindNotesScripts ();
        InitTracks();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_state == BattleState.SWITCHING)
			UpdateSwitching ();
	}

    void InitTracks()
    {
        for(int i=0; i < m_tracks.Count; i++)
        {
            m_tracks[i].Id = i;
        }
    }

	#region PAUSE/RESUME

	public void Pause(){
		for (int i=0; i < m_tracks.Count; i++) {
			m_tracks[i].Pause();
		}
	}

	public void Resume(){
		for (int i=0; i < m_tracks.Count; i++) {
			m_tracks[i].Resume();
		}
	}

	#endregion

	#region SWITCHING

	public void SwitchPhase(){
		if (m_state == BattleState.ATTACK)
			m_switchState = BattleState.DEFEND;
		else
			m_switchState = BattleState.ATTACK;
		m_state = BattleState.SWITCHING;
	}

	public void UpdateSwitching(){
		int notesOnTracks = 0;
		for( int i=0; i < m_tracks.Count; i ++ ){
			notesOnTracks += m_tracks[i].NotesOnTrack.Count;
		}
		//if no more notes, we can switch
		if (notesOnTracks <= 0) {
			ExecuteSwitch();
		}
	}

	public void ExecuteSwitch(){
		m_state = m_switchState;
		//notify engine
		m_engine.OnSwitchSuccessful();
		//notify tracks
		for( int i=0; i < m_tracks.Count; i ++ ){
			m_tracks[i].SwitchPhase();
		}
		if (m_state == BattleState.DEFEND)
			m_currentSpeed = Mathf.Abs (m_currentSpeed) * -1;
		else
			m_currentSpeed = Mathf.Abs (m_currentSpeed);
		CheckCurrentTrack ();
	}

    #endregion

    #region LAUNCH_NOTE

    /// <summary>
    ///  Called by the NoteGenerator. Only entry to create a note on a track
    /// </summary>
    public void LaunchNote( NoteData _data, int _iteration){
		m_iteration = _iteration;
		//don't throw notes while switchin'
		if (m_state == BattleState.SWITCHING)
			return;
		//set subtype according to randomness
		ApplySubtype (_data);

		bool success = false;
		switch (_data.Type) {
			case NoteData.NoteType.SIMPLE : success = LaunchNote(_data, m_simpleNotes); break;
			case NoteData.NoteType.LONG : success = LaunchLongNote(_data); break;
            case NoteData.NoteType.SLIDE: success = LaunchNote(_data, m_slideNotes); break;
		}
		if (success) {
			m_engine.OnNoteLaunched (_data);
		}
	}

	bool LaunchNote(NoteData _data, List<BattleNote> _notes){
		BattleNote note = null;
		//Search for a available note
		for (int i=0; i < _notes.Count; i ++) {
			if(_notes[i].CurrentState == BattleNote.State.DEAD ){
				note = _notes[i];
				break;
			}
		}
		if (note == null) {
			Debug.LogError( "No Note Available Note found ");
			return false;
		}
		//Launch the note on the right track
		return LaunchNoteOnTrack (note,_data);
	}
    
	bool LaunchLongNote( NoteData _data ){
		BattleNoteLong note = null;
		//TRYING TO ADD A TAIL
		//if the new note is a TAIL
		if (_data.Head == false) {
			bool lastIsLong = m_lastNoteLaunched && m_lastNoteLaunched.Type == NoteData.NoteType.LONG;
			//If there's no long note on the track. QUIT
			if( !lastIsLong )
				return false;
			//Convert Last Note
			BattleNoteLong lastLongNote = (BattleNoteLong)m_lastNoteLaunched;
			//Get TAIL note and launch it( if the last note is a head
			note = lastLongNote.TailNote;
			if (note != null) {				
				//Launch the TAIL note on the right track
				return LaunchNoteOnTrack (note, _data);
			}
			return false;
		}
		//TRYING TO ADD A HEAD
		else {
			//Else Search for an available long note HEAD
			for (int i=0; i < m_longNotes.Count; i ++) {
				BattleNoteLong tmpNote = (BattleNoteLong)m_longNotes [i];
				if (tmpNote.IsDead && tmpNote.IsHead) {
					note = tmpNote;
					break;
				}
			}
			if (note == null) {
				Debug.LogError ("No Available Note found ");
				return false;
			}
			//Launch the note on the right track
			return LaunchNoteOnTrack (note, _data);
		}
	}

    /// <summary>
    /// Every BattleNote added to the track pass in there
    /// </summary>
    bool LaunchNoteOnTrack(BattleNote _note,NoteData _data){
		//keep track of launched notes
		m_lastNoteLaunched = _note;
		//Affect data to BattleNote
		_note.Data = _data;		
		BattleTrack track = m_tracks [_data.TrackID];
		if ( !track.IsActive ) {
			track = GetReplacementTrack ();
		}
		//Affect TrackID ( may vary if a track is disabled ) : The TrackID of a track may point to another when it is disabled
		_data.TrackID = track.Id;
		//Launch
		m_tracks [_data.TrackID].Iteration = m_iteration;
		bool success = m_tracks [_data.TrackID].LaunchNote (_note,m_currentSpeed);
		CheckCurrentTrack ();
		return success;
	}

    #endregion

    #region INPUT
    /// <summary>
    /// Called when a input is pressed. The id correspond to the phase state ( attack =1 , defense = -1)
    /// methode is PRESS, RELEASE, SLIDE
    /// </summary>
    public void OnInputTriggered(int _id, BattleNote.HIT_METHOD _method)
    {
        if (CheckInputState(_id))
        {
            m_tracks[m_currentTrackID].OnInputTriggered(_method);
        }
        else
        {
            m_tracks[m_currentTrackID].OnInputError(_method);
        }
    }

	/// <summary>
	/// Check the input according to the phase (attack / defense)
	/// </summary>
	bool CheckInputState(int _id){
		if(m_state == BattleState.SWITCHING)
			return (_id < 0 && m_switchState == BattleState.ATTACK) || (_id > 0 && m_switchState == BattleState.DEFEND);
		return (_id < 0 && m_state == BattleState.DEFEND) || (_id > 0 && m_state == BattleState.ATTACK);
	}

    public void RaiseNoteEvent(NoteEventInfo _eventNote)
    {		
        if (noteEventHandler != null)
        {
            CheckCurrentTrack();
            var nextNote = m_tracks[m_currentTrackID].CurrentNote;
            _eventNote.NextNote = nextNote !=null ? nextNote.Data : null ; // the note being hit/missed cannot be the current
            noteEventHandler.Invoke(this, _eventNote);
        }
		//check redirection
		BattleTrack track = m_tracks[ _eventNote.NoteHit.TrackID ];
		if (!track.IsActive && track.IsEmpty) {
			var replcmtTrack = GetReplacementTrack ();
			if( replcmtTrack != null )
				RedirectTrack (track.Id,replcmtTrack.Id);
		}
    }

	#endregion

	#region FIND_NOTES
	void FindNotesScripts(){
		//Create list of simples notes scripts
		BattleNote[] notes = m_simpleNotesGroup.GetComponentsInChildren<BattleNote> (true);
		m_simpleNotes = new List<BattleNote> (notes);
        //Long notes
		notes = m_longNotesGroup.GetComponentsInChildren<BattleNote> (true);
		m_longNotes = new List<BattleNote> (notes);
        //Slide notes
        notes = m_slideNotesGroup.GetComponentsInChildren<BattleNote>(true);
        m_slideNotes = new List<BattleNote>(notes);
    }

    #endregion
    
    ///<summary>
    /// Give subtype to the note (MAGIC / REGULAR ...)
    ///</summary>
    void ApplySubtype(NoteData _note){
		if (_note.Type == NoteData.NoteType.LONG ) {
			return;
		}
		int r = Random.Range (0, 100);
		if (r < m_magicNoteRate) {			
			_note.Subtype = NoteData.NoteSubtype.MAGIC;
		} else {
			_note.Subtype = NoteData.NoteSubtype.REGULAR;
		}
	}

	#region DISABLING

	/** Disable a track from the set and so its next notes are redirected to another track*/
	public void DisableTrack(int _index, int _replacementIndex){
		if (_index < m_tracks.Count) {
			//Debug.Log ("Replace " + _index + " par " + _replacementIndex);
			bool trackIsClear = m_tracks [_index].Disable ();
			if (trackIsClear) {
				RedirectTrack (_index, _replacementIndex);
			}
			CheckCurrentTrack();
		} else {
			Debug.LogError ("Track number " + _index + " is not accessible");
		}
	}

	void RedirectTrack(int _disabledIndex, int _replacementIndex){
		//redirect disabled tracks to the replacement one
		for (int i = 0; i < m_tracks.Count; i++) {	
			if (m_tracks [i].Id == _disabledIndex) {
				m_tracks [i] = m_tracks [_replacementIndex];
				m_tracks [i].Id = _replacementIndex;
				break;
			}
		}
	}

	BattleTrack GetReplacementTrack(){
		//redirect disabled tracks to the replacement one
		for (int i = 0; i < m_tracks.Count; i++) {	
			if (m_tracks [i].IsActive) {
				return m_tracks [i];
			}
		}
		return null;
	}
    #endregion
    
    ///<summary>
    /// Adds the performed note to the engine, even if missed, so that it can be processed
    ///</summary>
    public BattleScoreManager.Accuracy AddNote(BattleNote _note, float _accuracy){
		CheckCurrentTrack ();
		return m_engine.AddNote (_note.Data,_accuracy);
	}

    /// <summary>
    /// Search between all tracks to see the one which is the current one aka the one with the next not on it
    /// </summary>
    void CheckCurrentTrack(){
		float bestTime = float.MaxValue;
		int minIteration = int.MaxValue;
		for(int i=0; i < m_tracks.Count; i ++){
			BattleNote n = m_tracks[i].CurrentNote;
			if( n==null )
				continue;
			if( m_tracks[i].Iteration < minIteration || n.Data.TimeBegin < bestTime ){
				bestTime = n.Data.TimeBegin ;
				minIteration = m_tracks[i].Iteration;
				m_currentTrackID = i;
			}
		}
	}

    /// <summary>
    /// Computes the speed of the notes according to the timeshift ( time for a note to arrive )
    /// </summary>
    /// <param name="_timeShift"></param>
    public void SetTimeShift(float _timeShift){
		m_currentSpeed = m_tracks [0].Length / _timeShift;
	}

	public BattleState State{
		get{
			return m_state;
		}
	}

	public BattleState PhaseState{
		get{
			if( m_state == BattleState.SWITCHING ){
				if( m_switchState == BattleState.ATTACK )
					return BattleState.DEFEND;
				else
					return BattleState.ATTACK;
			}
			return m_state;
		}
	}

	public bool IsAttacking{
		get{
			return PhaseState == BattleState.ATTACK;
		}
	}

	public static BattleTracksManager instance{
		get{
			return _instance;
		}
	}
}

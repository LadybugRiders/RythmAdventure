using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleTracksManager : MonoBehaviour {
	[SerializeField] BattleEngine m_engine;
	//State
	public enum BattleState { ATTACK, DEFEND, SWITCHING };
	private BattleState m_state = BattleState.DEFEND;
	private BattleState m_switchState = BattleState.ATTACK;

	//Tracks
	public List<BattleTrack> m_tracks;
	private int m_currentTrackID = 0;

	/** GameObject containing all simple notes */
	public GameObject m_simpleNotesGroup;
	public GameObject m_longNotesGroup;
	/** List of simple notes scipts */
	private List<BattleNote> m_simpleNotes;
	private List<BattleNote> m_longNotes;

	//COLOR
	public Color attackColor;
	public Color defendColor;

	[SerializeField] int m_magicNoteRate = 20;

	private BattleNote m_lastNoteLaunched = null;
	/** How many loops we have done */
	private int m_iteration = 1;

	private float m_currentSpeed = 5.0f;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 ();
		m_simpleNotesGroup.transform.localPosition = new Vector3 ();
		m_longNotesGroup.transform.localPosition = new Vector3 ();
		FindNotesScripts ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_state == BattleState.SWITCHING)
			UpdateSwitching ();
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
		m_engine.OnSwitchSuccesful();
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

	/** Called by the NoteGenerator. Only entry to create a note on a track */
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
			case NoteData.NoteType.LONG : success = LaunchLongNote(_data);break;
		}
		if (success) {
			m_engine.OnNoteLaunched (_data);
		}
	}

	bool LaunchNote(NoteData _data, List<BattleNote> _inputArray){
		BattleNote note = null;
		//Search for a available note
		for (int i=0; i < _inputArray.Count; i ++) {
			if( _inputArray[i].CurrentState == BattleNote.State.DEAD ){
				note = _inputArray[i];
				break;
			}
		}
		if (note == null) {
			Debug.LogError( "No Available Note found ");
			return false;
		}
		//Launch the note on the right track
		return LaunchNoteOnTrack (note,_data);
	}

	/** Laucnh a long note */
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

	/** Every BattleNote added to the track pass in there */
	bool LaunchNoteOnTrack(BattleNote _note,NoteData _data){
		//keep track of launched notes
		m_lastNoteLaunched = _note;
		//Affect data to BattleNote
		_note.Data = _data;		
		//Affect TrackID ( may vary if a track is disabled )
		_data.TrackID = m_tracks[_data.TrackID].Id;
		//Launch
		m_tracks [_data.TrackID].Iteration = m_iteration;
		bool success = m_tracks [_data.TrackID].LaunchNote (_note,m_currentSpeed);
		CheckCurrentTrack ();
		return success;
	}

	#endregion

	#region INPUT

	/** Called when a input is pressed. The id correspond to the phase state ( attack > 0, defense < 0) */
	public void OnInputDown(int _id){
		if (CheckInputState(_id)) {			
			m_tracks [m_currentTrackID].OnInputDown ();
		} else {	
			m_tracks[m_currentTrackID].OnInputError(true);
		}
	}

	/** Called when a input is released. The id correspond to the phase state ( attack > 0, defense < 0) */
	public void OnInputUp(int _id){
		if (CheckInputState(_id)) {			
			m_tracks [m_currentTrackID].OnInputUp ();
		} else {	
			m_tracks[m_currentTrackID].OnInputError(false);
		}
	}

	bool CheckInputState(int _id){
		if(m_state == BattleState.SWITCHING)
			return (_id < 0 && m_switchState == BattleState.ATTACK) || (_id > 0 && m_switchState == BattleState.DEFEND);
		return (_id < 0 && m_state == BattleState.DEFEND) || (_id > 0 && m_state == BattleState.ATTACK);
	}

	#endregion

	#region FIND_NOTES
	void FindNotesScripts(){
		FindSimpleNotes ();
		FindLongNotes ();
	}

	void FindSimpleNotes(){		
		//Create list of simples notes scripts
		BattleNote[] notes = m_simpleNotesGroup.GetComponentsInChildren<BattleNote> (true);
		m_simpleNotes = new List<BattleNote> (notes);
	}

	void FindLongNotes(){		
		BattleNote[] notes = m_longNotesGroup.GetComponentsInChildren<BattleNote> (true);
		m_longNotes = new List<BattleNote> (notes);
	}

	#endregion

	/** Give subtype to the note*/
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
			m_tracks [_index].Disable ();
			//redirect disabled tracks to the replacement one
			for(int i=0; i < m_tracks.Count; i++){	
				if( m_tracks[i].Id == _index ){
					m_tracks[i] = m_tracks[_replacementIndex];
					m_tracks[i].Id = _replacementIndex;
					break;
				}
			}
			CheckCurrentTrack();
		} else {
			Debug.LogError ("Track number " + _index + " is not accessible");
		}
	}

	#endregion

	/** Adds the performed note to the engine, even if missed */
	public BattleScoreManager.Accuracy AddNote(BattleNote _note, float _accuracy){
		CheckCurrentTrack ();
		return m_engine.AddNote (_note.Data,_accuracy);
	}

	/** Search between all tracks to see the one which is the current one */
	void CheckCurrentTrack(){
		float bestTime = float.MaxValue;
		int minIteration = int.MaxValue;
		for(int i=0; i < m_tracks.Count; i ++){
			BattleNote n = m_tracks[i].CurrentNote;
			if( n==null)
				continue;
			if( m_tracks[i].Iteration < minIteration || n.Data.TimeBegin < bestTime ){
				bestTime = n.Data.TimeBegin ;
				minIteration = m_tracks[i].Iteration;
				m_currentTrackID = i;
			}
		}
	}

	/** Computes the speed of the notes according to the timeshift ( time for a note to arrive )*/
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
}

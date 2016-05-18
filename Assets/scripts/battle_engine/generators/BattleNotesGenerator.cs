using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleNotesGenerator : MonoBehaviour {

	[SerializeField] protected BattleEngine m_engine;
	[SerializeField] protected BattleTracksManager m_tracksManager;

	[SerializeField] protected SongDataSO m_songData;
	[SerializeField] protected TextAsset m_songDataJSON;

	[SerializeField] protected bool m_loop = true;
	[SerializeField] protected float m_speedModifier = 1.0f;

	protected float m_timeElapsed = 0.0f;
	/** Time for a note to complete its way to the end */
	protected float m_timeShift = 0.0f;
	/** Synchronisation issue */
	protected float m_timeSynchroDelta = 0f;

	protected List< NoteData > m_notes;
	protected int m_index = 0;
	protected NoteData m_currentNote = null;
	protected int m_iteration = 1;
	protected bool m_looper = true;

	/** How many times we have reached the end of the notes */
	protected int m_notesIterations = 1;

	protected bool m_paused = true;
	protected bool m_finished = false;

	// Use this for initialization
	void Awake () {
		//m_notes = m_songData.GetAllNotes ();
		//Debug.Log (m_songDataJSON.text);
#if !UNITY_STANDALONE && !UNITY_EDITOR
		//m_timeSynchroDelta = -0.15f;
#endif
	}

	public void Begin(float _timeShift){
		if (m_notes.Count <= 0)
			return;
		m_paused = false;
		m_timeShift = _timeShift;
		m_index = GetFirstNoteIndex(_timeShift);
		m_currentNote = m_notes [m_index];
		//to ensure that notes & music are in the same iterations
		m_iteration = 1;
		m_notesIterations = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_paused || m_finished)
			return;
		m_timeElapsed = m_engine.TimeElapsed + m_timeShift + m_timeSynchroDelta;

		//check loop and modify time if longer that clip time ( for looping )
		if (m_timeElapsed >= m_engine.AudioSrc.clip.length) {
			//check if we havent looped yet
			if (m_looper) {
				m_iteration++;
				m_looper = false;
			}
			//change time
			m_timeElapsed -= m_engine.AudioSrc.clip.length;
			//check note to spawn
		} else {
			m_looper = true;
		}

		//if the current note time has been reached
		//we also check that we are in the same iteration
		if(m_notesIterations == m_iteration && m_currentNote.TimeBegin  <= m_timeElapsed) {
			//Debug.Log (m_currentNote.TimeBegin + " " + m_timeElapsed + " " + m_currentNote.Type.ToString());
			//Send note
			m_tracksManager.LaunchNote(m_currentNote.Clone(),m_iteration);
			//go to next note or end
			m_index ++;
			if( m_index < m_notes.Count ){
				m_currentNote = m_notes[m_index];
			}else{				
				m_index = 0;
				m_currentNote = m_notes [m_index];
				m_notesIterations ++;
			}
		}
	}

	public void LoadData(JSONObject _json){
		m_notes = new List<NoteData> ();
		//Debug.Log (jsonData);
		List<JSONObject> arrayNotes = _json.GetField ("notes").list;
		foreach(JSONObject noteJSON in arrayNotes){
			NoteData noteData = new NoteData();
			//Debug.Log( noteJSON);
			noteData.Type = (NoteData.NoteType) ((int)noteJSON.GetField("type").n);
			noteData.TimeBegin = noteJSON.GetField("time").f;
			noteData.Head = noteJSON.GetField("head").b;
			noteData.TrackID =(int) noteJSON.GetField("track").n;
			m_notes.Add( noteData);
		}
	}

	int GetFirstNoteIndex(float _beginTime){
		for(int i=0 ; i < m_notes.Count; i ++){
			if( m_notes[i].TimeBegin > _beginTime )
				return i;
		}
		return m_notes.Count-1;
	}

	public void BeginDebug(float _timeShift, float _timeBegin){
		if (m_notes.Count <= 0)
			return;
		m_paused = false;
		m_timeShift = _timeShift;
		m_index = GetFirstNoteIndex(_timeBegin + _timeShift);
		m_currentNote = m_notes [m_index];
		//to ensure that notes & music are in the same iterations
		m_iteration = 1;
		m_notesIterations = 1;
	}

	public float TimeSynchroDelta {
		get {
			return m_timeSynchroDelta;
		}
		set {
			m_timeSynchroDelta = value;
		}
	}
}

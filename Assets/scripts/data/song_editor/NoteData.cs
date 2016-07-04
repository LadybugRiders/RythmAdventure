﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class NoteData {

	[SerializeField] protected float m_timeBegin;
	
	public enum NoteType{ SIMPLE, LONG, SLIDE };
	[SerializeField] protected NoteType m_type = NoteType.SIMPLE;

	[SerializeField] protected int m_trackID= 0;
	
	public enum NoteSubtype { REGULAR, MAGIC, NOTHING };
	[SerializeField] protected NoteSubtype m_subtype;

	/** Unused if not LONG */
	[SerializeField] protected bool m_head = true;

	/** Use to store the accuracy of a hit note. Unused outside battle */
	protected BattleScoreManager.Accuracy m_hitAccuracy;

	public NoteData Clone(){
		NoteData noteData = new NoteData ();
		noteData.TimeBegin = this.TimeBegin;
		noteData.Type = this.Type;
		noteData.TrackID = this.TrackID;
		noteData.Subtype = this.Subtype;
		noteData.Head = this.Head;
		return noteData;
	}

	#region GETTERS_SETTERS

	public NoteType Type {
		get {
			return m_type;
		}
		set {
			m_type = value;
		}
	}

	public int TrackID {
		get {
			return m_trackID;
		}
		set {
			m_trackID = value;
		}
	}

	public float TimeBegin {
		get {
			return m_timeBegin;
		}
		set {
			m_timeBegin = value;
		}
	}

	public bool Head {
		get {
			return m_head;
		}
		set {
			m_head = value;
		}
	}

	public NoteSubtype Subtype {
		get {
			return m_subtype;
		}
		set {
			m_subtype = value;
		}
	}
	

	public BattleScoreManager.Accuracy HitAccuracy {
		get {
			return m_hitAccuracy;
		}
		set {
			m_hitAccuracy = value;
		}
	}
	#endregion

}

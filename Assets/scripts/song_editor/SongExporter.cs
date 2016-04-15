﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SongExporter {

	JSONObject m_json = new JSONObject();

	public void SetUp(SongEditorManager _manager){
		if( _manager.music != null)
			m_json.AddField ("clipName", _manager.music.name);
		else
			m_json.AddField ("clipName", _manager.songName);

		m_json.AddField ("timeSpeed", _manager.timeSpeed);
	}

	public void SetNotes(List<SongEditorNote> _notes){
		JSONObject allNotes = new JSONObject (JSONObject.Type.ARRAY);

		JSONObject note;
		for (int i = 0; i < _notes.Count; i ++) {
			note = new JSONObject();
			SongEditorNote seNote = _notes[i];
			note.AddField("type", (int) seNote.type);
			note.AddField("time", seNote.time);
			note.AddField("head", seNote.head);
			//TrackID
			if( seNote.CurrentTrack != null ){
				note.AddField("track", seNote.CurrentTrack.Id);
			}

			allNotes.Add(note);
		}

		m_json.AddField ("notes", allNotes);
	}

	public void Export(string _songName, BattleEngine.Difficulty _difficulty){
		
		string fullName = _songName + "_"+_difficulty.ToString().ToLower();
		string folderPath = Application.dataPath + "/Resources/song_data/" + _songName;

		//check folder and create if necessary
		if ( ! Directory.Exists (folderPath)) {
			Directory.CreateDirectory(folderPath);
		}

#if UNITY_EDITOR
        string text = m_json.Print();

        File.WriteAllText( folderPath +"/"+fullName+ ".json", text);
		AssetDatabase.Refresh();
#endif
	}
}

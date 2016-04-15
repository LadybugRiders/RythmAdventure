using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataCharacterWrapper {
 
	JSONObject m_data;
	//List of Characters
	List<DataCharacter> m_characters;

	public void Import(){

	}

	public void Load(JSONObject _dataJSON){
		m_data = _dataJSON;
	}

	public void Save(){
		for(int i=0; i < m_characters.Count; i++){
			string str = m_characters[i].GetData();
			Debug.Log( str );
		}
	}
}

		   
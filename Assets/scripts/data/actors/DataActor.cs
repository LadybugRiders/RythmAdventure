using UnityEngine;
using System.Collections;
using System.IO;

public class DataActor {
	
	protected Stats m_currentStats;
	protected Stats m_maxStats;

	JSONObject m_json = new JSONObject();

	string m_name = "player";

	public void Init(){
		m_currentStats = new Stats ();
		m_maxStats = new Stats ();
		m_currentStats.MP = 0;
	}

	public string GetData(){
		m_json.SetField ("lvl", m_maxStats.Level);
		m_json.SetField ("hp", m_maxStats.HP);

		return m_json.str;
	}

	public void Import(){

	}

	public Stats CurrentStats {
		get {
			return m_currentStats;
		}
		set {
			m_currentStats = value;
		}
	}

	public Stats MaxStats {
		get {
			return m_maxStats;
		}
		set {
			m_maxStats = value;
		}
	}
}

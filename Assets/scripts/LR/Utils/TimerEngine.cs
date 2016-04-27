using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimerEngine : MonoBehaviour {

	private static TimerEngine _instance;
	
	private List<Timer> m_timers;
	
	public static TimerEngine instance {
		get{
			if( _instance == null ){
				GameObject newGO = new GameObject("TimerEngine");
				_instance = newGO.AddComponent<TimerEngine>();
			}
			return _instance;
		}
	}

	void Awake(){
		if (_instance == null)
			_instance = this;
		if (m_timers == null)
			m_timers = new List<Timer> ();
	}
	
	// Update is called once per frame
	void Update () {
		//loop and update timers
		for (int i= m_timers.Count -1 ; i > -1; i --) {
			if( m_timers[i].UpdateTimer(Time.deltaTime) == true ){
				m_timers.RemoveAt(i);
			}
		}
	}

	/** Add a timer and start it. Time is in seconds */
	public Timer AddTimer(float _time, string _callbackBackName, GameObject _callbackObject){
		Timer timer = new Timer (_time, _callbackBackName,_callbackObject);
		m_timers.Add (timer);
		return timer;
	}

	public class Timer{
		protected float m_time;
		protected float m_targetTime;
		protected bool m_finished = false;
		
		protected string m_callbackName;
		protected GameObject m_callbackObject;
		
		public Timer(float _targetTime, string _callbackName, GameObject _callbackObject){
			m_targetTime = _targetTime;
			m_callbackName = _callbackName;
			m_callbackObject = _callbackObject;
		}

		protected void SendCallback(){
			//send callback
			if(m_callbackObject != null && m_callbackName != null && m_callbackName != "" )
				m_callbackObject.SendMessage(m_callbackName);
		}
		
		/** Update the timer and return true if ended */
		public bool UpdateTimer( float _deltaTime ){ 
			
			m_time += _deltaTime;
			//if time's up
			if( m_time >= m_targetTime){
				m_finished = true;
				SendCallback();
				return true;
			}
			return false;
		}
	}
}

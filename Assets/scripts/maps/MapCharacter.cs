using UnityEngine;
using System.Collections;

public class MapCharacter : MonoBehaviour {

    enum State { IDLE, MOVING };
    State m_state;

    MapNode m_targetNode;
    [SerializeField] float m_speed = 3.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        switch (m_state)
        {
            case State.IDLE: Idling(); break;
            case State.MOVING: Moving(); break;
        }
	}

    public void GoTo(MapNode node)
    {
        m_targetNode = node;
        m_state = State.MOVING;
    }

    void Moving()
    {
        Vector3 toTarget = m_targetNode.transform.position - transform.position;
        toTarget.z = 0; // we don't want to move along the Z axis
        //if the target is reached
        if(toTarget.magnitude <= m_speed * 1.5f)
        {
            m_state = State.IDLE;
            transform.position = m_targetNode.transform.position;
        }
        //else move to target
        else
        {
            transform.position += toTarget.normalized * m_speed;
        }
    }

    void Idling()
    {

    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteTouchHandler : MonoBehaviour {

    [SerializeField] List<GameObject> m_targetsCallback;

    [SerializeField] Collider2D m_collider;

    [SerializeField] string m_onInputDownCallback;
    [SerializeField] string m_onInputUpCallback;

    bool m_pressed = false;
    int m_inputDown = -1;

    // Use this for initialization
    void Start()
    {
        if (m_collider == null)
            m_collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check press
        if (m_pressed == false && IsPressed())
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(CustomGetTouchPosition());
            Vector2 touchPos = new Vector2(worldPoint.x, worldPoint.y);
            Collider2D coll = Physics2D.OverlapPoint(touchPos);
            //check attack 
            if (m_collider == coll)
            {
                m_pressed = true;
                SendCallbacks(m_onInputDownCallback);
            }
        }

        //Check Release
        if (m_pressed && IsReleased())
        {
            m_pressed = false;
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(CustomGetTouchPosition());
            Vector2 touchPos = new Vector2(worldPoint.x, worldPoint.y);
            Collider2D coll = Physics2D.OverlapPoint(touchPos);
            if (m_collider == coll)
            {
                SendCallbacks(m_onInputUpCallback);
            }
        }
    }

    Vector2 CustomGetTouchPosition()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        return Input.mousePosition;
#else
		return Input.GetTouch(0).position;
#endif
    }

    bool IsPressed()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        return Input.GetMouseButtonDown(0);
#else
			return Input.touchCount == 1;
#endif
    }

    bool IsReleased()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        return Input.GetMouseButtonUp(0);
#else
	return Input.touchCount < 1 ;
#endif
    }

    void SendCallbacks(string callback)
    {
        foreach( var go in m_targetsCallback)
        {
            go.SendMessage(callback, this, SendMessageOptions.DontRequireReceiver);
        }
    }
}

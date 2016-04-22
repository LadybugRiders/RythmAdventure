using UnityEngine;
using System.Collections;

public partial class MapNodesManager {
    
    MapNode m_pressedNode = null;

    // Use this for initialization
    void StartTouch()
    {
    }

    // Update is called once per frame
    void UpdateTouch()
    {
        if(IsPressed() || IsReleased())
        {
            //Get touched collider
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(CustomGetTouchPosition());
            var touchPosition = new Vector2(worldPoint.x, worldPoint.y);
            var colliderTouched = Physics2D.OverlapPoint(touchPosition);
            //Get the MapNode touched
            MapNode node = GetNodeTouched(colliderTouched);
            if (node == null)
            {
                m_pressedNode = null;
                return;
            }
            //finally, do the process
            if (IsPressed())
            {
                m_pressedNode = node;
                OnNodeTouchDown(node);
            }else
            {
                if( m_pressedNode == node)
                    OnNodeTouchRelease(node);
                m_pressedNode = null;
            }
        }
    }

    MapNode GetNodeTouched(Collider2D colliderTouched)
    {
        for(int i=0; i < m_nodes.Count; ++i)
        {
            if( m_nodes[i].gameObject.GetComponent<Collider2D>() == colliderTouched)
            {
                return m_nodes[i];
            }
        }
        return null;
    }

    #region UTILS
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
    #endregion
}

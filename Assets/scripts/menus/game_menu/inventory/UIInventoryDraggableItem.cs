using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UIInventoryDraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{    
    [SerializeField] private bool m_draggable = true;
    Vector3 m_initialPosition;
    
    void Update()
    {

    }

    public void OnBeginDrag(PointerEventData _eventData)
    {
        m_initialPosition = transform.localPosition;
    }

    public void OnDrag(PointerEventData _eventData)
    {
        if (m_draggable)
        {
            transform.position = _eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData _eventData)
    {
        if (m_draggable == false)
            return;
        
        transform.localPosition = m_initialPosition;
    }
        
}


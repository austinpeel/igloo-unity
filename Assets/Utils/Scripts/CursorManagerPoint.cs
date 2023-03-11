using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PointParameterUI))]
public class CursorManagerPoint : CursorManager, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private PointParameterUI pointParameterUI;

    private void Awake() 
    {
        pointParameterUI = GetComponent<PointParameterUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
       pointParameterUI.OnBeginDrag(Input.mousePosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        pointParameterUI.OnDrag(Input.mousePosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        pointParameterUI.OnEndDrag(Input.mousePosition);
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PointParameterUI))]
public class CursorManagerPoint : CursorManager, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private PointParameterUI pointParameterUI;
    private bool isDragging = false;

    private void Awake() 
    {
        pointParameterUI = GetComponent<PointParameterUI>();
    }

    public new void OnPointerEnter(PointerEventData pointerEventData)
    {
        // If the user is dragging the point then it should stays at ScaleHover and the cursor should remain the handCursor
        // So no need to Set them again
        if (isDragging)
        {
            return;
        }

        base.OnPointerEnter(pointerEventData);

        // Increase the size of the point when the cursor hover it
        pointParameterUI.SetScaleHover();
    }

    public new void OnPointerExit(PointerEventData pointerEventData)
    {
        // If the user is dragging the point then it should stays at ScaleHover and the cursor should remain the handCursor
        // And the point will be set back to ScaleNotHover in OnEndDrag
        if (isDragging)
        {
            return;
        }

        base.OnPointerExit(pointerEventData);

        // Decrease the size of the point when the cursor doesn't hover it anymore
        pointParameterUI.SetScaleNotHover();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        pointParameterUI.OnBeginDrag(Input.mousePosition);

        // Increase the size of the point when the point is being dragged
        pointParameterUI.SetScaleHover();

        // Be sure that the handCursor is displayed
        base.OnPointerEnter(eventData);
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        pointParameterUI.OnDrag(Input.mousePosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        pointParameterUI.OnEndDrag(Input.mousePosition);

        // Decrease the size of the point when the point is not being dragged anymore
        pointParameterUI.SetScaleNotHover();

        // Be sure that the handCursor is not displayed anymore
        base.OnPointerExit(eventData);
        isDragging = false;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public Texture2D handCursor;
    private Vector2 hotspot = new Vector2(14, 6);
    private EllipseUI ellipseUI;
    private CenterPointUI centerPointUI;
    private bool mouseHasEnteredEllipse = false;
    private bool isHandCursor = false;

    private void Awake() 
    {
        ellipseUI = GetComponent<EllipseUI>();
        centerPointUI = GetComponent<CenterPointUI>();
    }

    private void Update() 
    {
        if (mouseHasEnteredEllipse)
        {
            // Check the position of the mouse
            // If the mouse lies on the ellipse then display the handCursor
            if (ellipseUI.IsPositionOnEllipse(Input.mousePosition))
            {
                // If the cursor is already the hand, then we don't need to change it
                if (isHandCursor) return;

                Cursor.SetCursor(handCursor, hotspot, CursorMode.Auto);
                isHandCursor = true;
            }
            // Else display the default Cursor 
            else 
            {
                // If the cursor is already the default one, then we don't need to change it
                if (!isHandCursor) return;

                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                isHandCursor = false;
            }
        }
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        // If it is the ellipse, then we have to first check if the cursor lies on the ellipse
        if (ellipseUI)
        {
            mouseHasEnteredEllipse = true;
            return;
        }

        Cursor.SetCursor(handCursor, hotspot, CursorMode.Auto);
        isHandCursor = true;
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (ellipseUI)
        {
            mouseHasEnteredEllipse = false;
        }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        isHandCursor = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("BEGIN");
    }

    // TODO : update the text of the position (x, y) accordingly 
    public void OnDrag(PointerEventData eventData)
    {
        // Drag the center point to move the ellipse and the point
        if (centerPointUI)
        {   
            centerPointUI.SetCenterPosition(Input.mousePosition);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("END");
    }
}

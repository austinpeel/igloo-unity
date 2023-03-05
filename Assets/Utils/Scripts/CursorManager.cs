using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public Texture2D handCursor;
    private Vector2 hotspot = new Vector2(14, 6);
    private EllipseUI ellipseUI;
    private CenterPointUI centerPointUI;
    private bool mouseHasEnteredEllipse = false;

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
            // If the mouse is over the ellipse then display the handCursor
            // Else display the default Cursor

        }
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Cursor.SetCursor(handCursor, hotspot, CursorMode.Auto);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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

    // Used in Update() method
    // TODO : Check if the cursor is on the outline of the ellipse to display the right cursor
    private bool checkCursorOnEllipse(Vector2 cursorPosition)
    {
        if (!ellipseUI) return false;

        return true;
    }
}

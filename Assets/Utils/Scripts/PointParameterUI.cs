using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class PointParameterUI : MonoBehaviour
{
    // Define a custom event delegate with a 'sender' parameter and the position of the cursor
    public delegate void ParameterChangedEventHandler(object sender, Vector2 cursorPosition);
    // Define the custom event
    public event ParameterChangedEventHandler OnParameterChanged;
    protected RectTransform rectTransform;
    protected Vector2 positionRect;
    protected void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetPosition(Vector2 newPosition)
    {
        // Should be called only when Awake hasn't been called yet (with for example : OnValidate())
        if (!rectTransform)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        rectTransform.anchoredPosition = newPosition;

        positionRect = newPosition;
    }

    public void TriggerParameterChanged(Vector2 cursorPosition)
    {
        OnParameterChanged?.Invoke(this, cursorPosition);
    }

    public abstract void OnBeginDrag(Vector2 cursorPosition);
    public abstract void OnDrag(Vector2 cursorPosition);
    public abstract void OnEndDrag(Vector2 cursorPosition);
}
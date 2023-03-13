using UnityEngine;

public class AnglePointUI : PointParameterUI
{
    // Define a custom event delegate with a 'sender' parameter
    public delegate void ParameterEndDragEventHandler(object sender);
    // Define the custom event
    public event ParameterEndDragEventHandler OnParameterEndDrag;
    public override void OnBeginDrag(Vector2 cursorPosition) {}

    public override void OnDrag(Vector2 cursorPosition)
    {
        TriggerParameterChanged(cursorPosition);
    }

    public override void OnEndDrag(Vector2 cursorPosition)
    {
        OnParameterEndDrag?.Invoke(this);
    }
}

using UnityEngine;

public class CenterPointUI : PointParameterUI
{
    // Define a custom event delegate with a 'sender' parameter
    public delegate void ParameterEndDragEventHandler(object sender);
    public delegate void ParameterBeginDragEventHandler(object sender, Vector2 beginDragPosition);
    // Define the custom event
    public event ParameterEndDragEventHandler OnParameterEndDrag;
    public event ParameterBeginDragEventHandler OnParameterBeginDrag;

    public override void OnBeginDrag(Vector2 cursorPosition) 
    {
        OnParameterBeginDrag?.Invoke(this, cursorPosition);
    }

    public override void OnDrag(Vector2 cursorPosition)
    {
        TriggerParameterChanged(cursorPosition);
    }

    public override void OnEndDrag(Vector2 cursorPosition)
    {
        OnParameterEndDrag?.Invoke(this);
    }
}

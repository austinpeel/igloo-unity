using UnityEngine;

public class QPointUI : PointParameterUI
{
    public override void OnBeginDrag(Vector2 cursorPosition) {}

    public override void OnDrag(Vector2 cursorPosition)
    {
        TriggerParameterChanged(cursorPosition);
    }

    public override void OnEndDrag(Vector2 cursorPosition) {}
}

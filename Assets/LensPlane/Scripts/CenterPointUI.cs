using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPointUI : MonoBehaviour
{
    [SerializeField] private EllipseUI ellipseUI;
    private RectTransform rectTransform;

    public void SetCenterPosition(Vector2 newPosition)
    {
        if (!ellipseUI) return;

        // Move the center of the ellipse to the cursor position 
        transform.position = newPosition;

        // Move the ellipse to the cursor position
        ellipseUI.SetCenterPosition(newPosition);
    }
}

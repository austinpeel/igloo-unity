using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPointUI : MonoBehaviour
{
    [SerializeField] private EllipseUI ellipseUI;
    [SerializeField] private float distanceMagnetCenter = 1f;
    private RectTransform rectTransform;
    private Vector2 centerPosition;

    private void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();

        // The CenterPoint should already be at the center so that we can have the coordinate of it in rectTransform coordinates (x, y)
        centerPosition = transform.position;
    }

    public void SetCenterPosition(Vector2 newPosition)
    {
        if (!ellipseUI) return;

        // Move the center of the ellipse to the cursor position 
        transform.position = newPosition;

        // Move the ellipse to the cursor position
        ellipseUI.MovePosition(newPosition);

        // Set the center of the ellipse with the coordinate relative to the anchors of the RectTranform
        ellipseUI.SetCenterPosition(rectTransform.anchoredPosition);
    }

    // "Magnet Effect" for the position :
    // If the position at the end of the drag is less than distanceMagnetCenter to the center position
    // Then set the position to centerPosition
    public void MagnetPosition()
    {
        if (rectTransform.anchoredPosition.magnitude < distanceMagnetCenter)
        {
            SetCenterPosition(centerPosition);
        }
    }
}

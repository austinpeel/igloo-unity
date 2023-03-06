using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPointUI : MonoBehaviour
{
    [SerializeField] private EllipseUI ellipseUI;
    private RectTransform rectTransform;

    private void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
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
}

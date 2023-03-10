using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QPointUI : MonoBehaviour
{
    [SerializeField] private EllipseUI ellipseUI;
    private RectTransform rectTransform;

    private void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateQPointPosition();
    }

    private void OnValidate() 
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateQPointPosition();
    }

    public void UpdateQPointPosition()
    {
        rectTransform.anchoredPosition = ellipseUI.GetPositionRectQPoint();
    }

    public void SetMajorAxis(float yAxisCursor)
    {
        float convertedY = ellipseUI.ConvertScreenPositionInEllipseRect(Vector2.up * yAxisCursor).y;

        ellipseUI.SetSemiMajorAxis(convertedY);

        UpdateQPointPosition();
    }

}

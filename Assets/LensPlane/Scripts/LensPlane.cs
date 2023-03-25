using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class LensPlane : MonoBehaviour
{
    [SerializeField] private EllipseUI ellipseUI;
    [SerializeField] private AxisUI yAxis;
    [SerializeField] private AxisUI xAxis;
    [SerializeField] private float boundaryX;
    [SerializeField] private float boundaryY;
    private RectTransform rectTransform;
    private float width = 0f;
    private float height = 0f;

    private void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        yAxis.SetAxisLength(height);
        xAxis.SetAxisLength(width);

        //InitializeAxisX();
        //InitializeAxisY();
    }

    private void Start() 
    {
        ellipseUI.OnEllipsePositionChanged += OnEllipsePositionChangedHandler;
    }
    
    private void OnValidate() 
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        yAxis.SetAxisLength(height);
        xAxis.SetAxisLength(width);

        //InitializeAxisX();
        //InitializeAxisY();
    }

    private void OnDestroy() 
    {
        ellipseUI.OnEllipsePositionChanged -= OnEllipsePositionChangedHandler;
    }

    /*
    private void InitializeAxisX()
    {
        if (!xAxis) return;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        xAxis.SetPositions(Vector2.left * halfWidth, Vector2.right * halfWidth, true);
    }

    private void InitializeAxisY()
    {
        if (!yAxis) return;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        yAxis.SetPositions(Vector2.down * halfHeight, Vector2.up * halfHeight, true);
    }
    */

    private void OnEllipsePositionChangedHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldCursorPosition)
    {
        if (!CheckPositionInBoundaries(ellipseNewPosition))
        {
            // Convert to the limit position
            Vector2 convertedPosition = ConvertToLimitPosition(ellipseNewPosition);
            // Move the ellipse to this limit position
            ellipseUI.MoveRectPosition(convertedPosition);
            ellipseUI.SetCenterPosition(convertedPosition, false);
        }
    }

    private bool CheckPositionInBoundaries(Vector2 position)
    {
        float limitPositionX = width / 2f - boundaryX;
        float limitPositionY = height / 2f - boundaryY;

        // Check the x coordinate
        if (-limitPositionX <= position.x && position.x <= limitPositionX)
        {
            // Check the y coordinate
            if (-limitPositionY <= position.y && position.y <= limitPositionY)
            {
                return true;
            }
        }

        return false;
    }

    // Change the position so that it is in the boundaries
    private Vector2 ConvertToLimitPosition(Vector2 position)
    {
        Vector2 convertedPosition = position;

        float limitPositionX = width / 2f - boundaryX;
        float limitPositionY = height / 2f - boundaryY;

        if (Mathf.Abs(position.x) > limitPositionX)
        {
            convertedPosition.x = Mathf.Sign(position.x) * limitPositionX;
        }

        if (Mathf.Abs(position.y) > limitPositionY)
        {
            convertedPosition.y = Mathf.Sign(position.y) * limitPositionY;
        }
        
        return convertedPosition;
    }

    public Vector2 ConvertScreenPositionInPlaneRect(Vector2 screenPosition)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, 
            GetComponentInParent<Canvas>().worldCamera, out localPosition);

        return localPosition;
    }

}

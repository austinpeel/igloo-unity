using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class LensPlane : MonoBehaviour
{
    [SerializeField] private EllipseUI ellipseUI;
    [SerializeField] private float xCoordinateMax = 4f;
    [SerializeField] private float yCoordinateMax = 4f;
    [SerializeField] private GridUI gridUI;
    [SerializeField] private AxisUI yAxis;
    [SerializeField] private AxisUI xAxis;
    [SerializeField] private TextMeshProUGUI currentModeText;
    [SerializeField] private float boundaryX;
    [SerializeField] private float boundaryY;
    private RectTransform rectTransform;
    private float width = 0f;
    private float height = 0f;
    private const string SNAP_MODE_TEXT = "Snap mode";
    private const string FREE_MODE_TEXT = "Free mode";

    private void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        yAxis.SetAxisLength(height);
        xAxis.SetAxisLength(width);

        UpdateCurrentModeText();
    }

    private void Start() 
    {
        ellipseUI.OnEllipsePositionChanged += OnEllipsePositionChangedHandler;
        ellipseUI.OnEllipsePositionEndDrag += OnEllipsePositionEndDragHandler;
        ellipseUI.OnEllipseEinsteinChanged += OnEllipseEinsteinChangedHandler;
        ellipseUI.OnEllipseQChanged += OnEllipseQChangedHandler;
    }

    private void Update() 
    {
        if (!ellipseUI) return;

        // Check if the Left Shift Key is hold down and change mode accordingly (when Left Shift key is hold down the ellipse is in Rotation mode)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ellipseUI.SetIsInSnapMode(false);
            UpdateCurrentModeText();
        } 
        
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ellipseUI.SetIsInSnapMode(true);
            UpdateCurrentModeText();
        }
    }
    
    private void OnValidate() 
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        yAxis.SetAxisLength(height);
        xAxis.SetAxisLength(width);

        UpdateCurrentModeText();
    }

    private void OnDestroy() 
    {
        ellipseUI.OnEllipsePositionChanged -= OnEllipsePositionChangedHandler;
        ellipseUI.OnEllipsePositionEndDrag -= OnEllipsePositionEndDragHandler;
        ellipseUI.OnEllipseEinsteinChanged -= OnEllipseEinsteinChangedHandler;
        ellipseUI.OnEllipseQChanged -= OnEllipseQChangedHandler;
    }

    private void UpdateCurrentModeText()
    {
        if (!ellipseUI) return;

        if (ellipseUI.GetIsInSnapMode())
        {
            currentModeText.text = SNAP_MODE_TEXT;
            return;
        }

        currentModeText.text = FREE_MODE_TEXT;
    }

    public float GetEllipseQParameter()
    {
        if (!ellipseUI) return 0f;

        return ellipseUI.GetQParameter();
    }

    public float GetEllipsePhiAngleParameter()
    {
        if (!ellipseUI) return 0f;

        return ellipseUI.GetPhiAngleParameter();
    }

    public float GetEllipseEinsteinRadiusParameter()
    {
        if (!ellipseUI) return 0f;

        return ellipseUI.GetEinsteinRadiusParameter();
    }

    public Vector2 GetEllipseCenterPositionParameter()
    {
        if (!ellipseUI) return Vector2.zero;

        return ellipseUI.GetCenterPositionParameter();
    }

    private void OnEllipseQChangedHandler(Vector2 qNewPosition, Vector2 ellipseOldCursorPosition)
    {
        ellipseUI.SetQWithYAxis(qNewPosition.y);

        // Update the positions of the points parameter
        ellipseUI.UpdatePointsParametersPositions();

        if (ellipseUI.GetIsInSnapMode())
        {
            ellipseUI.MagnetQPoint();
        }
    }

    private void OnEllipseEinsteinChangedHandler(Vector2 einsteinNewPosition, Vector2 ellipseOldCursorPosition)
    {
        ellipseUI.DrawEllipseGivenEinsteinRadiusAndQ(einsteinNewPosition.x, ellipseUI.GetQParameter(), false, false);

        float einsteinInCoord = Utils.ConvertRectPositionToCoordinate(rectTransform, einsteinNewPosition, xCoordinateMax, yCoordinateMax).x;

        ellipseUI.SetEinsteinRadius(einsteinInCoord);
        ellipseUI.SetEinsteinInRect(einsteinNewPosition.x);
                
        // Update the positions of the points parameter
        ellipseUI.UpdatePointsParametersPositions();
    }

    private void OnEllipsePositionChangedHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldCursorPosition)
    {
        // Check if it's outside the boundaries
        if (!CheckPositionInBoundaries(ellipseNewPosition))
        {
            // Convert to the limit position
            Vector2 convertedPosition = ConvertToLimitPosition(ellipseNewPosition);
            Vector2 convertedCoord = Utils.ConvertRectPositionToCoordinate(rectTransform, convertedPosition, xCoordinateMax, yCoordinateMax);
            // Move the ellipse to this limit position
            ellipseUI.MoveRectPosition(convertedPosition);
            ellipseUI.SetCenterPosition(convertedPosition, convertedCoord);
        }
        // Else if it remains inside the boundaries simply, then simply moves the ellipse
        else 
        {
            Vector2 convertedCoord = Utils.ConvertRectPositionToCoordinate(rectTransform, ellipseNewPosition, xCoordinateMax, yCoordinateMax);
            // Move the ellipse to the ellipseNewPosition
            ellipseUI.MoveRectPosition(ellipseNewPosition);
            ellipseUI.SetCenterPosition(ellipseNewPosition, convertedCoord);
        }

        if (ellipseUI.GetIsInSnapMode())
        {
            ellipseUI.MagnetCenterPoint();
        }

        // Display the grid
        if (gridUI)
        {
            gridUI.SetGridVisibility(true);
        }
    }

     private void OnEllipsePositionEndDragHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldCursorPosition)
    {
        // Don't display the grid
        if (gridUI)
        {
            gridUI.SetGridVisibility(false);
        }

        // Put the ellipse back at the old position if it hides any point
        if(!CheckAllEllipsePointsVisibility())
        {
            // Convert the old position
            Vector2 oldConvertedPosition = Utils.ConvertScreenPositionToRect(rectTransform, GetComponentInParent<Canvas>().worldCamera, ellipseOldCursorPosition);
            Vector2 oldConvertedCoord = Utils.ConvertRectPositionToCoordinate(rectTransform, oldConvertedPosition, xCoordinateMax, yCoordinateMax);
            // Move the ellipse to the old position
            ellipseUI.MoveRectPosition(oldConvertedPosition);
            ellipseUI.SetCenterPosition(oldConvertedPosition, oldConvertedCoord);
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

    // Check that all of the button on the ellipse are displayed on the screen
    private bool CheckAllEllipsePointsVisibility()
    {
        Vector2 centerEllipsePosition = ellipseUI.GetCenterPositionParameter();
        float widthLimit = width / 2f;
        float heightLimit = height / 2f;

        // Check for the Q Point
        float qRectDistance = ellipseUI.GetPositionRectQPoint().y;
        float qRotationAngle = ellipseUI.rectTransform.eulerAngles.z;
        Vector2 rotatedQPointPosition = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * qRotationAngle) * qRectDistance, 
                                                    Mathf.Cos(Mathf.Deg2Rad * qRotationAngle) * qRectDistance);
        Vector2 qPointPosition = centerEllipsePosition + rotatedQPointPosition;

        if (!CheckPositionInBoundaries(qPointPosition))
        {
            return false;
        }

        // Check for the Angle Point
        Vector2 anglePointPosition = centerEllipsePosition + rotatedQPointPosition.normalized * (rotatedQPointPosition.magnitude + ellipseUI.GetAnglePointParameterLineLength());

        if (!CheckPositionInBoundaries(anglePointPosition))
        {
            return false;
        }

        // Check for the Einstein Point
        float einsteinRectDistance = ellipseUI.GetPositionRectEinsteinPoint().x;
        float einsteinRotationAngle = ellipseUI.rectTransform.eulerAngles.z;
        Vector2 rotatedEinsteinPointPosition = new Vector2(Mathf.Cos(Mathf.Deg2Rad * einsteinRotationAngle) * einsteinRectDistance, 
                                                    Mathf.Sin(Mathf.Deg2Rad * einsteinRotationAngle) * einsteinRectDistance);
        Vector2 einsteinPointPosition = centerEllipsePosition + rotatedEinsteinPointPosition;

        if (!CheckPositionInBoundaries(einsteinPointPosition))
        {
            return false;
        }

        return true;
    }
}

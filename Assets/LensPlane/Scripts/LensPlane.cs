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
    
    // Reset Parameters
    [Header("Reset Parameters")]
    [SerializeField] private Vector2 centerPositionReset = Vector2.zero;
    [SerializeField] private float qReset = 0.5f;
    [SerializeField] private float einsteinRadiusReset = 1f;
    [SerializeField] private float phiAngleReset = 0f;

    // Convergence Kappa
    [Header("Convergence Kappa")]
    [SerializeField] private Image convergenceMap;
    [SerializeField] private bool displayConvergenceMap = true;
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

        yAxis.SetAxisLength(height, true);
        xAxis.SetAxisLength(width, true);

        UpdateCurrentModeText();
        UpdateConvergenceMap();
    }

    private void Start() 
    {
        ellipseUI.OnEllipsePositionChanged += OnEllipsePositionChangedHandler;
        ellipseUI.OnEllipsePositionEndDrag += OnEllipsePositionEndDragHandler;
        ellipseUI.OnEllipseEinsteinChanged += OnEllipseEinsteinChangedHandler;
        ellipseUI.OnEllipseQChanged += OnEllipseQChangedHandler;
        ellipseUI.OnEllipseAngleChanged += OnEllipseAngleChangedHandler;
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

        yAxis.SetAxisLength(height, false);
        xAxis.SetAxisLength(width, false);

        UpdateCurrentModeText();
        UpdateConvergenceMap();
    }

    private void OnDestroy() 
    {
        ellipseUI.OnEllipsePositionChanged -= OnEllipsePositionChangedHandler;
        ellipseUI.OnEllipsePositionEndDrag -= OnEllipsePositionEndDragHandler;
        ellipseUI.OnEllipseEinsteinChanged -= OnEllipseEinsteinChangedHandler;
        ellipseUI.OnEllipseQChanged -= OnEllipseQChangedHandler;
        ellipseUI.OnEllipseAngleChanged -= OnEllipseAngleChangedHandler;
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

    public void ResetEllipseParameters()
    {
        if (!ellipseUI) return;

        SetEllipseCenterPositionParameter(centerPositionReset);
        SetEllipseEinsteinRadiusParameter(einsteinRadiusReset);
        SetEllipsePhiAngleParameter(phiAngleReset);
        SetEllipseQParameter(qReset);
    }

    // Set the q ratio of the ellipse and redraw it accordingly
    public void SetEllipseQParameter(float newQ)
    {
        if (!ellipseUI) return;

        ellipseUI.SetQ(newQ, true);
    }

    // Get the q ratio of the ellipse
    public float GetEllipseQParameter()
    {
        if (!ellipseUI) return 0f;

        return ellipseUI.GetQParameter();
    }

    // Set the phi angle of the ellipse in degree and redraw it accordingly
    public void SetEllipsePhiAngleParameter(float newAngle)
    {
        if (!ellipseUI) return;

        ellipseUI.SetAngle(newAngle, true);
    }

    // Get the phi angle of the ellipse in degree
    public float GetEllipsePhiAngleParameter()
    {
        if (!ellipseUI) return 0f;

        return ellipseUI.GetPhiAngleParameter();
    }

    // Set the einstein radius in coordinate and redraw the ellipse accordingly
    public void SetEllipseEinsteinRadiusParameter(float newEinsteinRadius)
    {
        if (!ellipseUI) return;

        ellipseUI.SetEinsteinRadius(newEinsteinRadius);

        float einsteinInRect = Utils.ConvertCoordinateToRectPosition(rectTransform, Vector2.right * newEinsteinRadius, xCoordinateMax, yCoordinateMax).x;
        ellipseUI.SetEinsteinInRect(einsteinInRect, true);
    }

    // Get the einstein radius of the ellipse in coordinate
    public float GetEllipseEinsteinRadiusParameter()
    {
        if (!ellipseUI) return 0f;

        return ellipseUI.GetEinsteinRadiusParameter();
    }

    // Set the position of the center of the ellipse in coordinate and redraw the ellipse accordingly
    public void SetEllipseCenterPositionParameter(Vector2 newCenterCoord)
    {
        if (!ellipseUI) return;

        Vector2 centerPositionRect = Utils.ConvertCoordinateToRectPosition(rectTransform, newCenterCoord, xCoordinateMax, yCoordinateMax);
        ellipseUI.SetCenterPosition(centerPositionRect, newCenterCoord, true);
    }

    // Get the position of the center of the ellipse in coordinate
    public Vector2 GetEllipseCenterPositionParameter()
    {
        if (!ellipseUI) return Vector2.zero;

        return ellipseUI.GetCenterPositionParameter();
    }

    private void OnEllipseAngleChangedHandler(Vector2 angleNewPosition, Vector2 ellipseOldCursorPosition)
    {
        // if angleNewPosition.x > 0 => turn clockwise (decrease angle)
        // if angleNewPosition.x < 0 => turn anti-clockwise (increase angle)
        float sign = (angleNewPosition.x > 0) ? -1.0f : 1.0f;

        float deltaAngle = sign * Vector2.Angle(Vector2.up, angleNewPosition.normalized);
        float angle = GetEllipsePhiAngleParameter();

        ellipseUI.SetAngle(angle + deltaAngle, true);
        

        if (!ellipseUI.GetIsInRotationMode())
        {
            ellipseUI.SetIsInRotationMode(true);
            ellipseUI.DisplayRotationLines(true);
        }

        if (ellipseUI.GetIsInSnapMode())
        {
            ellipseUI.MagnetAnglePoint();
        }

        // Update the convergence map (Kappa)
        UpdateConvergenceMap();
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

        // Update the convergence map (Kappa)
        UpdateConvergenceMap();
    }

    private void OnEllipseEinsteinChangedHandler(Vector2 einsteinNewPosition, Vector2 ellipseOldCursorPosition)
    {
        // Convert position in LensPlane Rect
        Vector2 einsteinLensRect = ConvertEllipseRectToLensPlaneRect(einsteinNewPosition);
        // Check if it's outside the boundaries
        if (!CheckPositionInBoundaries(einsteinLensRect))
        {
            // Convert to the limit position
            Vector2 convertedPosition = ConvertToLimitPosition(einsteinLensRect);
            float convertedWidthX = (convertedPosition - ellipseUI.GetCenterPositionRectParameter()).x;
            // Get the RectTransform Einstein radius that corresponds to the position X
            float convertedEinsteinR = ellipseUI.ComputeEinsteinRadius(convertedWidthX, ellipseUI.ComputeMajorAxis(convertedWidthX, GetEllipseQParameter()));

            Vector2 convertedCoord = Utils.ConvertRectPositionToCoordinate(rectTransform, Vector2.right * convertedEinsteinR, xCoordinateMax, yCoordinateMax);
            // Move the ellipse to this limit position
            ellipseUI.SetEinsteinRadius(convertedCoord.x);
            ellipseUI.SetEinsteinInRect(convertedEinsteinR, true);

            // Update the convergence map (Kappa)
            UpdateConvergenceMap();

            return;
        }

        // Get the RectTransform Einstein radius that corresponds to the position X
        float convertedR = ellipseUI.ComputeEinsteinRadius(einsteinNewPosition.x, ellipseUI.ComputeMajorAxis(einsteinNewPosition.x, GetEllipseQParameter()));

        float einsteinInCoord = Utils.ConvertRectPositionToCoordinate(rectTransform, Vector2.right * convertedR, xCoordinateMax, yCoordinateMax).x;

        ellipseUI.SetEinsteinRadius(einsteinInCoord);
        ellipseUI.SetEinsteinInRect(convertedR, true);

        // Update the convergence map (Kappa)
        UpdateConvergenceMap();
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

        // Update the convergence map (Kappa)
        UpdateConvergenceMap();
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

        // Update the convergence map (Kappa)
        UpdateConvergenceMap();
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

    private Vector2 ConvertEllipseRectToLensPlaneRect(Vector2 ellipseRect)
    {
        return ellipseUI.GetCenterPositionRectParameter() + ellipseRect;
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
        Vector2 centerEllipsePosition = ellipseUI.GetCenterPositionRectParameter();
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

    // Compute the convergence Kappa of the SIE profile
    public float KappaSIE(float x, float y)
    {
        // From COOLEST :
        // With the major axis of the ellipsoid along the x axis
        // Kappa = einsteinRadius / (2 * Mathf.sqrt(q * (x^2) + (y^2) / q))

        float einsteinRadius = GetEllipseEinsteinRadiusParameter();
        float q = GetEllipseQParameter();
        float angle = (90f + GetEllipsePhiAngleParameter()) * Mathf.Deg2Rad;

        float rotatedX = x * Mathf.Cos(angle) + y * Mathf.Sin(angle);
        float rotatedY = -x * Mathf.Sin(angle) + y * Mathf.Cos(angle);

        if (x == 0 && y == 0) return float.MaxValue;

        return einsteinRadius / (2f * Mathf.Sqrt(q * (rotatedX*rotatedX) + (rotatedY*rotatedY) / q));
    }

    // Compute the convergence Kappa of the SIS profile
    public float KappaSIS(float x, float y)
    {
        // From COOLEST :
        // Kappa = einsteinRadius / (2 * Mathf.sqrt((x^2) + (y^2)))

        float einsteinRadius = GetEllipseEinsteinRadiusParameter();

        return einsteinRadius / (2f * Mathf.Sqrt((x*x) + (y*y)));
    }

    public void SetDisplayConvergenceMap(bool newDisplayConvergenceMap, bool redraw = false)
    {
        displayConvergenceMap = newDisplayConvergenceMap;

        if (redraw)
        {
            UpdateConvergenceMap();
        }
    }

    public bool GetDisplayConvergenceMap()
    {
        return displayConvergenceMap;
    }

    public void UpdateConvergenceMap()
    {
        if (!convergenceMap) return;

        // If displayConvergenceMap is set to false then clear the map
        if (!displayConvergenceMap)
        {
            ClearConvergenceMap();
            return;
        }

        float xRange = xCoordinateMax * 2f;
        float yRange = yCoordinateMax * 2f;

        int widthInt = ((int)width);
        int heightInt = ((int)height);

        Vector2 centerPosition = GetEllipseCenterPositionParameter();

        Texture2D texture = new Texture2D(widthInt, heightInt);

        Color[] colorsArray = new Color[widthInt * heightInt];

        for (int y = 0; y < heightInt; y++)
        {
            for (int x = 0; x < widthInt; x++)
            {
                float convertedX = (-xCoordinateMax + x * (xRange / widthInt)) - centerPosition.x;
                float convertedY = (-yCoordinateMax + y * (yRange / heightInt)) - centerPosition.y;
                
                colorsArray[y * widthInt + x] = new Color(1f , 0f, 0f, KappaSIE(convertedX,convertedY));
            }
        }

        texture.SetPixels(colorsArray);
        texture.Apply();

        convergenceMap.sprite = Sprite.Create(texture, new Rect(0, 0, widthInt, heightInt), Vector2.one * 0.5f);
    }

    public void ClearConvergenceMap()
    {
        if (!convergenceMap) return;

        float xRange = xCoordinateMax * 2f;
        float yRange = yCoordinateMax * 2f;

        int widthInt = ((int)width);
        int heightInt = ((int)height);

        Vector2 centerPosition = GetEllipseCenterPositionParameter();

        Texture2D texture = new Texture2D(widthInt, heightInt);

        convergenceMap.sprite = Sprite.Create(texture, new Rect(0, 0, widthInt, heightInt), Vector2.one * 0.5f);
    }
}

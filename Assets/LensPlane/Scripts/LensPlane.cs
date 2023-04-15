using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static DestroyUtils;

[RequireComponent(typeof(RectTransform))]
public class LensPlane : MonoBehaviour, ICoordinateConverter
{
    [SerializeField] private LensEllipseUI lensEllipseUI;
    [SerializeField] private float xCoordinateMax = 4f;
    [SerializeField] private float yCoordinateMax = 4f;
    [SerializeField] private GridUI gridUI;
    [SerializeField] private AxisUI yAxis;
    [SerializeField] private AxisUI xAxis;
    [SerializeField] private TextMeshProUGUI currentModeText;
    [SerializeField] private float boundaryX;
    [SerializeField] private float boundaryY;

    // Convergence Kappa
    [Header("Convergence Kappa")]
    [SerializeField] private Image convergenceMap;
    [SerializeField] private bool displayConvergenceMap = true;
    [SerializeField] private GameObject ellipsesKappaParent;
    [SerializeField] private GameObject ellipsePrefab;
    [SerializeField] private bool displayEllipsesConvergenceMap = true;
    private RectTransform rectTransform;
    private float width = 0f;
    private float height = 0f;
    private const string SNAP_MODE_TEXT = "Snap mode";
    private const string FREE_MODE_TEXT = "Free mode";
    private float[] ellipsesKappaEinsteinArray = new float[10];

    private void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        yAxis.SetAxisLength(height, true);
        xAxis.SetAxisLength(width, true);

        UpdateCurrentModeText();
        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    private void Start() 
    {
        lensEllipseUI.OnEllipsePositionChanged += OnEllipsePositionChangedHandler;
        lensEllipseUI.OnEllipsePositionEndDrag += OnEllipsePositionEndDragHandler;
        lensEllipseUI.OnEllipseEinsteinChanged += OnEllipseEinsteinChangedHandler;
        lensEllipseUI.OnEllipseQChanged += OnEllipseQChangedHandler;
        lensEllipseUI.OnEllipseAngleChanged += OnEllipseAngleChangedHandler;
    }

    private void Update() 
    {
        if (!lensEllipseUI) return;

        // Check if the Left Shift Key is hold down and change mode accordingly (when Left Shift key is hold down the ellipse is in Rotation mode)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            lensEllipseUI.SetIsInSnapMode(false);
            UpdateCurrentModeText();
        } 
        
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            lensEllipseUI.SetIsInSnapMode(true);
            UpdateCurrentModeText();
        }
    }
    
    private void OnValidate() 
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
        /*

        yAxis.SetAxisLength(height, false);
        xAxis.SetAxisLength(width, false);

        UpdateCurrentModeText();
        UpdateConvergenceMap();

        if (!ellipsesKappaParent || !ellipsePrefab) return;

        DrawEllipsesKappa();
        */
    }

    private void OnDestroy() 
    {
        lensEllipseUI.OnEllipsePositionChanged -= OnEllipsePositionChangedHandler;
        lensEllipseUI.OnEllipsePositionEndDrag -= OnEllipsePositionEndDragHandler;
        lensEllipseUI.OnEllipseEinsteinChanged -= OnEllipseEinsteinChangedHandler;
        lensEllipseUI.OnEllipseQChanged -= OnEllipseQChangedHandler;
        lensEllipseUI.OnEllipseAngleChanged -= OnEllipseAngleChangedHandler;
    }

    private void UpdateCurrentModeText()
    {
        if (!lensEllipseUI || !currentModeText) return;

        if (lensEllipseUI.GetIsInSnapMode())
        {
            currentModeText.text = SNAP_MODE_TEXT;
            return;
        }

        currentModeText.text = FREE_MODE_TEXT;
    }

    public void ResetEllipseParameters()
    {
        if (!lensEllipseUI) return;

        lensEllipseUI.ResetParameters();

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    // Set the q ratio of the ellipse and redraw it accordingly
    public void SetEllipseQParameter(float newQ)
    {
        if (!lensEllipseUI) return;

        lensEllipseUI.SetQ(newQ, true);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    // Get the q ratio of the ellipse
    public float GetEllipseQParameter()
    {
        if (!lensEllipseUI) return 0f;

        return lensEllipseUI.GetQParameter();
    }

    // Set the phi angle of the ellipse in degree and redraw it accordingly
    public void SetEllipsePhiAngleParameter(float newAngle)
    {
        if (!lensEllipseUI) return;

        lensEllipseUI.SetAngle(newAngle, true);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    // Get the phi angle of the ellipse in degree
    public float GetEllipseAngleParameter()
    {
        if (!lensEllipseUI) return 0f;

        return lensEllipseUI.GetAngleParameter();
    }

    // Set the einstein radius in coordinate and redraw the ellipse accordingly
    public void SetEllipseEinsteinRadiusParameter(float newEinsteinRadius)
    {
        if (!lensEllipseUI) return;

        lensEllipseUI.SetEinsteinRadius(newEinsteinRadius, true);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    // Get the einstein radius of the ellipse in coordinate
    public float GetEllipseEinsteinRadiusParameter()
    {
        if (!lensEllipseUI) return 0f;

        return lensEllipseUI.GetEinsteinRadiusParameter();
    }

    // Set the position of the center of the ellipse in coordinate and redraw the ellipse accordingly
    public void SetEllipseCenterPositionParameter(Vector2 newCenterCoord)
    {
        if (!lensEllipseUI) return;

        lensEllipseUI.SetCenterPosition(newCenterCoord, true);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    // Get the position of the center of the ellipse in coordinate
    public Vector2 GetEllipseCenterPositionParameter()
    {
        if (!lensEllipseUI) return Vector2.zero;

        return lensEllipseUI.GetCenterPositionParameter();
    }

    private void OnEllipseAngleChangedHandler(Vector2 angleNewPosition, Vector2 ellipseOldCursorPosition)
    {
        // if angleNewPosition.x > 0 => turn clockwise (decrease angle)
        // if angleNewPosition.x < 0 => turn anti-clockwise (increase angle)
        float sign = (angleNewPosition.x > 0) ? -1.0f : 1.0f;

        float deltaAngle = sign * Vector2.Angle(Vector2.up, angleNewPosition.normalized);
        float angle = GetEllipseAngleParameter();

        lensEllipseUI.SetAngle(angle + deltaAngle, true);
        

        if (!lensEllipseUI.GetIsInRotationMode())
        {
            lensEllipseUI.SetIsInRotationMode(true);
            lensEllipseUI.DisplayRotationLines(true);
        }

        if (lensEllipseUI.GetIsInSnapMode())
        {
            lensEllipseUI.MagnetAnglePoint();
        }

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    private void OnEllipseQChangedHandler(Vector2 qNewPosition, Vector2 ellipseOldCursorPosition)
    {
        lensEllipseUI.SetQWithYAxis(qNewPosition.y);

        // Update the positions of the points parameter
        lensEllipseUI.UpdatePointsParametersPositions();

        if (lensEllipseUI.GetIsInSnapMode())
        {
            lensEllipseUI.MagnetQPoint();
        }

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
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
            float convertedWidthX = (convertedPosition - lensEllipseUI.GetCenterPositionInRect()).x;
            // Get the RectTransform Einstein radius that corresponds to the position X
            float convertedEinsteinR = lensEllipseUI.ComputeEinsteinRadius(convertedWidthX, lensEllipseUI.ComputeMajorAxis(convertedWidthX, GetEllipseQParameter()));

            Vector2 convertedCoord = Utils.ConvertRectPositionToCoordinate(rectTransform, Vector2.right * convertedEinsteinR, xCoordinateMax, yCoordinateMax);
            // Move the ellipse to this limit position
            lensEllipseUI.SetEinsteinRadius(convertedCoord.x, true);

            // Update the convergence map and the convergence ellipses (Kappa)
            UpdateConvergenceKappa();

            return;
        }

        // Get the RectTransform Einstein radius that corresponds to the position X
        float convertedR = lensEllipseUI.ComputeEinsteinRadius(einsteinNewPosition.x, lensEllipseUI.ComputeMajorAxis(einsteinNewPosition.x, GetEllipseQParameter()));

        float einsteinInCoord = Utils.ConvertRectPositionToCoordinate(rectTransform, Vector2.right * convertedR, xCoordinateMax, yCoordinateMax).x;

        lensEllipseUI.SetEinsteinRadius(einsteinInCoord, true);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
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
            lensEllipseUI.SetCenterPosition(convertedCoord, true);
        }
        // Else if it remains inside the boundaries simply, then simply moves the ellipse
        else 
        {
            Vector2 convertedCoord = Utils.ConvertRectPositionToCoordinate(rectTransform, ellipseNewPosition, xCoordinateMax, yCoordinateMax);
            // Move the ellipse to the ellipseNewPosition
            lensEllipseUI.SetCenterPosition(convertedCoord, true);
        }

        if (lensEllipseUI.GetIsInSnapMode())
        {
            lensEllipseUI.MagnetCenterPoint();
        }

        // Display the grid
        if (gridUI)
        {
            gridUI.SetGridVisibility(true);
        }

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
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
            lensEllipseUI.SetCenterPosition(oldConvertedCoord, true);
        }

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
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

    public Vector2 ConvertCoordinateToRectPosition(Vector2 coordinate)
    {
        return Utils.ConvertCoordinateToRectPosition(rectTransform, coordinate, xCoordinateMax, yCoordinateMax);
    }

    private Vector2 ConvertEllipseRectToLensPlaneRect(Vector2 ellipseRect)
    {
        return lensEllipseUI.GetCenterPositionInRect() + ellipseRect;
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
        Vector2 centerEllipsePosition = lensEllipseUI.GetCenterPositionInRect();
        float widthLimit = width / 2f;
        float heightLimit = height / 2f;

        // Check for the Q Point
        float qRectDistance = lensEllipseUI.GetPositionRectQPoint().y;
        float qRotationAngle = lensEllipseUI.rectTransform.eulerAngles.z;
        Vector2 rotatedQPointPosition = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * qRotationAngle) * qRectDistance, 
                                                    Mathf.Cos(Mathf.Deg2Rad * qRotationAngle) * qRectDistance);
        Vector2 qPointPosition = centerEllipsePosition + rotatedQPointPosition;

        if (!CheckPositionInBoundaries(qPointPosition))
        {
            return false;
        }

        // Check for the Angle Point
        Vector2 anglePointPosition = centerEllipsePosition + rotatedQPointPosition.normalized * (rotatedQPointPosition.magnitude + lensEllipseUI.GetAnglePointParameterLineLength());

        if (!CheckPositionInBoundaries(anglePointPosition))
        {
            return false;
        }

        // Check for the Einstein Point
        float einsteinRectDistance = lensEllipseUI.GetPositionRectEinsteinPoint().x;
        float einsteinRotationAngle = lensEllipseUI.rectTransform.eulerAngles.z;
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
        float angle = (90f + GetEllipseAngleParameter()) * Mathf.Deg2Rad;

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

    private float ComputeEinsteinRadiusFromKappa(float kappa)
    {
        if (!lensEllipseUI) return 0f;

        // With the major axis of the ellipsoid along the x axis
        // Kappa = einsteinRadius / (2 * Mathf.sqrt(q * (x^2) + (y^2) / q))
        // Thus :
        // Mathf.sqrt((einsteinRadius / (2 * Kappa))^2 * q) = y, with x = 0
        float einsteinRadius = GetEllipseEinsteinRadiusParameter();
        float q = GetEllipseQParameter();

        float minorAxis = Mathf.Sqrt(Mathf.Pow(einsteinRadius / (2 * kappa), 2f) * q);

        return lensEllipseUI.ComputeEinsteinRadius(minorAxis, lensEllipseUI.ComputeMajorAxis(minorAxis, q));  
    }

    private void FillEinsteinEllipsesKappaArray()
    {
        for (int i = 0; i < ellipsesKappaEinsteinArray.Length; i++)
        {
            ellipsesKappaEinsteinArray[i] = ComputeEinsteinRadiusFromKappa(0.1f * (i+1));
        }
    }

    // Draw Ellipses where Kappa equals 0.1, 0.2, 0.3 ... 0.9, 1.0
    public void DrawEllipsesKappa()
    {
        float einsteinRadius = GetEllipseEinsteinRadiusParameter();
        float q = GetEllipseQParameter();
        float angle = GetEllipseAngleParameter();
        Vector2 centerPosition = GetEllipseCenterPositionParameter();

        FillEinsteinEllipsesKappaArray();

        for (int i = 0; i < ellipsesKappaEinsteinArray.Length; i++)
        {
            EllipseUI ellipseKappa = Instantiate(ellipsePrefab, ellipsesKappaParent.transform).GetComponent<EllipseUI>();
            ellipseKappa.SetQ(q);
            ellipseKappa.SetEinsteinRadius(ellipsesKappaEinsteinArray[i], true);
            ellipseKappa.SetAngle(angle, true);
            ellipseKappa.SetCenterPosition(centerPosition, true);
        }
    }

    public void ClearEllipsesKappa()
    {
        if (!ellipsesKappaParent) return;

        // Don't know why it doesn't destroy all children in 1 pass (?) but it works
        // Check that there is still one child (Label of the axis)
        while (ellipsesKappaParent.transform.childCount > 0)
        {
            foreach (Transform child in ellipsesKappaParent.transform)
            {
                SafeDestroy(child.gameObject);
            }
        }
    }

    public void UpdateEllipsesKappa()
    {
        if (!ellipsesKappaParent || !ellipsePrefab) return;

        ClearEllipsesKappa();

        if (!displayEllipsesConvergenceMap) return;

        DrawEllipsesKappa();
    }

    public void UpdateConvergenceKappa()
    {
        UpdateConvergenceMap();
        UpdateEllipsesKappa();
    }

    // --------------------- USED IN LENS ELLIPSE EDITOR ---------------------
    public void SetLensEllipseUI(LensEllipseUI newLensEllipseUI)
    {
        lensEllipseUI = newLensEllipseUI;
    }

    public LensEllipseUI GetLensEllipseUI()
    {
        return lensEllipseUI;
    }

    public void SetXCoordinateMax(float newXCoordinateMax, bool redraw = false)
    {
        xCoordinateMax = newXCoordinateMax;

        if (redraw)
        {
            if (!xAxis) return;

            xAxis.SetMaxValue(xCoordinateMax, true);
        }
    }

    public float GetXCoordinateMax()
    {
        return xCoordinateMax;
    }

    public void SetYCoordinateMax(float newYCoordinateMax, bool redraw = false)
    {
        yCoordinateMax = newYCoordinateMax;

        if (redraw)
        {
            if (!yAxis) return;

            yAxis.SetMaxValue(yCoordinateMax, true);
        }
    }

    public float GetYCoordinateMax()
    {
        return yCoordinateMax;
    }

    public void SetGridUI(GridUI newGridUI)
    {
        gridUI = newGridUI;
    }

    public GridUI GetGridUI()
    {
        return gridUI;
    }

    public void SetYAxis(AxisUI newYAxis)
    {
        yAxis = newYAxis;
    }

    public AxisUI GetYAxis()
    {
        return yAxis;
    }

    public void SetXAxis(AxisUI newXAxis)
    {
        xAxis = newXAxis;
    }

    public AxisUI GetXAxis()
    {
        return xAxis;
    }

    public void SetCurrentModeText(TextMeshProUGUI newCurrentModeText, bool redraw = false)
    {
        currentModeText = newCurrentModeText;

        if (redraw)
        {
            UpdateCurrentModeText();
        }
    }

    public TextMeshProUGUI GetCurrentModeText()
    {
        return currentModeText;
    }

    public void SetBoundaryX(float newBoundaryX)
    {
        boundaryX = newBoundaryX;
    }

    public float GetBoundaryX()
    {
        return boundaryX;
    }

    public void SetBoundaryY(float newBoundaryY)
    {
        boundaryY = newBoundaryY;
    }

    public float GetBoundaryY()
    {
        return boundaryY;
    }

    public void SetConvergenceMap(Image newConvergenceMap, bool redraw = false)
    {
        convergenceMap = newConvergenceMap;

        if (redraw)
        {
            UpdateConvergenceMap();
        }
    }

    public Image GetConvergenceMap()
    {
        return convergenceMap;
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

    public void SetEllipsesKappaParent(GameObject newEllipsesKappaParent, bool redraw = false)
    {
        ellipsesKappaParent = newEllipsesKappaParent;

        if (redraw)
        {
            UpdateEllipsesKappa();
        }
    }

    public GameObject GetEllipseKappaParent()
    {
        return ellipsesKappaParent;
    }

    public void SetEllipsePrefab(GameObject newEllipsePrefab, bool redraw = false)
    {
        ellipsePrefab = newEllipsePrefab;

        if (redraw)
        {
            UpdateEllipsesKappa();
        }
    }

    public GameObject GetEllipsePrefab()
    {
        return ellipsePrefab;
    }

    public void SetDisplayEllipsesConvergenceMap(bool newDisplayEllipsesConvergenceMap, bool redraw = false)
    {
        displayEllipsesConvergenceMap = newDisplayEllipsesConvergenceMap;

        if (redraw)
        {
            UpdateEllipsesKappa();
        }
    }

    public bool GetDisplayEllipsesConvergenceMap()
    {
        return displayEllipsesConvergenceMap;
    }
}

using UnityEngine;
using UnityEngine.UI;
using static DestroyUtils;

[RequireComponent(typeof(RectTransform))]
public class LensPlane : Plane
{
    [SerializeField] private InteractableEllipseUI interactEllipseUI;

    // Convergence Kappa
    [Header("Convergence Kappa")]
    [SerializeField] private Color colorConvergenceMap = Color.red;
    [SerializeField] private Image convergenceMap;
    [SerializeField] private bool displayConvergenceMap = true;
    [SerializeField] private Image convergenceColorScale;
    [SerializeField] private GameObject colorScaleOutline;
    [SerializeField] private bool displayConvergenceColorScale = true;
    [SerializeField] private GameObject ellipsesKappaParent;
    [SerializeField] private GameObject ellipsePrefab;
    [SerializeField] private bool displayEllipsesConvergenceMap = true;
    
    private float[] ellipsesKappaEinsteinArray = new float[10];

    protected new void Awake() 
    {
        base.Awake();

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    private void Start() 
    {
        interactEllipseUI.OnEllipsePositionChanged += OnEllipsePositionChangedHandler;
        interactEllipseUI.OnEllipsePositionEndDrag += OnEllipsePositionEndDragHandler;
        interactEllipseUI.OnEllipseEinsteinChanged += OnEllipseEinsteinChangedHandler;
        interactEllipseUI.OnEllipseQChanged += OnEllipseQChangedHandler;
        interactEllipseUI.OnEllipseAngleChanged += OnEllipseAngleChangedHandler;
    }

    protected new void Update() 
    {
        if (!interactEllipseUI) return;

        base.Update();

        if (GetHasModeChanged())
        {
            interactEllipseUI.SetIsInSnapMode(GetIsInSnapMode());
        }
    }

    private void OnDestroy() 
    {
        interactEllipseUI.OnEllipsePositionChanged -= OnEllipsePositionChangedHandler;
        interactEllipseUI.OnEllipsePositionEndDrag -= OnEllipsePositionEndDragHandler;
        interactEllipseUI.OnEllipseEinsteinChanged -= OnEllipseEinsteinChangedHandler;
        interactEllipseUI.OnEllipseQChanged -= OnEllipseQChangedHandler;
        interactEllipseUI.OnEllipseAngleChanged -= OnEllipseAngleChangedHandler;
    }

    public void ResetEllipseParameters()
    {
        if (!interactEllipseUI) return;

        interactEllipseUI.ResetParameters();

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    // Set the q ratio of the ellipse and redraw it accordingly
    public void SetEllipseQParameter(float newQ)
    {
        if (!interactEllipseUI) return;

        interactEllipseUI.SetQ(newQ, true);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    // Get the q ratio of the ellipse
    public float GetEllipseQParameter()
    {
        if (!interactEllipseUI) return 0f;

        return interactEllipseUI.GetQParameter();
    }

    // Set the phi angle of the ellipse in degree and redraw it accordingly
    public void SetEllipsePhiAngleParameter(float newAngle)
    {
        if (!interactEllipseUI) return;

        interactEllipseUI.SetAngle(newAngle, true);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    // Get the phi angle of the ellipse in degree
    public float GetEllipseAngleParameter()
    {
        if (!interactEllipseUI) return 0f;

        return interactEllipseUI.GetAngleParameter();
    }

    // Set the einstein radius in coordinate and redraw the ellipse accordingly
    public void SetEllipseEinsteinRadiusParameter(float newEinsteinRadius)
    {
        if (!interactEllipseUI) return;

        interactEllipseUI.SetEinsteinRadius(newEinsteinRadius, true);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    // Get the einstein radius of the ellipse in coordinate
    public float GetEllipseEinsteinRadiusParameter()
    {
        if (!interactEllipseUI) return 0f;

        return interactEllipseUI.GetEinsteinRadiusParameter();
    }

    // Set the position of the center of the ellipse in coordinate and redraw the ellipse accordingly
    public void SetEllipseCenterPositionParameter(Vector2 newCenterCoord)
    {
        if (!interactEllipseUI) return;

        interactEllipseUI.SetCenterPosition(newCenterCoord, true);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    // Get the position of the center of the ellipse in coordinate
    public Vector2 GetEllipseCenterPositionParameter()
    {
        if (!interactEllipseUI) return Vector2.zero;

        return interactEllipseUI.GetCenterPositionParameter();
    }

    private void OnEllipseAngleChangedHandler(Vector2 angleNewPosition, Vector2 ellipseOldCursorPosition)
    {
        // if angleNewPosition.x > 0 => turn clockwise (decrease angle)
        // if angleNewPosition.x < 0 => turn anti-clockwise (increase angle)
        float sign = (angleNewPosition.x > 0) ? -1.0f : 1.0f;

        float deltaAngle = sign * Vector2.Angle(Vector2.up, angleNewPosition.normalized);
        float angle = GetEllipseAngleParameter();

        interactEllipseUI.SetAngle(angle + deltaAngle, true);
        

        if (!interactEllipseUI.GetIsInRotationMode())
        {
            interactEllipseUI.SetIsInRotationMode(true);
            interactEllipseUI.DisplayRotationLines(true);
        }

        if (interactEllipseUI.GetIsInSnapMode())
        {
            interactEllipseUI.MagnetAnglePoint();
        }

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    private void OnEllipseQChangedHandler(Vector2 qNewPosition, Vector2 ellipseOldCursorPosition)
    {
        interactEllipseUI.SetQWithYAxis(qNewPosition.y);

        // Update the positions of the points parameter
        interactEllipseUI.UpdatePointsParametersPositions();

        if (interactEllipseUI.GetIsInSnapMode())
        {
            interactEllipseUI.MagnetQPoint();
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
            float convertedWidthX = (convertedPosition - interactEllipseUI.GetCenterPositionInRect()).x;
            // Get the RectTransform Einstein radius that corresponds to the position X
            float convertedEinsteinR = interactEllipseUI.ComputeEinsteinRadius(convertedWidthX, interactEllipseUI.ComputeMajorAxis(convertedWidthX, GetEllipseQParameter()));

            Vector2 convertedCoord = ConvertRectPositionToCoordinate(Vector2.right * convertedEinsteinR);
            // Move the ellipse to this limit position
            interactEllipseUI.SetEinsteinRadius(convertedCoord.x, true);

            // Update the convergence map and the convergence ellipses (Kappa)
            UpdateConvergenceKappa();

            return;
        }

        // Get the RectTransform Einstein radius that corresponds to the position X
        float convertedR = interactEllipseUI.ComputeEinsteinRadius(einsteinNewPosition.x, interactEllipseUI.ComputeMajorAxis(einsteinNewPosition.x, GetEllipseQParameter()));

        float einsteinInCoord = ConvertRectPositionToCoordinate(Vector2.right * convertedR).x;

        interactEllipseUI.SetEinsteinRadius(einsteinInCoord, true);

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
            Vector2 convertedCoord = ConvertRectPositionToCoordinate(convertedPosition);
            // Move the ellipse to this limit position
            interactEllipseUI.SetCenterPosition(convertedCoord, true);
        }
        // Else if it remains inside the boundaries simply, then simply moves the ellipse
        else 
        {
            Vector2 convertedCoord = ConvertRectPositionToCoordinate(ellipseNewPosition);
            // Move the ellipse to the ellipseNewPosition
            interactEllipseUI.SetCenterPosition(convertedCoord, true);
        }

        if (interactEllipseUI.GetIsInSnapMode())
        {
            interactEllipseUI.MagnetCenterPoint();
        }

        // Display the grid
        GridUI gridUI = GetGridUI();
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
        GridUI gridUI = GetGridUI();
        if (gridUI)
        {
            gridUI.SetGridVisibility(false);
        }

        // Put the ellipse back at the old position if it hides any point
        if(!CheckAllEllipsePointsVisibility())
        {
            // Convert the old position
            Vector2 oldConvertedPosition = ConversionUtils.ConvertScreenPositionToRect(rectTransform, GetComponentInParent<Canvas>().worldCamera, ellipseOldCursorPosition);
            Vector2 oldConvertedCoord = ConvertRectPositionToCoordinate(oldConvertedPosition);
            // Move the ellipse to the old position
            interactEllipseUI.SetCenterPosition(oldConvertedCoord, true);
        }

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();
    }

    private bool CheckPositionInBoundaries(Vector2 position)
    {
        float limitPositionX = width / 2f - GetBoundaryX();
        float limitPositionY = height / 2f - GetBoundaryY();

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

        float limitPositionX = width / 2f - GetBoundaryX();
        float limitPositionY = height / 2f - GetBoundaryY();

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
        return interactEllipseUI.GetCenterPositionInRect() + ellipseRect;
    }

    // Check that all of the button on the ellipse are displayed on the screen
    private bool CheckAllEllipsePointsVisibility()
    {
        Vector2 centerEllipsePosition = interactEllipseUI.GetCenterPositionInRect();
        float widthLimit = width / 2f;
        float heightLimit = height / 2f;

        // Check for the Q Point
        float qRectDistance = interactEllipseUI.GetPositionRectQPoint().y;
        float qRotationAngle = interactEllipseUI.rectTransform.eulerAngles.z;
        Vector2 rotatedQPointPosition = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * qRotationAngle) * qRectDistance, 
                                                    Mathf.Cos(Mathf.Deg2Rad * qRotationAngle) * qRectDistance);
        Vector2 qPointPosition = centerEllipsePosition + rotatedQPointPosition;

        if (!CheckPositionInBoundaries(qPointPosition))
        {
            return false;
        }

        // Check for the Angle Point
        Vector2 anglePointPosition = centerEllipsePosition + rotatedQPointPosition.normalized * (rotatedQPointPosition.magnitude + interactEllipseUI.GetAnglePointParameterLineLength());

        if (!CheckPositionInBoundaries(anglePointPosition))
        {
            return false;
        }

        // Check for the Einstein Point
        float einsteinRectDistance = interactEllipseUI.GetPositionRectEinsteinPoint().x;
        float einsteinRotationAngle = interactEllipseUI.rectTransform.eulerAngles.z;
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
        float einsteinRadius = GetEllipseEinsteinRadiusParameter();
        float q = GetEllipseQParameter();

        return LensProfiles.KappaSIE(x, y, einsteinRadius, q, GetEllipseAngleParameter());
    }

    // Compute the convergence Kappa of the SIS profile
    public float KappaSIS(float x, float y)
    {
        float einsteinRadius = GetEllipseEinsteinRadiusParameter();

        return LensProfiles.KappaSIS(x, y, einsteinRadius);
    }

    public void UpdateConvergenceMap()
    {
        if (!convergenceMap) return;

        convergenceMap.gameObject.SetActive(displayConvergenceMap);

        // If displayConvergenceMap is set to false then clear the map
        if (!displayConvergenceMap)
        {
            ClearConvergenceMap();
            return;
        }

        float xCoordinateMax = GetXCoordinateMax();
        float yCoordinateMax = GetYCoordinateMax();

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

                colorsArray[y * widthInt + x] = new Color(colorConvergenceMap.r , colorConvergenceMap.g, colorConvergenceMap.b, KappaSIE(convertedX,convertedY));
            }
        }

        texture.SetPixels(colorsArray);
        texture.Apply();

        convergenceMap.sprite = Sprite.Create(texture, new Rect(0, 0, widthInt, heightInt), Vector2.one * 0.5f);
    }

    public void ClearConvergenceMap()
    {
        if (!convergenceMap) return;

        float xCoordinateMax = GetXCoordinateMax();
        float yCoordinateMax = GetYCoordinateMax();

        float xRange = xCoordinateMax * 2f;
        float yRange = yCoordinateMax * 2f;

        int widthInt = ((int)width);
        int heightInt = ((int)height);

        Vector2 centerPosition = GetEllipseCenterPositionParameter();

        Texture2D texture = new Texture2D(widthInt, heightInt);

        convergenceMap.sprite = Sprite.Create(texture, new Rect(0, 0, widthInt, heightInt), Vector2.one * 0.5f);
    }

    public void UpdateConvergenceColorScale()
    {
        if (!convergenceColorScale) return;

        // Use displayConvergenceColorScale to update accordingly the outline
        if (colorScaleOutline != null) colorScaleOutline.SetActive(displayConvergenceColorScale);

        foreach (Transform child in colorScaleOutline.transform)
        {
            child.gameObject.SetActive(displayConvergenceColorScale);
        }

        // If displayConvergenceColorScale is set to false then clear the color scale
        if (!displayConvergenceColorScale)
        {
            ClearConvergenceColorScale();
            return;
        }

        int widthInt = ((int)convergenceColorScale.rectTransform.rect.width);
        int heightInt = ((int)convergenceColorScale.rectTransform.rect.height);

        Texture2D texture = new Texture2D(widthInt, heightInt);
        texture.wrapMode = TextureWrapMode.Clamp;

        Color[] colorsArray = new Color[widthInt * heightInt];
        Color colorForX = colorConvergenceMap;

        for (int x = 0; x < widthInt; x++)
        {
            colorForX.a = (x)/((float)widthInt - 1);
            for (int y = 0; y < heightInt; y++)
            {
                colorsArray[y * widthInt + x] = colorForX;
            }
        }

        texture.SetPixels(colorsArray);
        texture.Apply();

        convergenceColorScale.sprite = Sprite.Create(texture, new Rect(0, 0, widthInt, heightInt), Vector2.one * 0.5f);
    }

    public void ClearConvergenceColorScale()
    {
        if (!convergenceColorScale) return;

        int widthInt = ((int)convergenceColorScale.rectTransform.rect.width);
        int heightInt = ((int)convergenceColorScale.rectTransform.rect.height);

        Texture2D texture = new Texture2D(widthInt, heightInt);

        convergenceColorScale.sprite = Sprite.Create(texture, new Rect(0, 0, widthInt, heightInt), Vector2.one * 0.5f);
    }

    private float ComputeEinsteinRadiusFromKappa(float kappa)
    {
        if (!interactEllipseUI) return 0f;

        // With the major axis of the ellipsoid along the x axis
        // Kappa = einsteinRadius / (2 * Mathf.sqrt(q * (x^2) + (y^2) / q))
        // Thus :
        // Mathf.sqrt((einsteinRadius / (2 * Kappa))^2 * q) = y, with x = 0
        float einsteinRadius = GetEllipseEinsteinRadiusParameter();
        float q = GetEllipseQParameter();

        float minorAxis = Mathf.Sqrt(Mathf.Pow(einsteinRadius / (2 * kappa), 2f) * q);

        return interactEllipseUI.ComputeEinsteinRadius(minorAxis, interactEllipseUI.ComputeMajorAxis(minorAxis, q));  
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

        if (ellipsesKappaParent.transform.childCount > 0)
        {
            for (int i = ellipsesKappaParent.transform.childCount; i > 0; i--)
            {
                SafeDestroy(ellipsesKappaParent.transform.GetChild(0).gameObject);
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
        UpdateConvergenceColorScale();
        UpdateEllipsesKappa();
    }

    // --------------------- USED IN LENS PLANE EDITOR ---------------------
    public void SetInteractEllipseUI(InteractableEllipseUI newInteractEllipseUI)
    {
        interactEllipseUI = newInteractEllipseUI;
    }

    public InteractableEllipseUI GetInteractEllipseUI()
    {
        return interactEllipseUI;
    }

    public void SetColorConvergenceMap(Color newColorConvergenceMap, bool redraw = false)
    {
        colorConvergenceMap = newColorConvergenceMap;

        if (redraw)
        {
            UpdateConvergenceKappa();
        }
    }

    public Color GetColorConvergenceMap()
    {
        return colorConvergenceMap;
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

    public void SetConvergenceColorScale(Image newConvergenceColorScale, bool redraw = false)
    {
        convergenceColorScale = newConvergenceColorScale;

        if (redraw)
        {
            UpdateConvergenceColorScale();
        }
    }

    public Image GetConvergenceColorScale()
    {
        return convergenceColorScale;
    }

    public void SetColorScaleOutline(GameObject newColorScaleOutline, bool redraw = false)
    {
        colorScaleOutline = newColorScaleOutline;

        if (redraw)
        {
            UpdateConvergenceColorScale();
        }
    }

    public GameObject GetColorScaleOutline()
    {
        return colorScaleOutline;
    }

    public void SetDisplayConvergenceColorScale(bool newDisplayConvergenceColorScale, bool redraw = false)
    {
        displayConvergenceColorScale = newDisplayConvergenceColorScale;

        if (redraw)
        {
            UpdateConvergenceColorScale();
        }
    }

    public bool GetDisplayConvergenceColorScale()
    {
        return displayConvergenceColorScale;
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

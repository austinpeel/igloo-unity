using UnityEngine;
using UnityEngine.UI;
using static DestroyUtils;

public class LensPlane : PlaneInteractableEllipse
{
    [Header("Scriptable Object")]
    [SerializeField] private LensParameters lensParameters;

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

        // Save the lens parameters in the ScriptableObject
        SaveLensParameters();
    }

    public new void ResetEllipseParameters()
    {
        base.ResetEllipseParameters();

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();

        // Save the lens parameters in the ScriptableObject
        SaveLensParameters();
    }

    public void SaveLensParameters()
    {
        if (!lensParameters) return;

        lensParameters.q = GetEllipseQParameter();
        lensParameters.einsteinRadius = GetEllipseEinsteinRadiusParameter();
        lensParameters.angle = GetEllipseAngleParameter();
        lensParameters.centerPosition = GetEllipseCenterPositionParameter();
    }

    // Set the q ratio of the ellipse and redraw it accordingly
    public new void SetEllipseQParameter(float newQ)
    {
        base.SetEllipseQParameter(newQ);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();

        // Save the lens parameters in the ScriptableObject
        SaveLensParameters();
    }

    // Set the phi angle of the ellipse in degree and redraw it accordingly
    public new void SetEllipsePhiAngleParameter(float newAngle)
    {
        base.SetEllipsePhiAngleParameter(newAngle);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();

        // Save the lens parameters in the ScriptableObject
        SaveLensParameters();
    }

    // Set the einstein radius in coordinate and redraw the ellipse accordingly
    public new void SetEllipseEinsteinRadiusParameter(float newEinsteinRadius)
    {
        base.SetEllipseEinsteinRadiusParameter(newEinsteinRadius);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();

        // Save the lens parameters in the ScriptableObject
        SaveLensParameters();
    }

    // Set the position of the center of the ellipse in coordinate and redraw the ellipse accordingly
    public new void SetEllipseCenterPositionParameter(Vector2 newCenterCoord)
    {
        base.SetEllipseCenterPositionParameter(newCenterCoord);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();

        // Save the lens parameters in the ScriptableObject
        SaveLensParameters();
    }

    protected override void OnEllipseAngleChangedHandler(Vector2 angleNewPosition, Vector2 ellipseOldCursorPosition)
    {
        base.OnEllipseAngleChangedHandler(angleNewPosition, ellipseOldCursorPosition);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();

        // Save the lens parameters in the ScriptableObject
        SaveLensParameters();
    }

    protected override void OnEllipseQChangedHandler(Vector2 qNewPosition, Vector2 ellipseOldCursorPosition)
    {
        base.OnEllipseQChangedHandler(qNewPosition, ellipseOldCursorPosition);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();

        // Save the lens parameters in the ScriptableObject
        SaveLensParameters();
    }

    protected override void OnEllipseEinsteinChangedHandler(Vector2 einsteinNewPosition, Vector2 ellipseOldCursorPosition)
    {
        base.OnEllipseEinsteinChangedHandler(einsteinNewPosition, ellipseOldCursorPosition);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();

        // Save the lens parameters in the ScriptableObject
        SaveLensParameters();
    }

    protected override void OnEllipsePositionChangedHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldCursorPosition)
    {
        base.OnEllipsePositionChangedHandler(ellipseNewPosition, ellipseOldCursorPosition);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();

        // Save the lens parameters in the ScriptableObject
        SaveLensParameters();
    }

    protected override void OnEllipsePositionEndDragHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldCursorPosition)
    {
        base.OnEllipsePositionEndDragHandler(ellipseNewPosition, ellipseOldCursorPosition);

        // Update the convergence map and the convergence ellipses (Kappa)
        UpdateConvergenceKappa();

        // Save the lens parameters in the ScriptableObject
        SaveLensParameters();
    }

    // Compute the convergence Kappa of the SIE profile
    public float KappaSIE(float x, float y)
    {
        float einsteinRadius = GetEllipseEinsteinRadiusParameter();
        float q = GetEllipseQParameter();

        return Profiles.KappaSIE(x, y, einsteinRadius, q, GetEllipseAngleParameter());
    }

    // Compute the convergence Kappa of the SIS profile
    public float KappaSIS(float x, float y)
    {
        float einsteinRadius = GetEllipseEinsteinRadiusParameter();

        return Profiles.KappaSIS(x, y, einsteinRadius);
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
    public void SetLensParameters(LensParameters newLensParameters)
    {
        lensParameters = newLensParameters;
    }

    public LensParameters GetLensParameters()
    {
        return lensParameters;
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

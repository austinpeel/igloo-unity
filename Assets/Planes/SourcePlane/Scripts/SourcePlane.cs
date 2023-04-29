using UnityEngine;
using UnityEngine.UI;

public class SourcePlane : PlaneInteractableEllipse
{
    [Header("Brightness Map")]
    [SerializeField] private Color colorBrightnessMap = Color.red;
    [SerializeField] private Image brightnessMap;
    [SerializeField] private bool displayBrightnessMap = true;
    [SerializeField] private Image brightnessColorScale;
    [SerializeField] private GameObject colorScaleOutline;
    [SerializeField] private bool displayBrightnessColorScale = true;

    protected new void Awake() 
    {
        base.Awake();

        // Update the brightness map and the color scale
        UpdateBrightness();
    }

    public new void ResetEllipseParameters()
    {
        base.ResetEllipseParameters();

        // Update the brightness map and the color scale
        UpdateBrightness();
    }

    // Set the q ratio of the ellipse and redraw it accordingly
    public new void SetEllipseQParameter(float newQ)
    {
        base.SetEllipseQParameter(newQ);

        // Update the brightness map and the color scale
        UpdateBrightness();
    }

    // Set the phi angle of the ellipse in degree and redraw it accordingly
    public new void SetEllipsePhiAngleParameter(float newAngle)
    {
        base.SetEllipsePhiAngleParameter(newAngle);

        // Update the brightness map and the color scale
        UpdateBrightness();
    }

    // Set the einstein radius in coordinate and redraw the ellipse accordingly
    public new void SetEllipseEinsteinRadiusParameter(float newEinsteinRadius)
    {
        base.SetEllipseEinsteinRadiusParameter(newEinsteinRadius);

        // Update the brightness map and the color scale
        UpdateBrightness();
    }

    // Set the position of the center of the ellipse in coordinate and redraw the ellipse accordingly
    public new void SetEllipseCenterPositionParameter(Vector2 newCenterCoord)
    {
        base.SetEllipseCenterPositionParameter(newCenterCoord);

        // Update the brightness map and the color scale
        UpdateBrightness();
    }

    protected override void OnEllipseAngleChangedHandler(Vector2 angleNewPosition, Vector2 ellipseOldCursorPosition)
    {
        base.OnEllipseAngleChangedHandler(angleNewPosition, ellipseOldCursorPosition);

        // Update the brightness map and the color scale
        UpdateBrightness();
    }

    protected override void OnEllipseQChangedHandler(Vector2 qNewPosition, Vector2 ellipseOldCursorPosition)
    {
        base.OnEllipseQChangedHandler(qNewPosition, ellipseOldCursorPosition);

        // Update the brightness map and the color scale
        UpdateBrightness();
    }

    protected override void OnEllipseEinsteinChangedHandler(Vector2 einsteinNewPosition, Vector2 ellipseOldCursorPosition)
    {
        base.OnEllipseEinsteinChangedHandler(einsteinNewPosition, ellipseOldCursorPosition);

        // Update the brightness map and the color scale
        UpdateBrightness();
    }

    protected override void OnEllipsePositionChangedHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldCursorPosition)
    {
        base.OnEllipsePositionChangedHandler(ellipseNewPosition, ellipseOldCursorPosition);

        // Update the brightness map and the color scale
        UpdateBrightness();
    }

    protected override void OnEllipsePositionEndDragHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldCursorPosition)
    {
        base.OnEllipsePositionEndDragHandler(ellipseNewPosition, ellipseOldCursorPosition);

        // Update the brightness map and the color scale
        UpdateBrightness();
    }

    // Compute the brightness of the object with the SERSIC profile
    public float BrightnessSERSIC(float x, float y)
    {
        // TODO : Change so that the values are updated with sliders
        float sersicIndex = 1f;
        float amp = 1f;

        float einsteinRadius = GetEllipseEinsteinRadiusParameter();
        float q = GetEllipseQParameter();
        float angle = GetEllipseAngleParameter();

        return Profiles.BrightnessSersic(x, y, amp, sersicIndex, einsteinRadius, q, angle);
    }

    public void UpdateBrightnessMap()
    {
        if (!brightnessMap) return;

        brightnessMap.gameObject.SetActive(displayBrightnessMap);

        // If displayConvergenceMap is set to false then clear the map
        if (!displayBrightnessMap)
        {
            ClearBrightnessMap();
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

                colorsArray[y * widthInt + x] = new Color(colorBrightnessMap.r , colorBrightnessMap.g, colorBrightnessMap.b, BrightnessSERSIC(convertedX,convertedY));
            }
        }

        texture.SetPixels(colorsArray);
        texture.Apply();

        Debug.Log("Called");

        brightnessMap.sprite = Sprite.Create(texture, new Rect(0, 0, widthInt, heightInt), Vector2.one * 0.5f);
    }

    public void ClearBrightnessMap()
    {
        if (!brightnessMap) return;

        float xCoordinateMax = GetXCoordinateMax();
        float yCoordinateMax = GetYCoordinateMax();

        float xRange = xCoordinateMax * 2f;
        float yRange = yCoordinateMax * 2f;

        int widthInt = ((int)width);
        int heightInt = ((int)height);

        Vector2 centerPosition = GetEllipseCenterPositionParameter();

        Texture2D texture = new Texture2D(widthInt, heightInt);

        brightnessMap.sprite = Sprite.Create(texture, new Rect(0, 0, widthInt, heightInt), Vector2.one * 0.5f);
    }

    public void UpdateBrightnessColorScale()
    {
        if (!brightnessColorScale) return;

        // Use displayConvergenceColorScale to update accordingly the outline
        if (colorScaleOutline != null) colorScaleOutline.SetActive(displayBrightnessColorScale);

        foreach (Transform child in colorScaleOutline.transform)
        {
            child.gameObject.SetActive(displayBrightnessColorScale);
        }

        // If displayConvergenceColorScale is set to false then clear the color scale
        if (!displayBrightnessColorScale)
        {
            ClearBrightnessColorScale();
            return;
        }

        int widthInt = ((int)brightnessColorScale.rectTransform.rect.width);
        int heightInt = ((int)brightnessColorScale.rectTransform.rect.height);

        Texture2D texture = new Texture2D(widthInt, heightInt);
        texture.wrapMode = TextureWrapMode.Clamp;

        Color[] colorsArray = new Color[widthInt * heightInt];
        Color colorForX = colorBrightnessMap;

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

        brightnessColorScale.sprite = Sprite.Create(texture, new Rect(0, 0, widthInt, heightInt), Vector2.one * 0.5f);
    }

    public void ClearBrightnessColorScale()
    {
        if (!brightnessColorScale) return;

        int widthInt = ((int)brightnessColorScale.rectTransform.rect.width);
        int heightInt = ((int)brightnessColorScale.rectTransform.rect.height);

        Texture2D texture = new Texture2D(widthInt, heightInt);

        brightnessColorScale.sprite = Sprite.Create(texture, new Rect(0, 0, widthInt, heightInt), Vector2.one * 0.5f);
    }

    public void UpdateBrightness()
    {
        UpdateBrightnessMap();
        UpdateBrightnessColorScale();
    }

    // --------------------- USED IN LENS PLANE EDITOR ---------------------
    public void SetColorBrightnessMap(Color newColorBrightnessMap, bool redraw = false)
    {
        colorBrightnessMap = newColorBrightnessMap;

        if (redraw)
        {
            UpdateBrightnessMap();
        }
    }

    public Color GetColorBrightnessMap()
    {
        return colorBrightnessMap;
    }

    public void SetBrightnessMap(Image newBrightnessMap, bool redraw = false)
    {
        brightnessMap = newBrightnessMap;

        if (redraw)
        {
            UpdateBrightnessMap();
        }
    }

    public Image GetBrightnessMap()
    {
        return brightnessMap;
    }

    public void SetDisplayBrightnessMap(bool newDisplayBrightnessMap, bool redraw = false)
    {
        displayBrightnessMap = newDisplayBrightnessMap;

        if (redraw)
        {
            UpdateBrightnessMap();
        }
    }

    public bool GetDisplayBrightnessMap()
    {
        return displayBrightnessMap;
    }

    public void SetBrightnessColorScale(Image newBrightnessColorScale, bool redraw = false)
    {
        brightnessColorScale = newBrightnessColorScale;

        if (redraw)
        {
            UpdateBrightnessColorScale();
        }
    }

    public Image GetBrightnessColorScale()
    {
        return brightnessColorScale;
    }

    public void SetColorScaleOutline(GameObject newColorScaleOutline, bool redraw = false)
    {
        colorScaleOutline = newColorScaleOutline;

        if (redraw)
        {
            UpdateBrightnessColorScale();
        }
    }

    public GameObject GetColorScaleOutline()
    {
        return colorScaleOutline;
    }

    public void SetDisplayBrightnessColorScale(bool newDisplayBrightnessColorScale, bool redraw = false)
    {
        displayBrightnessColorScale = newDisplayBrightnessColorScale;

        if (redraw)
        {
            UpdateBrightnessColorScale();
        }
    }

    public bool GetDisplayBrightnessColorScale()
    {
        return displayBrightnessColorScale;
    }
}

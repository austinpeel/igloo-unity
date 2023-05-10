using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static DestroyUtils;

public class SourcePlane : PlaneInteractableEllipse
{
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float sersicIndex = 1f;

    [Header("Sliders")]
    [SerializeField] private SliderCurrentValue sliderAmplitude;
    [SerializeField] private SliderCurrentValue sliderSersicIndex;

    [Header("Brightness Map")]
    [SerializeField] private Color colorBrightnessMap = Color.red;
    [SerializeField] private Image brightnessMap;
    [SerializeField] private bool displayBrightnessMap = true;
    [SerializeField] private Image brightnessColorScale;
    [SerializeField] private GameObject colorScaleOutline;
    [SerializeField] private bool displayBrightnessColorScale = true;
    [SerializeField] private GameObject ellipsesBrightnessParent;
    [SerializeField] private GameObject ellipsePrefab;
    [SerializeField] private bool displayEllipsesBrightnessMap = true;

    private float[] ellipsesBrightnessEinsteinArray = new float[10];

    protected new void Awake() 
    {
        base.Awake();

        // Update the brightness map and the color scale
        UpdateBrightness();
    }

    // Wrapper so that the slider can call it
    public void SetAmplitudeSlider(float newAmplitude)
    {
        SetAmplitude(newAmplitude, true);

        if (sliderAmplitude) sliderAmplitude.UpdateSliderValue(amplitude);
    }

    public void SetAmplitude(float newAmplitude, bool redraw = false)
    {
        amplitude = newAmplitude;

        if (redraw)
        {
            UpdateBrightness();
        }
    }

    public float GetAmplitude()
    {
        return amplitude;
    }

    // Wrapper so that the slider can call it
    public void SetSersicIndexSlider(float newSersicIndex)
    {
        SetSersicIndex(newSersicIndex, true);

        if (sliderSersicIndex) sliderSersicIndex.UpdateSliderValue(sersicIndex);
    }

    public void SetSersicIndex(float newSersicIndex, bool redraw = false)
    {
        sersicIndex = newSersicIndex;

        if (redraw)
        {
            UpdateBrightness();
        }
    }

    public float GetSersicIndex()
    {
        return sersicIndex;
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
    public float BrightnessSERSIC(float x, float y, bool log10 = false)
    {
        float einsteinRadius = GetEllipseEinsteinRadiusParameter();
        float q = GetEllipseQParameter();
        float angle = GetEllipseAngleParameter();

        return Profiles.BrightnessSersic(x, y, amplitude, sersicIndex, einsteinRadius, q, angle, log10);
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

                colorsArray[y * widthInt + x] = new Color(colorBrightnessMap.r , colorBrightnessMap.g, colorBrightnessMap.b, BrightnessSERSIC(convertedX,convertedY, true));
            }
        }

        texture.SetPixels(colorsArray);
        texture.Apply();

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

    private float ComputeEinsteinRadiusFromBrightness(float brightness)
    {
        if (!interactEllipseUI) return 0f;

        // With the major axis of the ellipsoid along the x axis
        float einsteinRadius = GetEllipseEinsteinRadiusParameter();
        float q = GetEllipseQParameter();

        float partOne = Mathf.Pow(1f - Mathf.Log(brightness / amplitude) / Profiles.BnSersic(sersicIndex), 2 * sersicIndex);
        float partTwo = q * einsteinRadius * einsteinRadius;

        float minorAxis = Mathf.Sqrt(partOne * partTwo);

        if (minorAxis < 0f || minorAxis == float.NaN) return 0f;

        return interactEllipseUI.ComputeEinsteinRadius(minorAxis, interactEllipseUI.ComputeMajorAxis(minorAxis, q));  
    }

    private void FillEinsteinEllipsesBrightnessArray(float min)
    {
        // The min is included

        for (int i = 0; i < ellipsesBrightnessEinsteinArray.Length; i++)
        {
            ellipsesBrightnessEinsteinArray[i] = ComputeEinsteinRadiusFromBrightness(min + 0.5f * (i));
        }
    }

    // Draw Ellipses where log10 of Brightness equals min and increase by 0.5 at each iteration
    public void DrawEllipsesBrightness()
    {
        float einsteinRadius = GetEllipseEinsteinRadiusParameter();
        float q = GetEllipseQParameter();
        float angle = GetEllipseAngleParameter();
        Vector2 centerPosition = GetEllipseCenterPositionParameter();

        FillEinsteinEllipsesBrightnessArray(0f);

        for (int i = 0; i < ellipsesBrightnessEinsteinArray.Length; i++)
        {
            EllipseUI ellipseBrightness = Instantiate(ellipsePrefab, ellipsesBrightnessParent.transform).GetComponent<EllipseUI>();
            ellipseBrightness.SetQ(q);
            ellipseBrightness.SetEinsteinRadius(ellipsesBrightnessEinsteinArray[i], true);
            ellipseBrightness.SetAngle(angle, true);
            ellipseBrightness.SetCenterPosition(centerPosition, true);
        }
    }

    public void ClearEllipsesBrightness()
    {
        if (!ellipsesBrightnessParent) return;

        if (ellipsesBrightnessParent.transform.childCount > 0)
        {
            if (Application.isPlaying)
            {
                for (int i = ellipsesBrightnessParent.transform.childCount; i > 0; i--)
                {
                    SafeDestroyGameObject(ellipsesBrightnessParent.transform.GetChild(0));
                }
            }
            else 
            {
                // In Edit mode there is an error if we destroy the ellipses before the next frame
                for (int i = 0; i < ellipsesBrightnessParent.transform.childCount; i++)
                {
                    SafeDestroyGameObjectNextFrame(ellipsesBrightnessParent.transform.GetChild(i));
                }
            }
        }
    }

    public void UpdateEllipsesBrightness()
    {
        if (!ellipsesBrightnessParent || !ellipsePrefab) return;

        ClearEllipsesBrightness();

        if (!displayEllipsesBrightnessMap) return;

        DrawEllipsesBrightness();
    }

    public void UpdateBrightness()
    {
        UpdateBrightnessMap();
        UpdateBrightnessColorScale();
        UpdateEllipsesBrightness();
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

    public void SetSliderAmplitude(SliderCurrentValue newSliderAmplitude, bool redraw = false)
    {
        sliderAmplitude = newSliderAmplitude;

        if (sliderAmplitude && redraw) 
        {
            sliderAmplitude.UpdateSliderValue(amplitude);
        }
    }

    public SliderCurrentValue GetSliderAmplitude()
    {
        return sliderAmplitude;
    }

    public void SetSliderSersicIndex(SliderCurrentValue newSliderSersicIndex, bool redraw = false)
    {
        sliderSersicIndex = newSliderSersicIndex;
        
        if (sliderSersicIndex && redraw) 
        {
            sliderSersicIndex.UpdateSliderValue(sersicIndex);
        }
    }

    public SliderCurrentValue GetSliderSersicIndex()
    {
        return sliderSersicIndex;
    }

    public void SetEllipsesBrightnessParent(GameObject newEllipsesBrightnessParent, bool redraw = false)
    {
        ellipsesBrightnessParent = newEllipsesBrightnessParent;

        if (redraw)
        {
            UpdateEllipsesBrightness();
        }
    }

    public GameObject GetEllipseBrightnessParent()
    {
        return ellipsesBrightnessParent;
    }

    public void SetEllipsePrefab(GameObject newEllipsePrefab, bool redraw = false)
    {
        ellipsePrefab = newEllipsePrefab;

        if (redraw)
        {
            UpdateEllipsesBrightness();
        }
    }

    public GameObject GetEllipsePrefab()
    {
        return ellipsePrefab;
    }

    public void SetDisplayEllipsesBrightnessMap(bool newDisplayEllipsesBrightnessMap, bool redraw = false)
    {
        displayEllipsesBrightnessMap = newDisplayEllipsesBrightnessMap;

        if (redraw)
        {
            UpdateEllipsesBrightness();
        }
    }

    public bool GetDisplayEllipsesBrightnessMap()
    {
        return displayEllipsesBrightnessMap;
    }
}

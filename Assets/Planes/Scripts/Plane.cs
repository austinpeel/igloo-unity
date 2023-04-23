using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class Plane : MonoBehaviour, ICoordinateConverter
{
    private const string SNAP_MODE_TEXT = "Snap mode";
    private const string FREE_MODE_TEXT = "Free mode";
    [SerializeField] private float xCoordinateMax = 4f;
    [SerializeField] private float yCoordinateMax = 4f;
    [SerializeField] private GridUI gridUI;
    [SerializeField] private AxisUI yAxis;
    [SerializeField] private AxisUI xAxis;
    [SerializeField] private TextMeshProUGUI currentModeText;
    [SerializeField] private float boundaryX;
    [SerializeField] private float boundaryY;

    protected RectTransform rectTransform;
    protected float width = 0f;
    protected float height = 0f;
    private bool isInSnapMode = true;
    private bool hasModeChanged = false;

    protected void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        if (yAxis) yAxis.SetAxisLength(height, true);
        if (xAxis) xAxis.SetAxisLength(width, true);

        UpdateCurrentModeText(isInSnapMode);
    }

    protected void Update() 
    {
        // Check if the Left Shift Key is hold down and change mode accordingly (when Left Shift key is hold down the ellipse is in Rotation mode)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            hasModeChanged = true;
            isInSnapMode = false;
            UpdateCurrentModeText(isInSnapMode);
            return;
        } 
        
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            hasModeChanged = true;
            isInSnapMode = true;
            UpdateCurrentModeText(isInSnapMode);
            return;
        }

        hasModeChanged = false;
    }

    private void OnValidate() 
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
    }

    public void UpdateCurrentModeText(bool isInSnapMode)
    {
        if (!currentModeText) return;

        if (isInSnapMode)
        {
            currentModeText.text = SNAP_MODE_TEXT;
            return;
        }

        currentModeText.text = FREE_MODE_TEXT;
    }

    public Vector2 ConvertScreenPositionInPlaneRect(Vector2 screenPosition)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, 
            GetComponentInParent<Canvas>().worldCamera, out localPosition);

        return localPosition;
    }
    public Vector2 ConvertCoordinateToRectPosition(Vector2 coordinate)
    {
        return ConversionUtils.ConvertCoordinateToRectPosition(rectTransform, coordinate, xCoordinateMax, yCoordinateMax);
    }

    public Vector2 ConvertRectPositionToCoordinate(Vector2 rectPosition)
    {
        return ConversionUtils.ConvertRectPositionToCoordinate(rectTransform, rectPosition, xCoordinateMax, yCoordinateMax);
    }

    public bool GetHasModeChanged()
    {
        return hasModeChanged;
    }

    public bool GetIsInSnapMode()
    {
        return isInSnapMode;
    }

    // --------------------- USED IN PLANE EDITOR ---------------------

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
            UpdateCurrentModeText(isInSnapMode);
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
}

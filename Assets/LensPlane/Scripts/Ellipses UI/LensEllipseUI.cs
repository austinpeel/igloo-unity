using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LensEllipseUI : EllipseUI
{
    [SerializeField] private float distanceMagnetCenter = 25f;
    [SerializeField] private float distanceMagnetQ = 0.05f;
    [SerializeField] private float distanceMagnetAngle = 10f;
    [SerializeField] private QPointUI qPointParameter;
    [SerializeField] private ParameterImageValueDisplay qPointParameterDisplay;
    [SerializeField] private CenterPointUI centerPointParameter;
    [SerializeField] private ParameterImageValueDisplay centerPointParameterDisplay;
    [SerializeField] private EinsteinPointUI einsteinPointParameter;
    [SerializeField] private ParameterImageValueDisplay einsteinPointParameterDisplay;
    [SerializeField] private AnglePointUI anglePointParameter;
    [SerializeField] private ParameterImageValueDisplay anglePointParameterDisplay;
    [SerializeField] private LineUI semiMajorAxisLine;
    [SerializeField] private LineUI anglePointParameterLine;
    [SerializeField] private float anglePointParameterLineLength = 2f;
    [SerializeField] private LineUI axisYRotation;
    [SerializeField] private CircularArcUI arcAngleRotation;

    // Custom Events for Position
    // Define a custom event delegate with the position of the center of the ellipse
    public delegate void EllipsePositionChangedEventHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition);
    // Define the custom event
    public event EllipsePositionChangedEventHandler OnEllipsePositionChanged;
    // Define a custom event delegate with the new position of the center of the ellipse and the position at the beginning of the drag
    public delegate void EllipsePositionEndDragEventHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition);
    public event EllipsePositionEndDragEventHandler OnEllipsePositionEndDrag;

    private void TriggerPositionEndDrag(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition)
    {
        OnEllipsePositionEndDrag?.Invoke(ellipseNewPosition, ellipseOldPosition);
    }

    private void TriggerPositionChanged(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition)
    {
        OnEllipsePositionChanged?.Invoke(ellipseNewPosition, ellipseOldPosition);
    }

    // Custom Events for Einstein Radius
    // Define a custom event delegate with the position of the cursor in RectTransform and the oldPosition in Screen Position
    public delegate void EllipseEinsteinChangedEventHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition);
    public event EllipseEinsteinChangedEventHandler OnEllipseEinsteinChanged;

    private void TriggerEinsteinChanged(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition)
    {
        OnEllipseEinsteinChanged?.Invoke(ellipseNewPosition, ellipseOldPosition);
    }

    // Custom Events for Q
    // Define a custom event delegate with the position of the cursor in RectTransform and the oldPosition in Screen Position
    public delegate void EllipseQChangedEventHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition);
    public event EllipseQChangedEventHandler OnEllipseQChanged;

    private void TriggerQChanged(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition)
    {
        OnEllipseQChanged?.Invoke(ellipseNewPosition, ellipseOldPosition);
    }

    // Custom Events for Angle Phi
    // Define a custom event delegate with the position of the cursor in RectTransform and the oldPosition in Screen Position
    public delegate void EllipseAngleChangedEventHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition);
    public event EllipseAngleChangedEventHandler OnEllipseAngleChanged;

    private void TriggerAngleChanged(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition)
    {
        OnEllipseAngleChanged?.Invoke(ellipseNewPosition, ellipseOldPosition);
    }

    private Vector2 beginDragPosition = Vector2.zero;
    private bool isInSnapMode = true;
    private bool isInRotationMode = false;
    private List<ParameterImageValueDisplay> parameterImageValueList = new List<ParameterImageValueDisplay>();

    private new void Awake() 
    {
        base.Awake();
        InitializeParameterImageValueList();
        //base.InitializeCoordinateConverter();
    }

    private void InitializeParameterImageValueList()
    {
        if (qPointParameterDisplay)
        {
            parameterImageValueList.Add(qPointParameterDisplay);
        }
        if (centerPointParameterDisplay)
        {
            parameterImageValueList.Add(centerPointParameterDisplay);
        }
        if (einsteinPointParameterDisplay)
        {
            parameterImageValueList.Add(einsteinPointParameterDisplay);
        }
        if (anglePointParameterDisplay)
        {
            parameterImageValueList.Add(anglePointParameterDisplay);
        }
    }

    private new void Start()
    {
        base.Start();

        RedrawLensEllipse();

        // Subscribe to the events of the different points
        qPointParameter.OnParameterChanged += OnParameterChangedHandler;
        centerPointParameter.OnParameterChanged += OnParameterChangedHandler;
        einsteinPointParameter.OnParameterChanged += OnParameterChangedHandler;
        anglePointParameter.OnParameterChanged += OnParameterChangedHandler;

        // Events for EndDrag (Magnet parameters)
        centerPointParameter.OnParameterEndDrag += OnParameterEndDragHandler;
        qPointParameter.OnParameterEndDrag += OnParameterEndDragHandler;
        anglePointParameter.OnParameterEndDrag += OnParameterEndDragHandler;

        // Events for BeginDrag
        centerPointParameter.OnParameterBeginDrag += OnParameterBeginDragHandler;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        // Unsubscribe from the OnParameterChanged event to prevent memory leaks
        qPointParameter.OnParameterChanged -= OnParameterChangedHandler;
        centerPointParameter.OnParameterChanged -= OnParameterChangedHandler;
        einsteinPointParameter.OnParameterChanged -= OnParameterChangedHandler;
        anglePointParameter.OnParameterChanged -= OnParameterChangedHandler;

        // Events for EndDrag (Magnet parameters)
        centerPointParameter.OnParameterEndDrag -= OnParameterEndDragHandler;
        qPointParameter.OnParameterEndDrag -= OnParameterEndDragHandler;
        anglePointParameter.OnParameterEndDrag -= OnParameterEndDragHandler;

        // Events for BeginDrag
        centerPointParameter.OnParameterBeginDrag -= OnParameterBeginDragHandler;
    }

    private void RedrawLensEllipse()
    {
        float q = GetQParameter();
        float einsteinRadius = GetEinsteinRadiusParameter();
        float angle = GetAngleParameter();
        Vector2 currentCenterPosition = GetCenterPositionParameter();

        // Display Q value and Einstein radius value
        SetQ(q);
        SetEinsteinRadius(einsteinRadius, true);
        SetAngle(angle, true);
        SetCenterPosition(currentCenterPosition, true);
        UpdatePointsParametersPositions();
        DisplayRotationLines(isInRotationMode);  
    }

    public void DisplayRotationLines(bool isInRotationMode)
    {
        // If the ellipse is in Rotation Mode, then display the rotation axis and the arc circle
        if (isInRotationMode)
        {
            axisYRotation.gameObject.SetActive(true);
            arcAngleRotation.gameObject.SetActive(true);
            return;
        }

        axisYRotation.gameObject.SetActive(false);
        arcAngleRotation.gameObject.SetActive(false);
    }

    public void UpdatePointsParametersPositions()
    {
        // The center for the CenterPoint will always be at (0,0)
        if (centerPointParameter)
        {
            centerPointParameter.SetPosition(Vector2.zero);
        }

        if (qPointParameter)
        {
            qPointParameter.SetPosition(GetPositionRectQPoint());
        }

        if (qPointParameterDisplay)
        {
            qPointParameterDisplay.SetPosition(GetPositionRectQPoint());
        }
        
        if (einsteinPointParameter)
        {
            einsteinPointParameter.SetPosition(GetPositionRectEinsteinPoint());
        }
        
        if (einsteinPointParameterDisplay)
        {
            einsteinPointParameterDisplay.SetPosition(GetPositionRectEinsteinPoint());
        }

        UpdateAnglePointAndLine();
        UpdateRotationLines();
    }

    private void UpdateAnglePointAndLine()
    {
        Vector2 startPosition = GetPositionRectQPoint();
        Vector2 endPosition = startPosition + Vector2.up * anglePointParameterLineLength;

        if (anglePointParameterLine)
        {
            anglePointParameterLine.SetPositions(startPosition, endPosition, true);
        }

        if (anglePointParameter)
        {
            anglePointParameter.SetPosition(endPosition);
        }

        if (anglePointParameterDisplay)
        {
            anglePointParameterDisplay.SetPosition(endPosition);
        }
    }

    private void UpdateRotationLines()
    {
        // Update the semi-major axis line if there is one
        if (semiMajorAxisLine)
        {
            semiMajorAxisLine.SetPositions(Vector2.zero, GetPositionRectQPoint(), true);
        }

        // Update the Y axis line if there is one
        if (axisYRotation)
        {
            Vector2 currentCenterPosition = GetCenterPositionInRect();
            axisYRotation.SetPositions(currentCenterPosition, currentCenterPosition + GetPositionRectQPoint(), true);
        }
    }

    // COOLEST convention tells that the angle is counter-clockwise from the positive y axis
    private void UpdateAngleDisplay()
    {
        float angle = GetAngleParameter();

        // Inverse the rotation of the ellipse so that the image and the value remains horizontal 
        parameterImageValueList.ForEach(imageValue => imageValue.SetRotationToZero());

        // Update the different labels and their value position
        if (anglePointParameterDisplay)
        {
            anglePointParameterDisplay.SetValueText(PhiAngleToString(angle));
            anglePointParameterDisplay.UpdateOffsetQParameter(angle);
        }

        if (qPointParameterDisplay)
        {
            qPointParameterDisplay.UpdateOffsetQParameter(angle);
        }

        if (einsteinPointParameterDisplay)
        {
            einsteinPointParameterDisplay.UpdateOffsetEinsteinParameter(angle);
        }

        if (centerPointParameterDisplay)
        {
            centerPointParameterDisplay.UpdateOffsetCenterParameter(angle);
        }

        // Update the semi-major axis line if there is one
        if (semiMajorAxisLine)
        {
            semiMajorAxisLine.SetRotationAngle(angle, true);
        }

        // Update the line of the point angle if there is one
        if (anglePointParameterLine)
        {
            anglePointParameterLine.SetRotationAngle(angle, true);
        }

        // Update the circular arc if there is one
        if (arcAngleRotation)
        {
            arcAngleRotation.SetAngle(angle, true);
        }
    }

    // Move the ellipse to the newPosition (in screen position)
    public void MoveScreenPosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    // Update the position's parameters with newCenterPosition in coordinates
    public new void SetCenterPosition(Vector2 newCenterPosition, bool redraw = false)
    {
        base.SetCenterPosition(newCenterPosition, redraw);

        if (centerPointParameter)
        {
            // Set the position of the point at the center of the ellipse
            centerPointParameter.SetPosition(Vector2.zero);
        }

        if (centerPointParameterDisplay)
        {
            centerPointParameterDisplay.SetValueText(PositionCenterToString(GetCenterPositionParameter()));
        }

        Vector2 centerPositionInRect = GetCenterPositionInRect();

        // Update the Y axis line if there is one
        if (axisYRotation)
        {
            axisYRotation.SetPositions(centerPositionInRect, centerPositionInRect + GetPositionRectQPoint(), true);
        }

        // Update the circular arc if there is one
        if (arcAngleRotation)
        {
            arcAngleRotation.SetPosition(centerPositionInRect);
        }
    }

    public new void SetAngle(float newAngle, bool redraw = false)
    {
        base.SetAngle(newAngle, redraw);

        if (redraw)
        {
            UpdateAngleDisplay();
        }
    }

    public void SetQWithYAxis(float axisValue)
    {
        float einsteinInRect = GetEinsteinInRect();

        // The Y axis should always be the semi major axis
        if (axisValue < einsteinInRect)
        {
            axisValue = einsteinInRect;
        }

        float widthX = GetWidthX();
        float widthY = GetWidthY();

        // Compute the delta between the oldValue and the newValue to update accordingly widthX
        float delta = axisValue - widthY;

        // The X axis should never be negative
        if (widthX - delta < 0)
        {
            return;
        }

        // Limit min value of Q to 0.1
        float limitQ = 0.1f;
        if ((widthX - delta) / axisValue < limitQ)
        {
            // Compute the difference of the axis that results with a q of value limitQ
            float diff = einsteinInRect * (1f - limitQ) / (1f + limitQ);

            SetWidthY(einsteinInRect + diff);
            SetWidthX(einsteinInRect - diff);

            SetQ(ComputeRatioQ(), true);
            return;
        }

        // The major axis here is always widthY
        SetWidthY(axisValue);
        SetWidthX(widthX - delta);

        SetQ(ComputeRatioQ(), true);
    }

    // Set the Einstein radius (in coordinate) and update the value displayed
    public new void SetEinsteinRadius(float newEinsteinRadius, bool redraw = false)
    {
        base.SetEinsteinRadius(newEinsteinRadius, redraw);
        
        float einsteinRadius = base.GetEinsteinRadiusParameter();

        // Update the value displayed
        if (einsteinPointParameterDisplay)
        {
            einsteinPointParameterDisplay.SetValueText(EinsteinRadiusToString(einsteinRadius));
        }

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public new void SetQ(float newQ, bool redraw = false)
    {
        base.SetQ(newQ, redraw);

        float q = base.GetQParameter();

        if (qPointParameterDisplay)
        {
            qPointParameterDisplay.SetValueText(QValueToString(q));
        }

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    // Return the q ratio that corresponds to (b/a)
    // With b as the semi-minor axis and a as the semi-major axis
    public float ComputeRatioQ()
    {
        float a, b;
        float widthX = GetWidthX();
        float widthY = GetWidthY();

        if (widthX > widthY)
        {
            a = widthX;
            b = widthY;
        } else
        {
            a = widthY;
            b = widthX;
        }

        return b/a;
    }

    // Compute the position (position in rectTransform) of the point that influences the Q value
    public Vector2 GetPositionRectQPoint()
    {
        // The Q point will always be on the ellipse at (centerPos.x, centerPos.y + widthY)
        // We remove the half of the thickness so that the point is centered
        float widthY = GetWidthY();
        float thickness = GetThickness();
        return Vector2.up * (widthY - thickness/2);
    }

    // Compute the position (position in rectTransform) of the point that influences the Q value
    public Vector2 GetPositionRectEinsteinPoint()
    {
        // The Q point will always be on the ellipse at (centerPos.x, centerPos.y + widthY)
        // We remove the half of the thickness so that the point is centered
        float widthX = GetWidthX();
        float thickness = GetThickness();
        return Vector2.right * (widthX - thickness/2);
    }

    public Vector2 ConvertScreenPositionInEllipseRect(Vector2 screenPosition)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, 
            GetComponentInParent<Canvas>().worldCamera, out localPosition);

        return localPosition;
    }

    public void SetDistanceMagnetCenter(float newDistanceMagnetCenter)
    {
        distanceMagnetCenter = newDistanceMagnetCenter;
    }

    public float GetDistanceMagnetCenter()
    {
        return distanceMagnetCenter;
    }

    // "Magnet Effect" for the position :
    // If the position at the end of the drag is less than distanceMagnetCenter to the center position
    // Then set the position to (0,0) (which is the center)
    public void MagnetCenterPoint()
    {

        if (GetCenterPositionInRect().magnitude <= distanceMagnetCenter)
        {
            // Set the center position at (0,0)
            SetCenterPosition(Vector2.zero, true);
        }
    }

    public void SetDistanceMagnetQ(float newDistanceMagnetQ)
    {
        distanceMagnetQ = newDistanceMagnetQ;
    }

    public float GetDistanceMagnetQ()
    {
        return distanceMagnetQ;
    }

    // "Magnet Effect" for the q ratio :
    // If the q obtained at the end of the drag is in the range  [1 - distanceMagnetQ, 1], then set it to 1
    public void MagnetQPoint()
    {
        float q = base.GetQParameter();

        if ((1f - q) <= distanceMagnetQ)
        {
            SetQ(1f, true);
            UpdatePointsParametersPositions();
        }
    }

    public void SetDistanceMagnetAngle(float newDistanceMagnetAngle)
    {
        distanceMagnetAngle = newDistanceMagnetAngle;
    }

    public float GetDistanceMagnetAngle()
    {
        return distanceMagnetAngle;
    }

    // "Magnet Effect" for the angle :
    // If the angle obtained at the end of the drag is in the range  [x - distanceMagnetAngle, x + distanceMagnetAngle],
    // for x in {0;90;180;270;360} then set it to the corresponding angle
    public void MagnetAnglePoint()
    {
        float angle = GetAngleParameter();

        float lowestDifference = angle;
        int lowestDifferenceAngle = 0;
        float difference;
        for (int i = 1; i < 5; i++)
        {
            difference = Mathf.Abs(angle - i * 90f);

            if (difference < lowestDifference)
            {
                lowestDifference = difference;
                lowestDifferenceAngle = i;
            }
        }

        if ((lowestDifference) <= distanceMagnetAngle)
        {
            SetAngle(lowestDifferenceAngle * 90f, true);
            UpdateAngleDisplay();
        }
    }

    public float ComputeEinsteinRadius(float minorAxis, float majorAxis)
    {
        return minorAxis + (majorAxis - minorAxis) / 2;
    }

    public float ComputeMajorAxis(float minorAxis, float q)
    {
        return minorAxis / q;
    }

    public float ComputeMinorAxis(float majorAxis, float q)
    {
        return majorAxis * q;
    }

    private void OnParameterChangedHandler(object sender, Vector2 cursorPosition)
    {
        PointParameterUI parameterUI = sender as PointParameterUI;

        if (parameterUI != null)
        {
            if (parameterUI is QPointUI)
            {
                float convertedY = ConvertScreenPositionInEllipseRect(cursorPosition).y;
                TriggerQChanged(Vector2.up * convertedY, beginDragPosition);
            } 
            else if (parameterUI is CenterPointUI)
            {
                MoveScreenPosition(cursorPosition);

                Vector2 convertedPosition = rectTransform.anchoredPosition;
                TriggerPositionChanged(convertedPosition, beginDragPosition);
            }
            else if (parameterUI is EinsteinPointUI)
            {
                float convertedX = ConvertScreenPositionInEllipseRect(cursorPosition).x;
                TriggerEinsteinChanged(Vector2.right * convertedX, beginDragPosition);
            }
            else if (parameterUI is AnglePointUI)
            {
                Vector2 convertedPosition = ConvertScreenPositionInEllipseRect(cursorPosition);
                TriggerAngleChanged(convertedPosition, beginDragPosition);
            }
        }
    }

    private void OnParameterEndDragHandler(object sender)
    {
        PointParameterUI parameterUI = sender as PointParameterUI;

        if (parameterUI != null)
        {
            if (parameterUI is CenterPointUI)
            {
                TriggerPositionEndDrag(GetCenterPositionInRect(), beginDragPosition);
            }
            else if (parameterUI is AnglePointUI)
            {
                isInRotationMode = false;
                DisplayRotationLines(isInRotationMode);
            }
        }
    }

    private void OnParameterBeginDragHandler(object sender, Vector2 beginDragCursorPosition)
    {
        PointParameterUI parameterUI = sender as PointParameterUI;

        if (parameterUI != null)
        {
            if (parameterUI is CenterPointUI)
            {
                // Store the position at the beginning of the drag
                beginDragPosition = beginDragCursorPosition;
            }
        }
    }

    private string EinsteinRadiusToString(float einsteinRadius)
    {
        return einsteinRadius.ToString("0.00")+"\"";
    }

    private string QValueToString(float q)
    {
        return q.ToString("0.00");
    }

    private string PhiAngleToString(float angle)
    {
        return angle.ToString("0.00");
    }

    private string PositionCenterToString(Vector2 position)
    {
        string posX = position.x.ToString("0.00");
        string posY = position.y.ToString("0.00");

        return "("+posX+"\""+","+posY+"\""+")";
    }

    public void SetIsInRotationMode(bool newIsInRotationMode)
    {
        isInRotationMode = newIsInRotationMode;
    }

    public bool GetIsInRotationMode()
    {
        return isInRotationMode;
    }

    public void SetIsInSnapMode(bool newIsInSnapMode)
    {
        isInSnapMode = newIsInSnapMode;
    }

    public bool GetIsInSnapMode()
    {
        return isInSnapMode;
    }

    public float GetAnglePointParameterLineLength()
    {
        return anglePointParameterLineLength;
    }

    // --------------------- USED IN LENS ELLIPSE EDITOR ---------------------
    public void SetQPointParameter(QPointUI newQPointParameter, bool redraw = false)
    {
        qPointParameter = newQPointParameter;

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public QPointUI GetQPointParameter()
    {
        return qPointParameter;
    }

    public void SetQPointParameterDisplay(ParameterImageValueDisplay newQPointParameterDisplay, bool redraw = false)
    {
        qPointParameterDisplay = newQPointParameterDisplay;

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public ParameterImageValueDisplay GetQPointParameterDisplay()
    {
        return qPointParameterDisplay;
    }

    public void SetCenterPointParameter(CenterPointUI newCenterPointParameter, bool redraw = false)
    {
        centerPointParameter = newCenterPointParameter;

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public CenterPointUI GetCenterPointParameter()
    {
        return centerPointParameter;
    }

    public void SetCenterPointParameterDisplay(ParameterImageValueDisplay newCenterPointParameterDisplay, bool redraw = false)
    {
        centerPointParameterDisplay = newCenterPointParameterDisplay;

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public ParameterImageValueDisplay GetCenterPointParameterDisplay()
    {
        return centerPointParameterDisplay;
    }

    public void SetEinsteinPointParameter(EinsteinPointUI newEinsteinPointParameter, bool redraw = false)
    {
        einsteinPointParameter = newEinsteinPointParameter;

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public EinsteinPointUI GetEinsteinPointParameter()
    {
        return einsteinPointParameter;
    }

    public void SetEinsteinPointParameterDisplay(ParameterImageValueDisplay newEinsteinPointParameterDisplay, bool redraw = false)
    {
        einsteinPointParameterDisplay = newEinsteinPointParameterDisplay;

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public ParameterImageValueDisplay GetEinsteinPointParameterDisplay()
    {
        return einsteinPointParameterDisplay;
    }

    public void SetAnglePointParameter(AnglePointUI newAnglePointParameter, bool redraw = false)
    {
        anglePointParameter = newAnglePointParameter;

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public AnglePointUI GetAnglePointParameter()
    {
        return anglePointParameter;
    }

    public void SetAnglePointParameterDisplay(ParameterImageValueDisplay newAnglePointParameterDisplay, bool redraw = false)
    {
        anglePointParameterDisplay = newAnglePointParameterDisplay;

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public ParameterImageValueDisplay GetAnglePointParameterDisplay()
    {
        return anglePointParameterDisplay;
    }

    public void SetSemiMajorAxisLine(LineUI newSemiMajorAxisLine, bool redraw = false)
    {
        semiMajorAxisLine = newSemiMajorAxisLine;

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public LineUI GetSemiMajorAxisLine()
    {
        return semiMajorAxisLine;
    }

    public void SetAnglePointParameterLine(LineUI newAnglePointParameterLine, bool redraw = false)
    {
        anglePointParameterLine = newAnglePointParameterLine;

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public LineUI GetAnglePointParameterLine()
    {
        return anglePointParameterLine;
    }

    public void SetAnglePointParameterLineLength(float newAnglePointParameterLineLength, bool redraw = false)
    {
        anglePointParameterLineLength = newAnglePointParameterLineLength;

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public void SetAxisYRotation(LineUI newAxisYRotation, bool redraw = false)
    {
        axisYRotation = newAxisYRotation;

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public LineUI GetAxisYRotation()
    {
        return axisYRotation;
    }

    public void SetArcAngleRotation(CircularArcUI newArcAngleRotation, bool redraw = false)
    {
        arcAngleRotation = newArcAngleRotation;

        if (redraw)
        {
            UpdatePointsParametersPositions();
        }
    }

    public CircularArcUI GetArcAngleRotation()
    {
        return arcAngleRotation;
    }
}

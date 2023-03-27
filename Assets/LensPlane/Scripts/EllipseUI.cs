using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EllipseUI : Graphic
{
    // One division for each degree seems to be enough
    [SerializeField] private int division = 360;
    [SerializeField] [Range(0, 1)] private float q = 0.5f;
    [SerializeField] private float thickness = 10f;
    [SerializeField] [Range(0, 360)] private float angle = 0f;
    [SerializeField] private float einsteinRadius = 100f;
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

    // Define a custom event delegate with the position of the center of the ellipse
    public delegate void EllipsePositionChangedEventHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition);
    // Define the custom event
    public event EllipsePositionChangedEventHandler OnEllipsePositionChanged;
    // Define a custom event delegate with the new position of the center of the ellipse and the position at the beginning of the drag
    public delegate void EllipsePositionEndDragEventHandler(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition);
    // Define the custom event
    public event EllipsePositionEndDragEventHandler OnEllipsePositionEndDrag;

    private void TriggerPositionEndDrag(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition)
    {
        OnEllipsePositionEndDrag?.Invoke(ellipseNewPosition, ellipseOldPosition);
    }

    private void TriggerPositionChanged(Vector2 ellipseNewPosition, Vector2 ellipseOldPosition)
    {
        OnEllipsePositionChanged?.Invoke(ellipseNewPosition, ellipseOldPosition);
    }

    private Vector2 beginDragPosition = Vector2.zero;

    private float widthX = 100f;
    private float widthY = 200f;
    private Vector2 currentCenterPosition = Vector2.zero;
    private bool isInSnapMode = false;
    private List<ParameterImageValueDisplay> parameterImageValueList = new List<ParameterImageValueDisplay>();

    private new void Awake() 
    {
        base.Awake();
        InitializeParameterImageValueList();
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

    private void Update() 
    {
        // Check if the Left Shift Key is hold down and change mode accordingly (when Left Shift key is hold down the ellipse is in Rotation mode)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isInSnapMode = true;
        } 
        
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isInSnapMode = false;
        }
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

    protected override void OnValidate()
    {
        UpdateAngleDisplay();
        UpdateRectTransformSize();

        // Display Q value and Einstein radius value
        SetQ(q);
        SetEinsteinRadius(einsteinRadius);

        SetCenterPosition(Vector2.zero);
        DrawEllipseGivenEinsteinRadiusAndQ(einsteinRadius, q, false, false);
        UpdatePointsParametersPositions();
    }

    // Create the meshs with respect to the chosen parameters of the class
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        vh.Clear();
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = base.color;

        float deltaAngle = Mathf.PI * 2 / division;
        int nbrVertex = 2 * division;

        for (int i = 0; i < division; i++)
        {
            float angle = deltaAngle * i;

            vertex.position = new Vector2((widthX - thickness) * Mathf.Cos(angle), (widthY - thickness) * Mathf.Sin(angle));
            vh.AddVert(vertex);

            vertex.position = new Vector2(widthX * Mathf.Cos(angle), widthY * Mathf.Sin(angle));
            vh.AddVert(vertex);
            
            int offset = i * 2;
            vh.AddTriangle(offset, (offset + 1) % nbrVertex, (offset + 3) % nbrVertex);
            vh.AddTriangle((offset + 3) % nbrVertex, (offset + 2) % nbrVertex, offset);
        }
    }

    private void UpdatePointsParametersPositions()
    {
        // The center for the CenterPoint will always be at (0,0)
        centerPointParameter.SetPosition(Vector2.zero);
        centerPointParameterDisplay.SetPosition(Vector2.zero);
        qPointParameter.SetPosition(GetPositionRectQPoint());
        qPointParameterDisplay.SetPosition(GetPositionRectQPoint());
        einsteinPointParameter.SetPosition(GetPositionRectEinsteinPoint());
        einsteinPointParameterDisplay.SetPosition(GetPositionRectEinsteinPoint());
        UpdateAnglePointAndLine();
    }

    private void UpdateAnglePointAndLine()
    {
        Vector2 startPosition = GetPositionRectQPoint();
        Vector2 endPosition = startPosition + Vector2.up * anglePointParameterLineLength;
        anglePointParameterLine.SetPositions(startPosition, endPosition, true);
        anglePointParameter.SetPosition(endPosition);
        anglePointParameterDisplay.SetPosition(endPosition);
    }

    private void ResetPosition()
    {
        rectTransform.anchoredPosition = Vector2.zero;
    }

    // COOLEST convention tells that the angle is counter-clockwise from the positive y axis
    private void UpdateAngleDisplay()
    {
        if (!base.rectTransform) return;

        base.rectTransform.rotation = Quaternion.Euler(0f, 0f , angle);

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

    // Update the delta size of the RectTransform attached to the ellipse
    private void UpdateRectTransformSize()
    {
        if (!base.rectTransform) return;

        base.rectTransform.sizeDelta = new Vector2(widthX * 2, widthY * 2);
    }

    // Move the ellipse to the newPosition (in screen position)
    public void MoveScreenPosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    // Move the ellipse to the newPosition (in rect position)
    public void MoveRectPosition(Vector2 newPosition)
    {
        if (!base.rectTransform) return;

        base.rectTransform.anchoredPosition = newPosition;
    }

    // Update the position's parameters and, if needed, save the newPosition (converted in RectTransform position) in centerPos 
    public void SetCenterPosition(Vector2 newPosition, bool trigger = true)
    {
        currentCenterPosition = newPosition;

        if (centerPointParameter)
        {
            // Set the position of the point at the center of the ellipse
            centerPointParameter.SetPosition(Vector2.zero);
        }

        if (centerPointParameterDisplay)
        {
            centerPointParameterDisplay.SetValueText(PositionCenterToString(currentCenterPosition));
        }

        // Update the Y axis line if there is one
        if (axisYRotation)
        {
            axisYRotation.SetPositions(newPosition, newPosition + GetPositionRectQPoint(), true);
        }

        // Update the circular arc if there is one
        if (arcAngleRotation)
        {
            arcAngleRotation.SetPosition(newPosition);
        }

        if (trigger)
        {
            TriggerPositionChanged(newPosition, beginDragPosition);
        }
    }

    public void SetAngle(float newAngle)
    {
        // The angle is within [0, 360] degree
        if (newAngle < 0)
        { 
            newAngle += 360f;
        }
        angle = newAngle % 360;
    }

    public void SetWidthX(float newValue)
    {
        widthX = Mathf.Abs(newValue);

        // This will redraw the ellipse
        SetVerticesDirty();
        UpdateRectTransformSize();
    }

    public void SetWidthY(float newValue)
    {    
        widthY = Mathf.Abs(newValue);

        // Update the semi-major axis line if there is one
        if (semiMajorAxisLine)
        {
            semiMajorAxisLine.SetPositions(Vector2.zero, GetPositionRectQPoint(), true);
        }

        // Update the Y axis line if there is one
        if (axisYRotation)
        {
            axisYRotation.SetPositions(currentCenterPosition, currentCenterPosition + GetPositionRectQPoint(), true);
        }

        // This will redraw the ellipse
        SetVerticesDirty();
        UpdateRectTransformSize();
    }

    public void SetWidthWithCheckAxis(float semiMinor, float semiMajor, bool isSemiMajorOnYAxis)
    {
        // If the semi major axis is on Y axis
        if (isSemiMajorOnYAxis)
        {
            SetWidthY(semiMajor);
            SetWidthX(semiMinor);
            return;
        }
        
        // If the semi major axis is on X axis
        SetWidthY(semiMinor);
        SetWidthX(semiMajor);
    }

    // Set the semi major axis to a new value and update the semi minor axis accordingly
    public void SetSemiMajorAxis(float semiMajor, bool isSemiMajorOnYAxis = true)
    {
        // Compute the delta between the oldValue and the newValue to update accordingly the semi minor axis (widthX)
        float semiMinor = ComputeMinorAxis(semiMajor, q);

        SetWidthWithCheckAxis(semiMinor, semiMajor, isSemiMajorOnYAxis);
    }

    // Set the semi minor axis to a new value and update the semi major axis accordingly
    public void SetSemiMinorAxis(float semiMinor, bool isSemiMajorOnYAxis = true)
    {
        // Compute the delta between the oldValue and the newValue to update accordingly the semi major axis (widthY)
        float semiMajor = ComputeMajorAxis(semiMinor, q);

        SetWidthWithCheckAxis(semiMinor, semiMajor, isSemiMajorOnYAxis);
    }

    public void SetQWithYAxis(float axisValue)
    {
        // The Y axis should always be the semi major axis
        if (axisValue < einsteinRadius)
        {
            axisValue = einsteinRadius;
        }

        // Compute the delta between the oldValue and the newValue to update accordingly widthX
        float delta = axisValue - widthY;

        // The X axis should never be negative
        if (widthX - delta < 0)
        {
            return;
        }

        // The major axis here is always widthY
        SetWidthY(axisValue);
        SetWidthX(widthX - delta);

        SetQ(ComputeRatioQ());
    }

    public void SetEinsteinRadius(float newEinsteinRadius)
    {
        einsteinRadius = newEinsteinRadius;

        if (einsteinPointParameterDisplay)
        {
            einsteinPointParameterDisplay.SetValueText(EinsteinRadiusToString(einsteinRadius));
        }
    }

    public void SetQ(float newQ)
    {
        q = newQ;

        if (qPointParameterDisplay)
        {
            qPointParameterDisplay.SetValueText(QValueToString(q));
        }
    }

    // This draw the ellipse with the given Einstein radius and keep the same q ratio
    public void DrawEllipseGivenEinsteinRadiusAndQ(float newEinstein, float newQ, bool setNewValueEinstein = true, bool setNewValueQ = true, bool isSemiMajorOnYAxis = true)
    {
        if (setNewValueEinstein)
        {
            SetEinsteinRadius(newEinstein);
        }

        if (setNewValueQ)
        {
            SetQ(newQ);
        }

        float deltaAxis = newEinstein * (1 - newQ) / (newQ + 1);

        // If the semi major axis is on Y axis
        if (isSemiMajorOnYAxis)
        {
            SetWidthY(newEinstein + deltaAxis);
            SetWidthX(newEinstein - deltaAxis);
            return;
        }

        // If the semi major axis is on X axis
        SetWidthY(newEinstein - deltaAxis);
        SetWidthX(newEinstein + deltaAxis);
    }

    // Return true if the given position lies on the edges of the ellipse
    // Return false otherwise
    // The position is given in Screen position
    public bool IsPositionOnEllipse(Vector2 position)
    {
        // Since the point is given in Screen position we have to convert it to position in RectTransform
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, position, 
            GetComponentInParent<Canvas>().worldCamera, out localPosition);
        
        // If the position is on the ellipse then the following equation is satisfied :
        // (x-h)^2/a^2 + (y-k)^2/b^2 = 1, with the center of the ellipse being (h,k)
        // For this first result, we want to check that the point lies or is within the ellipse (so <= 1)
        float partX = Mathf.Pow(localPosition.x, 2f) / Mathf.Pow(widthX, 2f);
        float partY = Mathf.Pow(localPosition.y, 2f) / Mathf.Pow(widthY, 2f);

        float firstResult = partX + partY;

        // But since the ellipse has a thickness,
        // We want to check if the point lies or is outside the ellipse - thickness (so >= 1)
        float partXWithThickness = Mathf.Pow(localPosition.x, 2f) / Mathf.Pow(widthX-thickness, 2f);
        float partYWithThickness = Mathf.Pow(localPosition.y, 2f) / Mathf.Pow(widthY-thickness, 2f);

        float secondResult = partXWithThickness + partYWithThickness;

        return firstResult <= 1f && secondResult >= 1;
    }

    // Return the q ratio that corresponds to (b/a)
    // With b as the semi-minor axis and a as the semi-major axis
    public float ComputeRatioQ()
    {
        float a, b;

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

    public void ResizeWidthOnCursorPosition(Vector2 beginPosition, Vector2 cursorPosition)
    {
        // If the Drag is in an area where beginPosition.x > beginPosition.y
        // Then resize the width along the X axis
        // This is done similarly for the y axis
        Vector2 localBeginPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, beginPosition, 
            GetComponentInParent<Canvas>().worldCamera, out localBeginPosition);

        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, cursorPosition, 
            GetComponentInParent<Canvas>().worldCamera, out localPosition);

        if (Mathf.Abs(localBeginPosition.x) > Mathf.Abs(localBeginPosition.y))
        {   
            SetWidthX(localPosition.x);
        }
        else 
        {
            SetWidthY(localPosition.y);
        }
    }

    // Compute the position (position in rectTransform) of the point that influences the Q value
    public Vector2 GetPositionRectQPoint()
    {
        // The Q point will always be on the ellipse at (centerPos.x, centerPos.y + widthY)
        // We remove the half of the thickness so that the point is centered
        return Vector2.up * (widthY - thickness/2);
    }

    // Compute the position (position in rectTransform) of the point that influences the Q value
    public Vector2 GetPositionRectEinsteinPoint()
    {
        // The Q point will always be on the ellipse at (centerPos.x, centerPos.y + widthY)
        // We remove the half of the thickness so that the point is centered
        return Vector2.right * (widthX - thickness/2);
    }

    public Vector2 ConvertScreenPositionInEllipseRect(Vector2 screenPosition)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, 
            GetComponentInParent<Canvas>().worldCamera, out localPosition);

        return localPosition;
    }

    // "Magnet Effect" for the position :
    // If the position at the end of the drag is less than distanceMagnetCenter to the center position
    // Then set the position to (0,0) (which is the center)
    public void MagnetCenterPoint()
    {
        if (currentCenterPosition.magnitude <= distanceMagnetCenter)
        {
            ResetPosition();
            // The center position 
            SetCenterPosition(Vector2.zero, false);
        }
    }

    // "Magnet Effect" for the q ratio :
    // If the q obtained at the end of the drag is in the range  [1 - distanceMagnetQ, 1], then set it to 1
    public void MagnetQPoint()
    {
        if ((1f - q) <= distanceMagnetQ)
        {
            SetQ(1f);
            DrawEllipseGivenEinsteinRadiusAndQ(einsteinRadius, q, false, false);
            UpdatePointsParametersPositions();
        }
    }

    // "Magnet Effect" for the angle :
    // If the angle obtained at the end of the drag is in the range  [x - distanceMagnetAngle, x + distanceMagnetAngle],
    // for x in {0;90;180;270;360} then set it to the corresponding angle
    public void MagnetAnglePoint()
    {
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
            SetAngle(lowestDifferenceAngle * 90f);
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

    // TODO : Maybe remove parameters' attributes and only use parameterUI by casting it with the appropriate type 
    private void OnParameterChangedHandler(object sender, Vector2 cursorPosition)
    {
        PointParameterUI parameterUI = sender as PointParameterUI;

        if (parameterUI != null)
        {
            if (parameterUI is QPointUI)
            {
                float convertedY = ConvertScreenPositionInEllipseRect(cursorPosition).y;
                SetQWithYAxis(convertedY);

                // Update the positions of the points parameter
                UpdatePointsParametersPositions();

                if (isInSnapMode)
                {
                    MagnetQPoint();
                }
            } 
            else if (parameterUI is CenterPointUI)
            {
                MoveScreenPosition(cursorPosition);

                Vector2 convertedPosition = rectTransform.anchoredPosition;
                SetCenterPosition(convertedPosition);
            }
            else if (parameterUI is EinsteinPointUI)
            {
                float convertedX = ConvertScreenPositionInEllipseRect(cursorPosition).x;

                // Get the Einstein radius that corresponds to the position X
                float convertedR;

                // If the major axis is on the X axis
                if (widthX > widthY)
                {
                    convertedR = ComputeEinsteinRadius(ComputeMinorAxis(convertedX, q), convertedX);
                    DrawEllipseGivenEinsteinRadiusAndQ(convertedR, q, true, false, false);
                }
                 // If the major axis is on the Y axis
                else
                {
                    convertedR = ComputeEinsteinRadius(convertedX, ComputeMajorAxis(convertedX, q));
                    DrawEllipseGivenEinsteinRadiusAndQ(convertedR, q, true, false, true);
                }
                
                // Update the positions of the points parameter
                UpdatePointsParametersPositions();
            }
            else if (parameterUI is AnglePointUI)
            {
                Vector2 convertedPosition = ConvertScreenPositionInEllipseRect(cursorPosition);

                // if convertedX > 0 => turn clockwise (decrease angle)
                // if convertedX < 0 => turn anti-clockwise (increase angle)
                float sign = (convertedPosition.x > 0) ? -1.0f : 1.0f;

                float deltaAngle = sign * Vector2.Angle(Vector2.up, convertedPosition.normalized);
                SetAngle(angle + deltaAngle);
                UpdateAngleDisplay();

                if (isInSnapMode)
                {
                    MagnetAnglePoint();
                }
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
                TriggerPositionEndDrag(currentCenterPosition, beginDragPosition);
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
        return einsteinRadius.ToString("0.0");
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
        string posX = position.x.ToString("0.0");
        string posY = position.y.ToString("0.0");

        return "("+posX+","+posY+")";
    }

    public bool GetIsInSnapMode()
    {
        return isInSnapMode;
    }

    public float GetAnglePointParameterLineLength()
    {
        return anglePointParameterLineLength;
    }

    public Vector2 GetCenterPositionParameter()
    {
        return currentCenterPosition;
    }

    public float GetQParameter()
    {
        return q;
    }

    public float GetEinsteinRadiusParameter()
    {
        return einsteinRadius;
    }

    public float GetPhiAngleParameter()
    {
        return angle;
    }
}

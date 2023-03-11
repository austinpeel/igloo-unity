using UnityEngine;
using UnityEngine.UI;

public class EllipseUI : Graphic
{
    // One division for each degree seems to be enough
    [SerializeField] private int division = 360;
    [SerializeField] private float widthX = 100f;
    [SerializeField] private float widthY = 200f;
    [SerializeField] private float thickness = 10f;
    [SerializeField] private float angle = 0f;
    [SerializeField] private float distanceMagnetCenter = 25f;
    [SerializeField] private ParametersDisplay parametersDisplay;
    [SerializeField] private QPointUI qPointParameter;
    [SerializeField] private CenterPointUI centerPointParameter;

    private Vector2 currentCenterPosition = Vector2.zero;

    private new void Start()
    {
        base.Start();

        // Subscribe to the events of the different points
        qPointParameter.OnParameterChanged += OnParameterChangedHandler;
        centerPointParameter.OnParameterChanged += OnParameterChangedHandler;
        centerPointParameter.OnParameterEndDrag += OnParameterEndDragHandler;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        // Unsubscribe from the OnParameterChanged event to prevent memory leaks
        qPointParameter.OnParameterChanged -= OnParameterChangedHandler;
        centerPointParameter.OnParameterChanged -= OnParameterChangedHandler;
        centerPointParameter.OnParameterEndDrag -= OnParameterEndDragHandler;
    }

    protected override void OnValidate()
    {
        UpdateAngle(angle);
        UpdateRectTransformSize();
        ComputeRatioQ();
        SetCenterPosition(Vector2.zero);
        InitializePointParameters();
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

    private void InitializePointParameters()
    {
        // The center for the CenterPoint will always be at (0,0)
        centerPointParameter.SetPosition(Vector2.zero);
        qPointParameter.SetPosition(GetPositionRectQPoint());
    }

    private void ResetPosition()
    {
        rectTransform.anchoredPosition = Vector2.zero;
    }

    // COOLEST convention tells that the angle is counter-clockwise from the positive y axis
    private void UpdateAngle(float newAngle)
    {
        if (!base.rectTransform) return;

        base.rectTransform.rotation = Quaternion.Euler(0f, 0f , newAngle);

        if (parametersDisplay)
        {
            parametersDisplay.SetAngleText(newAngle);
        }
    }

    // Update the delta size of the RectTransform attached to the ellipse
    private void UpdateRectTransformSize()
    {
        if (!base.rectTransform) return;

        base.rectTransform.sizeDelta = new Vector2(widthX * 2, widthY * 2);
    }

    // Move the ellipse to the newPosition
    public void MovePosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    // Update the position's parameters and, if needed, save the newPosition (converted in RectTransform position) in centerPos 
    public void SetCenterPosition(Vector2 newPosition)
    {
        currentCenterPosition = newPosition;

        if (centerPointParameter)
        {
            // Set the position of the point at the center of the ellipse
            centerPointParameter.SetPosition(Vector2.zero);
        }

        if (parametersDisplay)
        {
            parametersDisplay.SetPositionCenterText(newPosition);
        }
    }

    public void SetWidthX(float newValue)
    {
        widthX = Mathf.Abs(newValue);

        // This will redraw the ellipse
        SetVerticesDirty();
        UpdateRectTransformSize();

        if (parametersDisplay)
        {
            parametersDisplay.SetQValueText(ComputeRatioQ());
        }
    }

    public void SetWidthY(float newValue)
    {    
        widthY = Mathf.Abs(newValue);

        // This will redraw the ellipse
        SetVerticesDirty();
        UpdateRectTransformSize();

        if (parametersDisplay)
        {
            parametersDisplay.SetQValueText(ComputeRatioQ());
        }
    }

    // Set the semi major axis to a new value and update the semi minor axis accordingly
    public void SetSemiMajorAxis(float newValue)
    {
        // Compute the delta between the oldValue and the newValue to update accordingly the semi minor axis (widthX)
        float delta = newValue - widthY;

        // The major axis here is always widthY
        SetWidthY(newValue);
        SetWidthX(widthX - delta);
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

        if (parametersDisplay)
        {
            parametersDisplay.SetQValueText(b/a);
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
        if (currentCenterPosition.magnitude < distanceMagnetCenter)
        {
            ResetPosition();
            // The center position 
            SetCenterPosition(Vector2.zero);
        }
    }

    private void OnParameterChangedHandler(object sender, Vector2 cursorPosition)
    {
        PointParameterUI parameterUI = sender as PointParameterUI;

        if (parameterUI != null)
        {
            if (parameterUI is QPointUI)
            {
                float convertedY = ConvertScreenPositionInEllipseRect(Vector2.up * cursorPosition.y).y;

                SetSemiMajorAxis(convertedY);

                qPointParameter.SetPosition(GetPositionRectQPoint());
            } 
            else if (parameterUI is CenterPointUI)
            {
                MovePosition(cursorPosition);

                Vector2 convertedPosition = rectTransform.anchoredPosition;
                SetCenterPosition(convertedPosition);
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
                MagnetCenterPoint();   
            }
        }
    }
}

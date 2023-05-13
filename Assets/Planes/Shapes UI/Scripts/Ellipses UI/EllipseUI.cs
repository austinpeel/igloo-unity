using UnityEngine;
using UnityEngine.UI;

public class EllipseUI : Graphic
{
    [SerializeField] private Color ellipseColor = Color.black;
    [SerializeField] private float thickness = 10f;
    [SerializeField] [Range(0, 1)] private float q = 0.5f;
    [SerializeField] private float radius = 1f;
    [SerializeField] [Range(0, 360)] private float angle = 0f;
    [SerializeField] private Vector2 centerPosition = Vector2.zero;

    // One edge for each degree seems to be enough
    private int nbrEdges = 360;
    private float widthX = 100f;
    private float widthY = 200f;
    private ICoordinateConverter coordinateConverter;
    

    protected new void Awake() 
    {
        InitializeCoordinateConverter();
    }

    private new void OnEnable() 
    {
        base.OnEnable();

        InitializeCoordinateConverter();

        RedrawEllipse();
        RedrawAngle();
        RedrawPosition();
    }

    public void InitializeCoordinateConverter()
    {
        coordinateConverter = GetComponentInParent<ICoordinateConverter>();
        
        if (coordinateConverter == null)
        {
            Debug.Log("Can't find the ICoordinateConverter in Parent !");
        }
    }

    // Create the meshs with respect to the chosen parameters of the class
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        vh.Clear();
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = ellipseColor;

        float deltaAngle = Mathf.PI * 2 / nbrEdges;
        int nbrVertex = 2 * nbrEdges;

        for (int i = 0; i < nbrEdges; i++)
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

    public Color GetEllipseColor()
    {
        return ellipseColor;
    }

    public void SetEllipseColor(Color newEllipseColor, bool redraw = false)
    {
        ellipseColor = newEllipseColor;

        if (redraw)
        {
            RedrawEllipse();
        }
    }

    public Vector2 GetCenterPositionParameter()
    {
        return centerPosition;
    }

    public void SetCenterPosition(Vector2 newCenterPosition, bool redraw = false)
    {
        centerPosition = newCenterPosition;

        if (redraw)
        {
            RedrawPosition();
        }
    }

    private void RedrawPosition()
    {
        if (!base.rectTransform || coordinateConverter == null) return;

        Vector2 centerRectPosition = coordinateConverter.ConvertCoordinateToRectPosition(centerPosition);

        // Move the ellipse to the centerRectPosition (in rect position)
        base.rectTransform.anchoredPosition = centerRectPosition;
    }

    public float GetAngleParameter()
    {
        return angle;
    }

    public void SetAngle(float newAngle, bool redraw = false)
    {
        // The angle is within [0, 360] degree
        if (newAngle < 0)
        { 
            newAngle += 360f;
        }
        angle = newAngle % 360;

        if (redraw)
        {
            RedrawAngle();
        }
    }

    private void RedrawAngle()
    {
        if (!base.rectTransform) return;

        base.rectTransform.rotation = Quaternion.Euler(0f, 0f , angle);
    }

    public float GetRadiusParameter()
    {
        return radius;
    }

    // Set the Radius (in coordinate) and update the value displayed
    // (It is a scaling factor, we obtain it by finding the radius of the circle 
    // by the reducing semi-major axis and increasing the semi-minor axis by the same delta until they are equal)
    public void SetRadius(float newRadius, bool redraw = false)
    {
        // The radius should never be negative
        if (newRadius < 0f)
        {
            radius = 0f;
            return;
        }
        
        radius = newRadius;

        if (redraw)
        {
            RedrawEllipse();
        }
    }

    public float GetQParameter()
    {
        return q;
    }

    public void SetQ(float newQ, bool redraw = false)
    {
        q = newQ;

        if (redraw)
        {
            RedrawEllipse();
        }
    }

    // This redraw the ellipse with current parameters : the radius and q ratio
    public void RedrawEllipse()
    { 
        if (coordinateConverter == null)
        {
            Debug.Log("Redraw coordinateConverter is null !");
            return;
        }

        float deltaAxis = radius * (1 - q) / (q + 1);

        Vector2 widthVector = coordinateConverter.ConvertCoordinateToRectPosition(new Vector2(radius - deltaAxis, radius + deltaAxis));

        SetWidthY(widthVector.y);
        SetWidthX(widthVector.x);
    }

    // Get the radius in RectTransform position of the Plane
    public float GetRadiusInRect()
    {
        if (coordinateConverter == null)
        {
            Debug.Log("GetRadiusInRect coordinateConverter is null !");
            return 0f;
        }

        float radiusInRect = coordinateConverter.ConvertCoordinateToRectPosition(Vector2.right * radius).x;
        return radiusInRect;
    }

    // Get the center position in RectTransform position of the Plane
    public Vector2 GetCenterPositionInRect()
    {
        if (coordinateConverter == null)
        {
            Debug.Log("GetCenterPositionInRect coordinateConverter is null !");
            return Vector2.zero;
        }

        Vector2 centerPositionInRect = coordinateConverter.ConvertCoordinateToRectPosition(centerPosition);
        return centerPositionInRect;
    }

    public float GetThickness()
    {
        return thickness;
    }

    public void SetThickness(float newThickness, bool redraw = false)
    {
        thickness = newThickness;

        if (redraw)
        {
            // This will redraw the ellipse
            SetVerticesDirty();
            UpdateRectTransformSize();
        }
    }

    public float GetWidthX()
    {
        return widthX;
    }

    public void SetWidthX(float newValue, bool redraw = true)
    {
        widthX = Mathf.Abs(newValue);

        // This will redraw the ellipse
        SetVerticesDirty();
        UpdateRectTransformSize();
    }

    public float GetWidthY()
    {
        return widthY;
    }

    public void SetWidthY(float newValue)
    {    
        widthY = Mathf.Abs(newValue);

        // This will redraw the ellipse
        SetVerticesDirty();
        UpdateRectTransformSize();
    }

    // Update the delta size of the RectTransform attached to the ellipse
    protected void UpdateRectTransformSize()
    {
        if (!base.rectTransform) return;

        base.rectTransform.sizeDelta = new Vector2(widthX * 2, widthY * 2);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EllipseUI : Graphic
{
    [SerializeField] private float thickness = 10f;
    [SerializeField] [Range(0, 1)] private float q = 0.5f;
    [SerializeField] private float einsteinRadius = 1f;
    [SerializeField] [Range(0, 360)] private float angle = 0f;
    [SerializeField] private Vector2 centerPosition = Vector2.zero;

    // One edge for each degree seems to be enough
    private int nbrEdges = 360;
    private float widthX = 100f;
    private float widthY = 200f;
    private ICoordinateConverter coordinateConverter;
    

    private new void Awake() 
    {
        coordinateConverter = GetComponentInParent<ICoordinateConverter>();
        
        if (coordinateConverter == null)
        {
            Debug.Log("Can't find the ICoordinateConverter in Parent !");
        }
    }

    protected new void OnValidate() 
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
        vertex.color = base.color;

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

    public float GetEinsteinRadiusParameter()
    {
        return einsteinRadius;
    }

    // Set the Einstein radius (in coordinate) and update the value displayed
    public void SetEinsteinRadius(float newEinsteinRadius, bool redraw = false)
    {
        // The Einstein radius should never be negative
        if (newEinsteinRadius < 0f)
        {
            einsteinRadius = 0f;
            return;
        }
        
        einsteinRadius = newEinsteinRadius;

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

    // This redraw the ellipse with current parameters : the Einstein radius and q ratio
    public void RedrawEllipse()
    { 
        if (coordinateConverter == null)
        {
            Debug.Log("Redraw coordinateConverter is null !");
            return;
        }

        float einsteinInRect = coordinateConverter.ConvertCoordinateToRectPosition(Vector2.right * einsteinRadius).x;

        float deltaAxis = einsteinInRect * (1 - q) / (q + 1);

        SetWidthY(einsteinInRect + deltaAxis);
        SetWidthX(einsteinInRect - deltaAxis);
    }

    // Get the Einstein radius in RectTransform position of the Plane
    public float GetEinsteinInRect()
    {
        if (coordinateConverter == null)
        {
            Debug.Log("GetEinsteinInRect coordinateConverter is null !");
            return 0f;
        }

        float einsteinInRect = coordinateConverter.ConvertCoordinateToRectPosition(Vector2.right * einsteinRadius).x;
        return einsteinInRect;
    }

    // Get the center position in RectTransform position of the Plane
    public Vector2 GetCenterPositionInRect()
    {
        if (coordinateConverter == null)
        {
            Debug.Log("GetEinsteinInRect coordinateConverter is null !");
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

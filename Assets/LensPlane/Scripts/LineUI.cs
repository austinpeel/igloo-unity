using UnityEngine;
using UnityEngine.UI;

public class LineUI : Graphic
{
    [SerializeField] private Vector2 positionStart = Vector2.zero;
    [SerializeField] private Vector2 positionEnd = Vector2.up;
    [SerializeField] [Range(0,20)] private float width = 2f;
    private float rotationAngle = 0f;


    // Create the meshs with respect to the chosen parameters of the class
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        vh.Clear();

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = base.color;

        Vector2 lineVector = positionEnd - positionStart;

        // Compute the magnitude of the wanted line
        float length = lineVector.magnitude;

        // Calculate the normalized perpendicular of the line to take care of the width for the position
        Vector2 perpendicularWidth = Vector2.right * width / 2f;
        Vector2 directionLength = Vector2.up * length / 2f;

        // Create a line with this magnitude and put it along Y axis => from (0,0) to (0,magnitude)
        vertex.position = - directionLength + perpendicularWidth;
        vh.AddVert(vertex);

        vertex.position = - directionLength - perpendicularWidth;
        vh.AddVert(vertex);

        vertex.position = directionLength + perpendicularWidth;
        vh.AddVert(vertex);

        vertex.position = directionLength - perpendicularWidth;
        vh.AddVert(vertex);

        // Connect the vertices with 2 triangles
        vh.AddTriangle(0,1,3);
        vh.AddTriangle(0,3,2);
    }

    protected override void OnValidate()
    {
        UpdateAll();
    }

    private void UpdateAll()
    {
        UpdateRectTransformSize();
        UpdateRectTransformAngle();
        UpdateRectTransformAnchoredPosition();
    }

    // Update the delta size of the RectTransform attached to the line
    private void UpdateRectTransformSize()
    {
        if (!base.rectTransform) return;

        float length = (positionEnd - positionStart).magnitude;

        base.rectTransform.sizeDelta = new Vector2(width, length);
    }


    private void UpdateRectTransformAngle()
    {
        Vector2 lineVector = positionEnd - positionStart;
        float angle = Vector2.SignedAngle(Vector2.up, lineVector.normalized);

        base.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle + rotationAngle);
    }


    private void UpdateRectTransformAnchoredPosition()
    {
        Vector2 currentLineVector = positionEnd - positionStart;
        float length = currentLineVector.magnitude;

        base.rectTransform.anchoredPosition = positionStart + currentLineVector.normalized * length/2f;
    }

    public void SetPositionStart(Vector2 newPositionStart, bool redraw = false)
    {
        positionStart = newPositionStart;

        if (redraw)
        {
            // This will redraw the line
            SetVerticesDirty();
            UpdateAll();
        }
    }

    public void SetPositionEnd(Vector2 newPositionEnd, bool redraw = false)
    {
        positionEnd = newPositionEnd;

        if (redraw)
        {
            // This will redraw the line
            SetVerticesDirty();
            UpdateAll();
        }
    }

    public void SetPositions(Vector2 newPositionStart, Vector2 newPositionEnd, bool redraw = false)
    {
        positionStart = newPositionStart;
        positionEnd = newPositionEnd;

        if (redraw)
        {
            // This will redraw the line
            SetVerticesDirty();
            UpdateAll();
        }
    }

    public void SetWidth(float newWidth, bool redraw = false)
    {
        width = newWidth;

        if (redraw)
        {
            // This will redraw the line
            SetVerticesDirty();
            UpdateAll();
        }
    }

    public void SetRotationAngle(float newRotationAngle, bool redraw = false)
    {
        rotationAngle = newRotationAngle;

        if (redraw)
        {
            // No need to redraw the line, only update the rotation
            UpdateRectTransformAngle();
        }
    }

    public void SetColor(Color newColor, bool redraw = false)
    {
        base.color = newColor;

        if (redraw)
        {
            // This will redraw the line
            SetVerticesDirty();
        }
    }

}

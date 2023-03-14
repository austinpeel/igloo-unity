using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineUI : Graphic
{
    [SerializeField] private Vector2 positionStart = Vector2.zero;
    [SerializeField] private Vector2 positionEnd = Vector2.up;
    [SerializeField] private float width = 2f;


    // Create the meshs with respect to the chosen parameters of the class
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        vh.Clear();

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = base.color;

        // Calculate the normalized perpendicular of the line to take care of the width for the position
        Vector2 perpendicular = Vector2.Perpendicular(positionEnd - positionStart).normalized;

        vertex.position = positionStart + perpendicular * width/2f;
        vh.AddVert(vertex);

        vertex.position = positionStart - perpendicular * width/2f;
        vh.AddVert(vertex);

        vertex.position = positionEnd + perpendicular * width/2f;
        vh.AddVert(vertex);

        vertex.position = positionEnd - perpendicular * width/2f;
        vh.AddVert(vertex);

        // Connect the vertices with 2 triangles
        vh.AddTriangle(0,1,3);
        vh.AddTriangle(0,3,2);
    }

    protected override void OnValidate()
    {
        UpdateRectTransformSize();
    }

    // Update the delta size of the RectTransform attached to the line
    private void UpdateRectTransformSize()
    {
        if (!base.rectTransform) return;

        float height = (positionEnd - positionStart).magnitude;
        base.rectTransform.sizeDelta = new Vector2(width, height);
    }

    public void SetPositionStart(Vector2 newPositionStart, bool redraw = false)
    {
        positionStart = newPositionStart;

        if (redraw)
        {
            // This will redraw the ellipse
            SetVerticesDirty();
            UpdateRectTransformSize();
        }
    }

    public void SetPositionEnd(Vector2 newPositionEnd, bool redraw = false)
    {
        positionEnd = newPositionEnd;

        if (redraw)
        {
            // This will redraw the ellipse
            SetVerticesDirty();
            UpdateRectTransformSize();
        }
    }

    public void SetPositions(Vector2 newPositionStart, Vector2 newPositionEnd, bool redraw = false)
    {
        positionStart = newPositionStart;
        positionEnd = newPositionEnd;

        if (redraw)
        {
            // This will redraw the ellipse
            SetVerticesDirty();
            UpdateRectTransformSize();
        }
    }

}

using UnityEngine;
using UnityEngine.UI;

public class CircularArcUI : Graphic
{
    [SerializeField] private float radius = 50f;
    [SerializeField] private float thickness = 5f;
    [SerializeField] [Range(0, 360)] private float angle = 1f;

    // Create the meshs with respect to the chosen parameters of the class
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        vh.Clear();

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = base.color;
        
        int nbrVertex = 2 * ((int) angle) + 2;

        // The vertices are placed each degree
        for (int i = 0; i <= angle; i++)
        {
            float radiusMinusThickness = radius - thickness;

            // Add Pi / 2 because of the COOLEST convention

            float angleRadian = i * Mathf.Deg2Rad + Mathf.PI / 2f;

            vertex.position = new Vector2(radiusMinusThickness * Mathf.Cos(angleRadian), radiusMinusThickness * Mathf.Sin(angleRadian));
            vh.AddVert(vertex);

            vertex.position = new Vector2(radius * Mathf.Cos(angleRadian), radius * Mathf.Sin(angleRadian));
            vh.AddVert(vertex);
            
            int offset = i * 2;

            if ((i+1) * 2 == nbrVertex)
            {
                return;
            }

            vh.AddTriangle(offset, offset + 1, (offset + 3) % nbrVertex);
            vh.AddTriangle((offset + 3) % nbrVertex, (offset + 2) % nbrVertex, offset);
        }   
    }

    private new void OnValidate() 
    {
        UpdateRectTransformSize();
    }

    private void UpdateRectTransformSize()
    {
        if (!base.rectTransform) return;

        base.rectTransform.sizeDelta = new Vector2(radius * 2, radius * 2);
    }

    public void SetPosition(Vector2 newPosition)
    {
        if (!base.rectTransform) return;

        base.rectTransform.anchoredPosition = newPosition;
    }

    public void SetAngle(float newAngle, bool redraw = false)
    {
        angle = newAngle;

        if (redraw)
        {
            // This will redraw the circular arc
            SetVerticesDirty();
        }
    }

    public void SetRadius(float newRadius, bool redraw = false)
    {
        radius = newRadius;

        if (redraw)
        {
            // This will redraw the circular arc
            SetVerticesDirty();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class EllipseUI : Graphic
{
    [SerializeField] private int division = 20;
    [SerializeField] private float widthX = 100f;
    [SerializeField] private float widthY = 200f;
    [SerializeField] private float thickness = 10f;
    [SerializeField] private float angle = 0f;
    [SerializeField] private Vector2 centerPos = Vector2.zero;

    protected override void OnValidate()
    {
        updateAngle(angle);
        updateRectTransformSize(widthX, widthY);
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

    // COOLEST convention tells that the angle is counter-clockwise from the positive y axis
    private void updateAngle(float newAngle)
    {
        if (!base.rectTransform) return;

        base.rectTransform.rotation = Quaternion.Euler(0f, 0f , newAngle);
    }

    // Update the delta size of the RectTransform attached to the ellipse
    private void updateRectTransformSize(float a, float b)
    {
        if (!base.rectTransform) return;

        base.rectTransform.sizeDelta = new Vector2(a * 2, b * 2);
    }

    // Set the position of the center of the ellipse and store it in centerPos
    public void SetCenterPosition(Vector2 newPosition)
    {
        transform.position = newPosition;
        centerPos = newPosition;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class EllipseUI : Graphic
{
    [SerializeField] private int division = 20;
    [SerializeField] private float semiMajorAxis = 200f;
    [SerializeField] private float semiMinorAxis = 100f;
    [SerializeField] private float thickness = 10f;

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

            // Bottom Right Corner
            vertex.position = new Vector2((semiMajorAxis - thickness) * Mathf.Sin(angle), (semiMinorAxis - thickness) * Mathf.Cos(angle));
            vh.AddVert(vertex);

            // Top Right Corner
            vertex.position = new Vector2(semiMajorAxis * Mathf.Sin(angle), semiMinorAxis * Mathf.Cos(angle));
            vh.AddVert(vertex);
            
            int offset = i * 2;
            vh.AddTriangle(offset, (offset + 1) % nbrVertex, (offset + 3) % nbrVertex);
            vh.AddTriangle((offset + 3) % nbrVertex, (offset + 2) % nbrVertex, offset);
        }
    }
}

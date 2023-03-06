using UnityEngine;
using UnityEngine.UI;

public class EllipseUI : Graphic
{
    [SerializeField] private int division = 20;
    [SerializeField] private float widthX = 100f;
    [SerializeField] private float widthY = 200f;
    [SerializeField] private float thickness = 10f;
    [SerializeField] private float angle = 0f;
    private Vector2 centerPos = Vector2.zero;
    [SerializeField] private ParametersDisplay parametersDisplay;

    protected override void OnValidate()
    {
        UpdateAngle(angle);
        UpdateRectTransformSize(widthX, widthY);
        ComputeRatioQ();
        SetCenterPosition(Vector2.zero);
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
    private void UpdateRectTransformSize(float a, float b)
    {
        if (!base.rectTransform) return;

        base.rectTransform.sizeDelta = new Vector2(a * 2, b * 2);
    }

    // Move the ellipse to the newPosition
    public void MovePosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    // Store the newPosition (converted in RectTransform position) in centerPos
    public void SetCenterPosition(Vector2 newPosition)
    {
        centerPos = newPosition;

        if (parametersDisplay)
        {
            parametersDisplay.SetPositionCenterText(newPosition);
        }
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
}

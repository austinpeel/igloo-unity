using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DestroyUtils;

public class AxisUI : LineUI
{
    [SerializeField] private bool isAxisX = true;
    [SerializeField] private bool drawTickMarks = true;
    [SerializeField] private float widthTickMarks = 1f;
    [SerializeField] private float lengthTickMarks = 5f;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Image labelAxis;
    private float length = 500f;
    private float maxValue = 4f;
    private List<LineUI> axisScalingLines = new List<LineUI>();
    
    private new void OnDestroy() 
    {
        base.OnDestroy();

        ClearAxisScaling();
    }

    public void SetAxisLength(float newLength, bool redraw = false)
    {
        length = newLength;

        if (redraw)
        {
            Redraw();
        }
    }

    public void SetIsAxisX(bool newIsAxisX, bool redraw = false)
    {
        isAxisX = newIsAxisX;

        if (redraw)
        {
            Redraw();
        }
    }

    public bool GetIsAxisX()
    {
        return isAxisX;
    }

    public void SetDrawTickMarks(bool newDrawTickMarks, bool redraw = false)
    {
        drawTickMarks = newDrawTickMarks;

        if (redraw)
        {
            Redraw();
        }
    }

    public bool GetDrawTickMarks()
    {
        return drawTickMarks;
    }

    public void SetLabelAxis(Image newLabelAxis, bool redraw = false)
    {
        labelAxis = newLabelAxis;

        if (redraw)
        {
            Redraw();
        }
    }

    public Image GetLabelAxis()
    {
        return labelAxis;
    }

    public void SetLinePrefab(GameObject newLinePrefab, bool redraw = false)
    {
        linePrefab = newLinePrefab;

        if (redraw)
        {
            Redraw();
        }
    }

    public GameObject GetLinePrefab()
    {
        return linePrefab;
    }

    // Set the max value of the axis
    public void SetMaxValue(float newMaxValue, bool redraw = false)
    {
        // The maxValue will always be positive
        float absMax = Mathf.Abs(newMaxValue);

        maxValue = absMax;

        if (redraw)
        {
            Redraw();
        }
    }

    // Draw the tick marks on the axis for each arcsec
    private void DrawTickMarks()
    {
        float halfLength = length / 2f;
        float deltaPosition = halfLength / maxValue; 
        float halfTickMarksLength = lengthTickMarks / 2f;

        for (int i = 1; i <= Mathf.FloorToInt(maxValue); i++)
        {
            LineUI linePositive = Instantiate(linePrefab, transform).GetComponent<LineUI>();
            InitializeLine(linePositive, new Vector2(-halfTickMarksLength, i * deltaPosition), new Vector2(halfTickMarksLength, i * deltaPosition));
            axisScalingLines.Add(linePositive);

            LineUI lineNegative = Instantiate(linePrefab, transform).GetComponent<LineUI>();
            InitializeLine(lineNegative, new Vector2(-halfTickMarksLength, -i * deltaPosition), new Vector2(halfTickMarksLength, -i * deltaPosition));
            axisScalingLines.Add(lineNegative);
        }
    }

    private void InitializeLine(LineUI line, Vector2 positionStart, Vector2 positionEnd)
    {
        line.SetColor(GetColor());
        line.SetWidth(widthTickMarks);
        line.SetPositionStart(positionStart);
        line.SetPositionEnd(positionEnd, true);

        float angle = isAxisX ? 90f : 0f;
        line.SetRotationAngle(angle, true);
    }

    public void ClearAxisScaling()
    {
        foreach(LineUI line in axisScalingLines)
        {
            SafeDestroy(line.gameObject);
        }

        axisScalingLines.Clear();
    }

    public void Redraw()
    {
        float halfLength = length / 2f;
        
        if (isAxisX)
        {
            SetPositions(Vector2.left * halfLength, Vector2.right * halfLength, true);
            UpdateLabelPosition();
        }
        else 
        {
            SetPositions(Vector2.down * halfLength, Vector2.up * halfLength, true);
            UpdateLabelPosition();
        }

        ClearAxisScaling();

        if (drawTickMarks)
        {
            DrawTickMarks();
        }
    }

    private void UpdateLabelPosition()
    {
        if (!labelAxis) return;

        labelAxis.rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);

        float halfLength = length / 2f;
        Vector2 labelPosition = new Vector2(labelAxis.rectTransform.rect.height * labelAxis.rectTransform.localScale.x, halfLength);

        labelAxis.rectTransform.anchoredPosition = labelPosition - Vector2.up * (labelAxis.rectTransform.rect.width * labelAxis.rectTransform.localScale.x);
    }
}

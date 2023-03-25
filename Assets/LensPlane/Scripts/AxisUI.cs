using UnityEngine;
using UnityEngine.UI;

public class AxisUI : LineUI
{
    [SerializeField] private bool isAxisX = true;
    [SerializeField] private Image labelAxis;
    [SerializeField] private float offsetLabel = 6f;
    private float length = 0f;
    public void SetAxisLength(float newLength, bool redraw = true)
    {
        length = newLength;

        if (redraw)
        {
            Redraw();
        }
    }

    public void SetIsAxisX(bool newIsAxisX, bool redraw = true)
    {
        isAxisX = newIsAxisX;

        if (redraw)
        {
            Redraw();
        }
    }

    private void Redraw()
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
    }

    private void UpdateLabelPosition()
    {
        if (!labelAxis) return;

        float halfLength = length / 2f;
        Vector2 labelPosition = new Vector2(labelAxis.rectTransform.rect.height * labelAxis.rectTransform.localScale.x, halfLength);

        labelAxis.rectTransform.anchoredPosition = labelPosition - Vector2.up * (labelAxis.rectTransform.rect.width * labelAxis.rectTransform.localScale.x);
    }
}

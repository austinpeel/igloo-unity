using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using static DestroyUtils;

public class AxisUI : LineUI
{
    [SerializeField] private bool isAxisX = true;
    [SerializeField] private bool drawTickMarks = true;
    [SerializeField] private float widthTickMarks = 1f;
    [SerializeField] private float lengthTickMarks = 5f;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Image labelAxis;
    [SerializeField] private Color colorTickMarksLabels = Color.black;
    [SerializeField] private float scaleTickMarksLabels = 0.06f;
    [SerializeField] private float offsetTickMarksLabels = 5f;
    [SerializeField] private NumbersSpritesAxis numbersAxisSprites;
    private const string SPRITE_PATH = "Assets/Sprites/Numbers/";
    private float length = 500f;
    private float maxValue = 4f;
    private List<LineUI> tickMarkLines = new List<LineUI>();
    
    private new void OnDestroy() 
    {
        base.OnDestroy();

        ClearTickMarks();
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

        if (labelAxis) labelAxis.raycastTarget = false;

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

    public void SetColorTickMarksLabels(Color newColorTickMarksLabels, bool redraw = false)
    {
        colorTickMarksLabels = newColorTickMarksLabels;

        if (redraw)
        {
            Redraw();
        }
    }

    public Color GetColorTickMarksLabels()
    {
        return colorTickMarksLabels;
    }

    public void SetScaleTickMarksLabels(float newScaleTickMarksLabels, bool redraw = false)
    {
        scaleTickMarksLabels = newScaleTickMarksLabels;

        if (redraw)
        {
            Redraw();
        }
    }

    public float GetScaleTickMarksLabels()
    {
        return scaleTickMarksLabels;
    }

    public void SetOffsetTickMarksLabels(float newOffsetTickMarksLabels, bool redraw = false)
    {
        offsetTickMarksLabels = newOffsetTickMarksLabels;

        if (redraw)
        {
            Redraw();
        }
    }

    public float GetOffsetTickMarksLabels()
    {
        return offsetTickMarksLabels;
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

    public void SetNumbersAxisSprites(NumbersSpritesAxis newNumbersAxisSprites, bool redraw = false)
    {
        numbersAxisSprites = newNumbersAxisSprites;

        if (redraw)
        {
            Redraw();
        }
    }

    public NumbersSpritesAxis GetNumbersAxisSprites()
    {
        return numbersAxisSprites;
    }

    // Draw the tick marks on the axis for each arcsec
    private void DrawTickMarks()
    {
        float halfLength = length / 2f;
        float deltaPosition = halfLength / maxValue; 
        float halfTickMarksLength = lengthTickMarks / 2f;
        Sprite spriteLabelTickMark;
        GameObject labelTickMarkGameObject;

        for (int i = 1; i <= Mathf.FloorToInt(maxValue); i++)
        {
            // Positive Part
            LineUI linePositive = Instantiate(linePrefab, transform).GetComponent<LineUI>();
            InitializeLine(linePositive, new Vector2(-halfTickMarksLength, i * deltaPosition), new Vector2(halfTickMarksLength, i * deltaPosition));
            tickMarkLines.Add(linePositive);

            if (numbersAxisSprites)
            {
                // Load the sprite of the corresponding label (works for numbers from 1 to 9)
                // The index can be calculated that way because the sprites are put in a specific order in the ScriptableObject
                // If it's not the case, then you can loop through the list and use the value stored with the sprite to find the appropriate sprite. (less efficient)
                int spriteIndex = 2*(i-1);

                if (spriteIndex < numbersAxisSprites.numberSprites.Length)
                {
                    spriteLabelTickMark = numbersAxisSprites.numberSprites[spriteIndex].sprite;
                    labelTickMarkGameObject = new GameObject("Label_"+i);
                    labelTickMarkGameObject.transform.SetParent(linePositive.transform);

                    InitializeLabelTickMark(labelTickMarkGameObject.AddComponent<Image>(), spriteLabelTickMark);
                }
            }

            // Negative Part
            LineUI lineNegative = Instantiate(linePrefab, transform).GetComponent<LineUI>();
            InitializeLine(lineNegative, new Vector2(-halfTickMarksLength, -i * deltaPosition), new Vector2(halfTickMarksLength, -i * deltaPosition));
            tickMarkLines.Add(lineNegative);

            if (numbersAxisSprites)
            {
                // Load the sprite of the corresponding label (works for numbers from -1 to -9)
                // The index can be calculated that way because the sprites are put in a specific order in the ScriptableObject
                // If it's not the case, then you can loop through the list and use the value stored with the sprite to find the appropriate sprite. (less efficient)
                int spriteIndex = 2*(i-1)+1;

                if (spriteIndex < numbersAxisSprites.numberSprites.Length)
                {
                    spriteLabelTickMark = numbersAxisSprites.numberSprites[spriteIndex].sprite;
                    labelTickMarkGameObject = new GameObject("Label_"+"-"+i);
                    labelTickMarkGameObject.transform.SetParent(lineNegative.transform);

                    InitializeLabelTickMark(labelTickMarkGameObject.AddComponent<Image>(), spriteLabelTickMark);
                }
            }
        }
    }

    private void InitializeLabelTickMark(Image labelTickMark, Sprite spriteLabelTickMark)
    {
        labelTickMark.sprite = spriteLabelTickMark;
        labelTickMark.color = colorTickMarksLabels;
        labelTickMark.SetNativeSize();
        labelTickMark.rectTransform.localScale = Vector2.one * scaleTickMarksLabels;
        labelTickMark.rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);

        float halfWidth = labelTickMark.rectTransform.sizeDelta.x * scaleTickMarksLabels / 2f;
        labelTickMark.rectTransform.anchoredPosition = Vector2.down * (halfWidth + offsetTickMarksLabels);
        labelTickMark.raycastTarget = false;
    }

    private void InitializeLine(LineUI line, Vector2 positionStart, Vector2 positionEnd)
    {
        line.SetColor(GetColor());
        line.SetWidth(widthTickMarks);
        line.SetPositionStart(positionStart);
        line.SetPositionEnd(positionEnd, true);

        float angle = isAxisX ? 90f : 0f;
        line.SetRotationAngle(angle, true);
        line.raycastTarget = false;
    }

    public void ClearTickMarks()
    {
        foreach(LineUI line in tickMarkLines)
        {
            SafeDestroy(line.gameObject);
        }

        tickMarkLines.Clear();

        // Check that there is still one child (Label of the axis)
        if (transform.childCount > 1)
        {
            for (int i = transform.childCount; i > 1; i--)
            {
                // Don't Destroy the Label of the axis (it is child at index 0)
                SafeDestroy(transform.GetChild(1).gameObject);
            }
        }
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

        ClearTickMarks();

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using static DestroyUtils;

public class GridUI : Graphic
{
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private int numberLineX = 0;
    [SerializeField] private int numberLineY = 0;
    [SerializeField] private float widthLines = 1f;
    [SerializeField] private Color colorLines = Color.gray;
    [SerializeField] private bool isVisible = false;
    private float width = 0f;
    private float height = 0f;
    private List<LineUI> linesX, linesY;
    public const string linesXListKey = "linesXList";
    public const string linesYListKey = "linesYList";

    private new void Awake() 
    {
        InitializeGrid();
        UpdateGrid();
    }

    public void InitializeGrid()
    {
        RectTransform rect = GetComponentInParent<RectTransform>();

        width = rect.rect.width;
        height = rect.rect.height;

        base.rectTransform.sizeDelta = new Vector2(width, height);

        if (linesX == null)
        {
            linesX = new List<LineUI>();
        }

        if (linesY == null)
        {
            linesY = new List<LineUI>();
        }
    }

    public void SetGridVisibility(bool newIsVisible, bool redraw = false)
    {
        isVisible = newIsVisible;

        gameObject.SetActive(isVisible);

        if (redraw)
        {
            UpdateGrid();
        }
    }

    public bool GetGridVisibility()
    {
        return isVisible;
    }

    public void SetNumberLineX(int newNumberLineX, bool redraw = false)
    {
        numberLineX = newNumberLineX;

        if (redraw)
        {
            UpdateGrid();
        }
    }

    public int GetNumberLineX()
    {
        return numberLineX;
    }

    public void SetNumberLineY(int newNumberLineY, bool redraw = false)
    {
        numberLineY = newNumberLineY;

        if (redraw)
        {
            UpdateGrid();
        }
    }

    public int GetNumberLineY()
    {
        return numberLineY;
    }

    public void SetWidthLines(float newWidthLines, bool redraw = false)
    {
        widthLines = newWidthLines;

        if (redraw)
        {
            UpdateGrid();
        }
    }

    public float GetWidthLines()
    {
        return widthLines;
    }

    public void SetLinePrefab(GameObject newLinePrefab, bool redraw = false)
    {
        linePrefab = newLinePrefab;

        if (redraw)
        {
            UpdateGrid();
        }
    }

    public GameObject GetLinePrefab()
    {
        return linePrefab;
    }

    public void SetColorLines(Color newColorLines, bool redraw = false)
    {
        colorLines = newColorLines;

        if (redraw)
        {
            UpdateGrid();
        }
    }

    public Color GetColorLines()
    {
        return colorLines;
    }

    public void UpdateGrid()
    {
        ClearAllLines();
        InstantiateAllLines();
        SetGridVisibility(isVisible);
    }

    private void InstantiateAllLines()
    {
        if (!linePrefab) return;

        // If the number of line for an axis is odd
        // Just use the even number below 
        // (eg. if numberLineX = 7, then draw the same as if numberLineX = 6)

        if (numberLineX % 2 == 1)
        {
            numberLineX -= 1;
        }

        if (numberLineY % 2 == 1)
        {
            numberLineY -= 1;
        }

        float halfNumberLineX = numberLineX / 2f;
        float halfNumberLineY = numberLineY / 2f;

        float startX = - width / 2f;
        float startY = - height / 2f;

        // Instantiate the lines that are parallel to axis X
        float deltaY = (height / 2f) / (halfNumberLineX + 1);
        
        for (int i = 0; i < halfNumberLineX; i++)
        {
            int index = 2 * i;
            linesX.Add(Instantiate(linePrefab, transform).GetComponent<LineUI>());
            InitializeLine(linesX[index], new Vector2(startX, (i+1) * deltaY), new Vector2(-startX, (i+1) * deltaY));

            linesX.Add(Instantiate(linePrefab, transform).GetComponent<LineUI>());
            InitializeLine(linesX[index+1], new Vector2(startX, -(i+1) * deltaY),new Vector2(-startX, -(i+1) * deltaY));
        }

        // Instantiate the lines that are parallel to axis Y
        float deltaX = (width / 2f) / (halfNumberLineY + 1);

        for (int i = 0; i < halfNumberLineY; i++)
        {
            int index = 2 * i;
            linesY.Add(Instantiate(linePrefab, transform).GetComponent<LineUI>());
            InitializeLine(linesY[index], new Vector2((i+1) * deltaX, startY), new Vector2((i+1) * deltaX, -startY));

            linesY.Add(Instantiate(linePrefab, transform).GetComponent<LineUI>());
            InitializeLine(linesY[index+1], new Vector2(-(i+1) * deltaX, startY), new Vector2(-(i+1) * deltaX, -startY));
        }
    }

    public void ClearAllLines()
    {
        foreach (LineUI line in linesX)
        {
            SafeDestroyGameObject(line);
        }

        foreach (LineUI line in linesY)
        {
            SafeDestroyGameObject(line);
        }

        linesX.Clear();
        linesY.Clear();
    }

    private void InitializeLine(LineUI line, Vector2 positionStart, Vector2 positionEnd)
    {
        line.SetColor(colorLines);
        line.SetWidth(widthLines);
        line.SetPositionStart(positionStart);
        line.SetPositionEnd(positionEnd, true);
    }
}

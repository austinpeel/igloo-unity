using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUI : MonoBehaviour
{
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private int numberLineX = 0;
    [SerializeField] private int numberLineY = 0;
    [SerializeField] private float widthLines = 1f;
    [SerializeField] private Color colorLines = Color.gray;
    private float width = 0f;
    private float height = 0f;
    private RectTransform rectTransform;
    private List<LineUI> linesX, linesY;

    private void Awake() 
    {
        RectTransform rect = GetComponentInParent<RectTransform>();

        width = rect.rect.width;
        height = rect.rect.height;

        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(width, height);

        linesX = new List<LineUI>();
        linesY = new List<LineUI>();

        InstantiateAllLines();
    }

    public void SetGridVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    private void InstantiateAllLines()
    {
        if (!linePrefab) return;

        linesX.Clear();
        linesY.Clear();

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

        float deltaX = (width / 2f) / (halfNumberLineX + 1);
        float deltaY = (height / 2f) / (halfNumberLineY + 1);

        // Instantiate the lines that are parallel to axis X
        for (int i = 0; i < halfNumberLineX; i++)
        {
            int index = 2 * i;
            linesX.Add(Instantiate(linePrefab, transform).GetComponent<LineUI>());
            InitializeLine(linesX[index], new Vector2(startX, (i+1) * deltaY), new Vector2(-startX, (i+1) * deltaY));

            linesX.Add(Instantiate(linePrefab, transform).GetComponent<LineUI>());
            InitializeLine(linesX[index+1], new Vector2(startX, -(i+1) * deltaY),new Vector2(-startX, -(i+1) * deltaY));
        }

        // Instantiate the lines that are parallel to axis Y
        for (int i = 0; i < halfNumberLineY; i++)
        {
            int index = 2 * i;
            linesY.Add(Instantiate(linePrefab, transform).GetComponent<LineUI>());
            InitializeLine(linesY[index], new Vector2((i+1) * deltaX, startY), new Vector2((i+1) * deltaX, -startY));

            linesY.Add(Instantiate(linePrefab, transform).GetComponent<LineUI>());
            InitializeLine(linesY[index+1], new Vector2(-(i+1) * deltaX, startY), new Vector2(-(i+1) * deltaX, -startY));
        }
    }

    private void InitializeLine(LineUI line, Vector2 positionStart, Vector2 positionEnd)
    {
        line.SetColor(colorLines);
        line.SetWidth(widthLines);
        line.SetPositionStart(positionStart);
        line.SetPositionEnd(positionEnd, true);
    }
}

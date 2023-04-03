using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Vector2 ConvertRectPositionToCoordinate(RectTransform rect, Vector2 rectPosition, float xMax, float yMax)
    {
        if (!rect) return Vector2.zero;

        float widthRect = rect.rect.width;
        float heightRect = rect.rect.height;

        float scaleXPerRect = xMax / (widthRect / 2f);
        float scaleYPerRect = yMax / (heightRect / 2f);

        return new Vector2(rectPosition.x * scaleXPerRect, rectPosition.y * scaleYPerRect);
    }

    public static Vector2 ConvertCoordinateToRectPosition(RectTransform rect, Vector2 coordinate, float xMax, float yMax)
    {
        if (!rect) return Vector2.zero;

        float widthRect = rect.rect.width;
        float heightRect = rect.rect.height;

        float scaleXPerCoord = (widthRect / 2f) / xMax;
        float scaleYPerCoord = (heightRect / 2f) / yMax;

        return new Vector2(coordinate.x * scaleXPerCoord, coordinate.y * scaleYPerCoord);
    }

    public static Vector2 ConvertScreenPositionToRect(RectTransform rect, Camera camera, Vector2 screenPosition)
    {
        if (!rect) return Vector2.zero;

        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPosition, 
            camera, out localPosition);

        return localPosition;
    }
}

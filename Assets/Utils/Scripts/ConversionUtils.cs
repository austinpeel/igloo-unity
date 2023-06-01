using UnityEngine;

public class ConversionUtils : MonoBehaviour
{
    // Takes the angle that was measured counter-clockwise from the positive x axis 
    // and convert it to respect the COOLEST coordinate system (counter-clockwise from the positive y axis)
    // If convertToRad is set to true then convert the obtained angle in radian
    public static float ConvertAngleDegToCoolest(float angleDegree, bool convertToRad = false)
    {
        float convertedAngle = angleDegree - 90f;

        if (convertedAngle < 0f) convertedAngle += 360f;

        if (convertToRad) return Mathf.Deg2Rad * convertedAngle;

        return convertedAngle;
    }

    // Takes the angle that was measured respecting the COOLEST coordinate system (counter-clockwise from the positive y axis) 
    // and convert it to a coordinate system that measures counter-clockwise from the positive x axis
    // If convertToRad is set to true then convert the obtained angle in radian
    public static float ConvertAngleCoolestToDeg(float angleCoolest, bool convertToRad = false)
    {
        float convertedAngle = (angleCoolest + 90f) % 360f;

        if (convertToRad) return Mathf.Deg2Rad * convertedAngle;

        return convertedAngle;
    }

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

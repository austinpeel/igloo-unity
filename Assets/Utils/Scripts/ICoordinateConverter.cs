using UnityEngine;

public interface ICoordinateConverter
{
    Vector2 ConvertCoordinateToRectPosition(Vector2 coordinate);
    Vector2 ConvertRectPositionToCoordinate(Vector2 rectPosition);
}

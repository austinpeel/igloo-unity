using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICoordinateConverter
{
    Vector2 ConvertCoordinateToRectPosition(Vector2 coordinate);
}

using UnityEngine;

[CreateAssetMenu(fileName = "Ellipse Parameters", menuName = "Ellipse Parameters", order = 51)]
public class EllipseParameters : ScriptableObject
{
    public float thickness = 10f;
    public float q = 0.5f;
    public float einsteinRadius = 1f;
    public float angle = 0f;
    public Vector2 centerPosition = Vector2.zero;
}

using UnityEngine;

[CreateAssetMenu(fileName = "Lens Parameters", menuName = "Plane Parameters/Lens Parameters", order = 51)]
public class LensParameters : ScriptableObject
{
    public float q = 0.5f;
    public float einsteinRadius = 1f;
    public float angle = 0f;
    public Vector2 centerPosition = Vector2.zero;
}

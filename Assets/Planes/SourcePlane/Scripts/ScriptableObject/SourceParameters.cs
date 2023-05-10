using UnityEngine;

[CreateAssetMenu(fileName = "Source Parameters", menuName = "Plane Parameters/Source Parameters", order = 51)]
public class SourceParameters : ScriptableObject
{
    public float amplitude = 1f;
    public float sersicIndex = 1f;
    public float q = 0.5f;
    public float halfLightRadius = 1f;
    public float angle = 0f;
    public Vector2 centerPosition = Vector2.zero;
}

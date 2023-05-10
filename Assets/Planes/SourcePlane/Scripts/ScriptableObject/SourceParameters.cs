using UnityEngine;

[CreateAssetMenu(fileName = "Source Parameters", menuName = "Plane Parameters/Source Parameters", order = 51)]
public class SourceParameters : ScriptableObject
{
    public delegate void SourceParametersChanged();
    public event SourceParametersChanged OnSourceParametersChanged;

    private float _amplitude = 1f;
    public float amplitude
    {
        get { return _amplitude; }
        set
        {
            if (value != _amplitude)
            {
                _amplitude = value;
                OnSourceParametersChanged?.Invoke();
            }
        }
    }
    private float _sersicIndex = 1f;
    public float sersicIndex
    {
        get { return _sersicIndex; }
        set
        {
            if (value != _sersicIndex)
            {
                _sersicIndex = value;
                OnSourceParametersChanged?.Invoke();
            }
        }
    }
    private float _q = 0.5f;
    public float q
    {
        get { return _q; }
        set
        {
            if (value != _q)
            {
                _q = value;
                OnSourceParametersChanged?.Invoke();
            }
        }
    }
    private float _halfLightRadius = 1f;
    public float halfLightRadius
    {
        get { return _halfLightRadius; }
        set
        {
            if (value != _halfLightRadius)
            {
                _halfLightRadius = value;
                OnSourceParametersChanged?.Invoke();
            }
        }
    }
    private float _angle = 0f;
    public float angle
    {
        get { return _angle; }
        set
        {
            if (value != _angle)
            {
                _angle = value;
                OnSourceParametersChanged?.Invoke();
            }
        }
    }
    private Vector2 _centerPosition = Vector2.zero;
    public Vector2 centerPosition
    {
        get { return _centerPosition; }
        set
        {
            if (value != _centerPosition)
            {
                _centerPosition = value;
                OnSourceParametersChanged?.Invoke();
            }
        }
    }
}

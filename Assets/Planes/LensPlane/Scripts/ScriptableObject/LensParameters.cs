using UnityEngine;

[CreateAssetMenu(fileName = "Lens Parameters", menuName = "Plane Parameters/Lens Parameters", order = 51)]
public class LensParameters : ScriptableObject
{
    public delegate void LensParametersChanged();
    public event LensParametersChanged OnLensParametersChanged;
    
    private float _q = 0.5f;
    public float q
    {
        get { return _q; }
        set
        {
            if (value != _q)
            {
                _q = value;
                OnLensParametersChanged?.Invoke();
            }
        }
    }
    private float _einsteinRadius = 1f;
    public float einsteinRadius
    {
        get { return _einsteinRadius; }
        set
        {
            if (value != _einsteinRadius)
            {
                _einsteinRadius = value;
                OnLensParametersChanged?.Invoke();
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
                OnLensParametersChanged?.Invoke();
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
                OnLensParametersChanged?.Invoke();
            }
        }
    }
}

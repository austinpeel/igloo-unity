using UnityEngine;

[CreateAssetMenu(fileName = "Lens Parameters", menuName = "Plane Parameters/Lens Parameters", order = 0)]
public class LensParameters : ScriptableObject
{
    // public delegate void LensParametersChanged();
    // public event LensParametersChanged OnLensParametersChanged;
    public static event System.Action OnLensParametersChanged;

    public void ApplyParameters()
    {
        OnLensParametersChanged?.Invoke();
    }

    [SerializeField] private float _xCoordinateMax = 4f;
    public float xCoordinateMax
    {
        get { return _xCoordinateMax; }
        set
        {
            if (value != _xCoordinateMax)
            {
                _xCoordinateMax = value;
                OnLensParametersChanged?.Invoke();
            }
        }
    }

    [SerializeField] private float _yCoordinateMax = 4f;
    public float yCoordinateMax
    {
        get { return _yCoordinateMax; }
        set
        {
            if (value != _yCoordinateMax)
            {
                _yCoordinateMax = value;
                OnLensParametersChanged?.Invoke();
            }
        }
    }

    [SerializeField] private float _q = 0.5f;
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

    [SerializeField] private float _einsteinRadius = 1f;
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

    [SerializeField] private float _angle = 0f;
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

    [SerializeField] private Vector2 _centerPosition = Vector2.zero;
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

using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Source Light Map", menuName = "Plane Maps/Source Light Map", order = 51)]
public class SourceLightMap : ScriptableObject
{
    public delegate void SourceLightMapChanged();
    public event SourceLightMapChanged OnSourceLightMapChanged;
    
    private Sprite _sourceLightMap;
    public Sprite sourceLightMap
    {
        get { return _sourceLightMap; }
        set
        {
            if (value != _sourceLightMap)
            {
                _sourceLightMap = value;
                OnSourceLightMapChanged?.Invoke();
            }
        }
    }
}

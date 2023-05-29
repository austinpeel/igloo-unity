using UnityEngine;

[CreateAssetMenu(fileName = "Texture Residuals", menuName = "Plane Parameters/Texture Residuals", order = 51)]
public class TextureResiduals : ScriptableObject
{
    public delegate void TextureResidualsChanged();
    public event TextureResidualsChanged OnTextureResidualsChanged;

    [SerializeField] private Texture2D _texture;
    public Texture2D texture
    {
        get { return _texture; }
        set
        {
            if (value != _texture)
            {
                _texture = value;
                OnTextureResidualsChanged?.Invoke();
            }
        }
    }
}
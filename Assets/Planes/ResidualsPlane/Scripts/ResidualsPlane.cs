using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResidualsPlane : Plane
{
    [SerializeField] private Image residualsMapImage;

    [Header("Scriptable Object")]
    [SerializeField] private TextureResiduals computedTexture;
    [SerializeField] private TextureResiduals testTexture;

    protected void Start() 
    {
        computedTexture.OnTextureResidualsChanged += UpdateResidualsMap;
        testTexture.OnTextureResidualsChanged += UpdateResidualsMap;

        UpdateResidualsMap();
    }

    private void OnValidate() {
        UpdateResidualsMap();
    }

    protected void OnDestroy() 
    {
        computedTexture.OnTextureResidualsChanged -= UpdateResidualsMap;
        testTexture.OnTextureResidualsChanged -= UpdateResidualsMap;
    }

    public void UpdateResidualsMap()
    {
        int widthInt = testTexture.texture.width;
        int heightInt = testTexture.texture.height;

        var testText = testTexture.texture.GetRawTextureData<Color32>();
        var computeText = computedTexture.texture.GetRawTextureData<Color32>();

        Texture2D texture = new Texture2D(widthInt, heightInt);
        Color[] colorsArray = new Color[widthInt * heightInt];

        for (int y = 0; y < heightInt; y++)
        {
            for (int x = 0; x < widthInt; x++)
            {
                float brightnessDiffR = testText[y * widthInt + x].r - computeText[y * widthInt + x].r;
                float brightnessDiffG = testText[y * widthInt + x].g - computeText[y * widthInt + x].g;
                float brightnessDiffB = testText[y * widthInt + x].b - computeText[y * widthInt + x].b;

                if (y >= heightInt/2 && y <= heightInt/2)
                {
                    /*
                    Debug.Log("brightnessDiffR : "+brightnessDiffR + "y : "+y);
                    Debug.Log("brightnessDiffG : "+brightnessDiffG + "y : "+y);
                    Debug.Log("brightnessDiffB : "+brightnessDiffB + "y : "+y);
                    Debug.Log("brightnessR test : "+testText[y * widthInt + x].r + " x : "+x+ " y : "+y);
                    Debug.Log("brightnessG test : "+testText[y * widthInt + x].g + " x : "+x+ " y : "+y);
                    Debug.Log("brightnessB test : "+testText[y * widthInt + x].b + " x : "+x+ " y : "+y);

                    Debug.Log("brightnessR computed : "+computeText[y * widthInt + x].r + " x : "+x+ " y : "+y);
                    Debug.Log("brightnessG computed : "+computeText[y * widthInt + x].g + " x : "+x+ " y : "+y);
                    Debug.Log("brightnessB computed : "+computeText[y * widthInt + x].b + " x : "+x+ " y : "+y);
                    */
                }
                colorsArray[y * widthInt + x] = new Color(brightnessDiffR/255f, brightnessDiffG/255f, brightnessDiffB/255f);
            }
        }

        texture.SetPixels(colorsArray);
        texture.Apply();

        residualsMapImage.sprite = Sprite.Create(texture, new Rect(0, 0, widthInt, heightInt), Vector2.one * 0.5f);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

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

    public void ExportResidualsImage(string imageName = "default.png")
    {
        if (!residualsMapImage || !residualsMapImage.sprite) return;

        Texture2D texture = residualsMapImage.sprite.texture;

        byte[] png = texture.EncodeToPNG();
        #if UNITY_EDITOR_WIN
        File.WriteAllBytes(@"Assets\"+imageName, png);
        AssetDatabase.Refresh();
        #endif

        #if UNITY_EDITOR_OSX
        File.WriteAllBytes(@"Assets/"+imageName, png);
        AssetDatabase.Refresh();
        #endif
    }

    public void UpdateResidualsMap()
    {
        if (!computedTexture.texture || !testTexture.texture) return;

        int widthInt = testTexture.texture.width;
        int heightInt = testTexture.texture.height;

        Color32[] testText = testTexture.texture.GetPixels32();
        Color32[] computeText = computedTexture.texture.GetPixels32();

        Texture2D texture = new Texture2D(widthInt, heightInt);
        Color[] colorsArray = new Color[widthInt * heightInt];

        for (int y = 0; y < heightInt; y++)
        {
            for (int x = 0; x < widthInt; x++)
            {
                //colorsArray[y * widthInt + x] = ComputeAbsoluteDifferenceColor(testText[y * widthInt + x], computeText[y * widthInt + x]);
                colorsArray[y * widthInt + x] = ComputeAbsoluteRatioDifferenceColor(testText[y * widthInt + x], computeText[y * widthInt + x], 10f);
            }
        }

        texture.SetPixels(colorsArray);
        texture.Apply();

        residualsMapImage.sprite = Sprite.Create(texture, new Rect(0, 0, widthInt, heightInt), Vector2.one * 0.5f);
        /* DEBUG PURPOSE
        byte[] png = texture.EncodeToPNG();
        File.WriteAllBytes("residuals_source.png", png);
        */

    }

    private Color ComputeAbsoluteDifferenceColor(Color32 testedTexture, Color32 computedTexture)
    {
        float brightnessDiffR = Mathf.Abs(testedTexture.r - computedTexture.r);
        float brightnessDiffG = Mathf.Abs(testedTexture.g - computedTexture.g);
        float brightnessDiffB = Mathf.Abs(testedTexture.b - computedTexture.b);

        return new Color(brightnessDiffR/255f, brightnessDiffG/255f, brightnessDiffB/255f);
    }

    private Color ComputeAbsoluteRatioDifferenceColor(Color32 testedTexture, Color32 computedTexture, float scaleFactor)
    {
        float brightnessDiffR = Mathf.Abs(testedTexture.r - computedTexture.r) / (float) testedTexture.r;
        float brightnessDiffG = Mathf.Abs(testedTexture.g - computedTexture.g) / (float) testedTexture.g;
        float brightnessDiffB = Mathf.Abs(testedTexture.b - computedTexture.b) / (float) testedTexture.b;

        return new Color(brightnessDiffR * scaleFactor, brightnessDiffG * scaleFactor, brightnessDiffB * scaleFactor);
    }
}

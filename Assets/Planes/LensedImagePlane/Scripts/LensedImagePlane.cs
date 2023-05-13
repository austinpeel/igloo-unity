using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LensedImagePlane : Plane
{
    [Header("Scriptable Object")]
    [SerializeField] private LensParameters lensParameters;
    [SerializeField] private SourceParameters sourceParameters;

    [Header("Source Map")]
    [SerializeField] private Image sourceLightMapImage;
    [SerializeField] private Color colorLightMap = Color.red;

    // Parameters of the lens
    private float xLensCoordinateMax = 0f;
    private float yLensCoordinateMax = 0f;
    private float lensQ = 0.5f;
    private float lensEinsteinRadius = 1f;
    private float lensAngle = 0f;
    private Vector2 lensCenterPosition = Vector2.zero;

    // Parameters of the source
    private float xSourceCoordinateMax = 0f;
    private float ySourceCoordinateMax = 0f;
    private float sourceAmplitude = 1f;
    private float sourceSersicIndex = 1f;
    private float sourceQ = 0.5f;
    private float sourceHalfLightRadius = 1f;
    private float sourceAngle = 0f;
    private Vector2 sourceCenterPosition = Vector2.zero;


    protected void Start() 
    {
        sourceParameters.OnSourceParametersChanged += UpdateSourceParameters;
        lensParameters.OnLensParametersChanged += UpdateLensParameters;
    }

    protected void OnDestroy() 
    {
        sourceParameters.OnSourceParametersChanged -= UpdateSourceParameters;
        lensParameters.OnLensParametersChanged -= UpdateLensParameters;
    }

    public void UpdateSourceLightMap()
    {
        if (!sourceLightMapImage || xSourceCoordinateMax == 0f || ySourceCoordinateMax == 0f) return;
        
        int widthInt = ((int) width);
        int heightInt = ((int) height);

        float xRange = GetXCoordinateMax() * 2f;
        float yRange = GetYCoordinateMax() * 2f;

        Texture2D texture = new Texture2D(widthInt, heightInt);

        Color[] colorsArray = new Color[widthInt * heightInt];

        for (int y = 0; y < heightInt; y++)
        {
            for (int x = 0; x < widthInt; x++)
            {
                colorsArray[y * widthInt + x] = Color.clear;
            }
        }

        for (int y = 0; y < heightInt; y++)
        {
            for (int x = 0; x < widthInt; x++)
            {
                float convertedX = (-GetXCoordinateMax() + x * (xRange / widthInt)) - sourceCenterPosition.x;
                float convertedY = (-GetYCoordinateMax() + y * (yRange / heightInt)) - sourceCenterPosition.y;

                colorsArray[y * widthInt + x] = new Color(colorLightMap.r , colorLightMap.g, colorLightMap.b, BrightnessSERSIC(convertedX,convertedY, true));
            }
        }

        texture.SetPixels(colorsArray);
        texture.Apply();

        sourceLightMapImage.sprite = Sprite.Create(texture, new Rect(0, 0, widthInt, heightInt), Vector2.one * 0.5f);
    }

/*
    private Vector2 ComputeSourceWidthAndHeight(float xSourceCoordinateMax, float ySourceCoordinateMax)
    {
        if (GetXCoordinateMax() <= 0f || GetYCoordinateMax() <= 0f) return Vector2.zero;

        float sourceWidth = (rectTransform.rect.width / GetXCoordinateMax()) * xSourceCoordinateMax;
        float sourceHeight = (rectTransform.rect.height / GetYCoordinateMax()) * ySourceCoordinateMax;

        return new Vector2(sourceWidth, sourceHeight);
    }
*/

    // Compute the brightness of the object with the SERSIC profile
    public float BrightnessSERSIC(float x, float y, bool log10 = false)
    {
        return Profiles.BrightnessSersic(x, y, sourceAmplitude, sourceSersicIndex, sourceHalfLightRadius, sourceQ, sourceAngle, log10);
    }

    public void UpdateLensParameters()
    {
        if (!sourceLightMapImage ||!lensParameters) return;

        if (GetXCoordinateMax() != lensParameters.xCoordinateMax) SetXCoordinateMax(lensParameters.xCoordinateMax, true);
        if (GetYCoordinateMax() != lensParameters.yCoordinateMax) SetYCoordinateMax(lensParameters.yCoordinateMax, true);

        Material mat = sourceLightMapImage.material;

        // mat.SetTexture("_MainTex", croppedTexture);
        mat.SetFloat("_Q", lensParameters.q);
        mat.SetFloat("_ThetaE", lensParameters.einsteinRadius);
        mat.SetFloat("_Angle", lensParameters.angle);
        mat.SetFloat("_X0", lensParameters.centerPosition.x);
        mat.SetFloat("_Y0", lensParameters.centerPosition.y);

        sourceLightMapImage.material = mat;
        // DEBUG
        /*
        Debug.Log("lens parameters q : "+lensParameters.q);
        Debug.Log("lens parameters einsteinRadius : "+lensParameters.einsteinRadius);
        Debug.Log("lens parameters angle : "+lensParameters.angle);
        Debug.Log("lens parameters centerPosition : "+lensParameters.centerPosition);
        */
    }

    public void UpdateSourceParameters()
    {
        xSourceCoordinateMax = sourceParameters.xCoordinateMax;
        ySourceCoordinateMax = sourceParameters.yCoordinateMax;
        sourceAmplitude = sourceParameters.amplitude;
        sourceSersicIndex = sourceParameters.sersicIndex;
        sourceQ = sourceParameters.q;
        sourceHalfLightRadius = sourceParameters.halfLightRadius;
        sourceAngle = sourceParameters.angle;
        sourceCenterPosition = sourceParameters.centerPosition;

        UpdateSourceLightMap();
        // DEBUG
        /*
        Debug.Log("source parameters amp : "+sourceParameters.amplitude);
        Debug.Log("source parameters sersicIndex : "+sourceParameters.sersicIndex);
        Debug.Log("source parameters q : "+sourceParameters.q);
        Debug.Log("source parameters halfLightRadius : "+sourceParameters.halfLightRadius);
        Debug.Log("source parameters angle : "+sourceParameters.angle);
        Debug.Log("source parameters centerPosition : "+sourceParameters.centerPosition);
        */
    }


    // --------------------- USED IN LENS IMAGE PLANE EDITOR ---------------------
    public void SetLensParameters(LensParameters newLensParameters)
    {
        lensParameters = newLensParameters;
    }

    public LensParameters GetLensParameters()
    {
        return lensParameters;
    }

    public void SetSourceParameters(SourceParameters newSourceParameters)
    {
        sourceParameters = newSourceParameters;
    }

    public SourceParameters GetSourceParameters()
    {
        return sourceParameters;
    }
}

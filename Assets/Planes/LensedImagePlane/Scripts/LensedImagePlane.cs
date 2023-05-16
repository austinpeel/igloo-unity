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
    private float lensQ;
    private float lensEinsteinRadius;
    private float lensAngle;
    private Vector2 lensCenterPosition;

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

        UpdateAllParameters();
    }
    protected void OnDestroy() 
    {
        sourceParameters.OnSourceParametersChanged -= UpdateSourceParameters;
        lensParameters.OnLensParametersChanged -= UpdateLensParameters;
    }

    public void UpdateAllParameters()
    {
        UpdateLensParameters();
        UpdateSourceParameters();
    }

    public void UpdateSourceLightMap()
    {
        if (!sourceLightMapImage) return;
        
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
                float convertedX = (-GetXCoordinateMax() + x * (xRange / widthInt)) - sourceCenterPosition.x;
                float convertedY = (-GetYCoordinateMax() + y * (yRange / heightInt)) - sourceCenterPosition.y;

                colorsArray[y * widthInt + x] = new Color(colorLightMap.r , colorLightMap.g, colorLightMap.b, BrightnessSERSIC(convertedX,convertedY, true));
            }
        }

        texture.SetPixels(colorsArray);
        texture.Apply();

        sourceLightMapImage.material.SetTexture("_MainTex", texture);
        sourceLightMapImage.SetMaterialDirty();
    }

    // Compute the brightness of the object with the SERSIC profile
    public float BrightnessSERSIC(float x, float y, bool log10 = false)
    {
        return Profiles.BrightnessSersic(x, y, sourceAmplitude, sourceSersicIndex, sourceHalfLightRadius, sourceQ, sourceAngle, log10);
    }

    public void UpdateLensParameters()
    {
        if (!sourceLightMapImage ||!lensParameters) return;

        bool hasXCoordChanged = false;
        bool hasYCoordChanged = false;

        if (GetXCoordinateMax() != lensParameters.xCoordinateMax)
        {
            SetXCoordinateMax(lensParameters.xCoordinateMax, true);
            hasXCoordChanged = true;
        }
        if (GetYCoordinateMax() != lensParameters.yCoordinateMax)
        {
            SetYCoordinateMax(lensParameters.yCoordinateMax, true);
            hasYCoordChanged = true;
        }

        Material mat = sourceLightMapImage.material;

        if (lensQ != lensParameters.q)
        {
            lensQ = lensParameters.q;
            mat.SetFloat("_Q", lensQ);
        }
        if (lensEinsteinRadius != lensParameters.einsteinRadius || hasXCoordChanged)
        {
            lensEinsteinRadius = lensParameters.einsteinRadius;

            // Convert in UV
            float einsteinRadiusUV = lensEinsteinRadius / GetXCoordinateMax();
            mat.SetFloat("_ThetaE", einsteinRadiusUV);
        }
        if (lensAngle != lensParameters.angle)
        {
            lensAngle = lensParameters.angle;

            // Convert in radians
            float radAngle = Mathf.Deg2Rad * lensAngle;
            mat.SetFloat("_Angle", radAngle);
        }
        if (lensCenterPosition != lensParameters.centerPosition || hasXCoordChanged || hasYCoordChanged)
        {
            lensCenterPosition = lensParameters.centerPosition;

            // Convert in UV
            Vector2 centerPositionUV = new Vector2(lensCenterPosition.x / GetXCoordinateMax(), lensCenterPosition.y / GetYCoordinateMax());
            mat.SetVector("_CenterPosition", centerPositionUV);
        }

        sourceLightMapImage.material = mat;

        UpdateSourceLightMap();
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

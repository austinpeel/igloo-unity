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

    [Header("Lens Light Map")]
    [SerializeField] private Image lensLightMapImage;
    [SerializeField] private Color colorLensLightMap = Color.red;

    [Header("Materials Lensed Image")]
    [SerializeField] private Material lightMapSIE;
    [SerializeField] private Material lightMapSIS;

    // Parameters of the lens
    private float lensQ;
    private float lensEinsteinRadius;
    private float lensAngle;
    private Vector2 lensCenterPosition;

    // Parameters of the lens light
    private float lensAmplitude = 1f;
    private float lensSersicIndex = 1f;

    // Parameters of the source
    private float xSourceCoordinateMax = 0f;
    private float ySourceCoordinateMax = 0f;
    private float sourceAmplitude = 1f;
    private float sourceSersicIndex = 1f;
    private float sourceQ = 0.5f;
    private float sourceHalfLightRadius = 1f;
    private float sourceAngle = 0f;
    private Vector2 sourceCenterPosition = Vector2.zero;

    // Parameters of the materials of the lens
    private bool lensUsesSIE = true;

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

                colorsArray[y * widthInt + x] = new Color(colorLightMap.r , colorLightMap.g, colorLightMap.b, SourceBrightnessSERSIC(convertedX,convertedY, true));
            }
        }

        texture.SetPixels(colorsArray);
        texture.Apply();

        sourceLightMapImage.material.SetTexture("_MainTex", texture);
        sourceLightMapImage.SetMaterialDirty();
    }

    // Compute the brightness of the source with the SERSIC profile
    public float SourceBrightnessSERSIC(float x, float y, bool log10 = false)
    {
        return Profiles.BrightnessSersic(x, y, sourceAmplitude, sourceSersicIndex, sourceHalfLightRadius, sourceQ, sourceAngle, log10);
    }

    // Compute the brightness of the lens with the SERSIC profile
    public float LensBrightnessSERSIC(float x, float y, bool log10 = false)
    {
        return Profiles.BrightnessSersic(x, y, 1f, 1f, lensEinsteinRadius/2f, lensQ, lensAngle, log10);
    }

    public void UpdateLensParameters()
    {
        if (!sourceLightMapImage ||!lensParameters) return;

        Material mat;

        // SIS Part
        if (lensParameters.q == 1f)
        {
            mat = lightMapSIS;

            if (!lensUsesSIE)
            {
                UpdateLensMaterialParameters(mat);
            }
            else
            {
                ForceUpdateLensMaterialParameters(mat);
                lensUsesSIE = false;
            }
        }
        // SIE Part
        else
        {
            mat = lightMapSIE;

            if (lensUsesSIE)
            {
                UpdateLensMaterialParameters(mat);
            }
            else 
            {
                ForceUpdateLensMaterialParameters(mat);
                lensUsesSIE = true;
            } 
        }

        sourceLightMapImage.material = mat;

        UpdateSourceLightMap();

        UpdateLensLightMap();
    }

    private void UpdateLensMaterialParameters(Material material)
    {
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
        if (lensQ != lensParameters.q)
        {
            lensQ = lensParameters.q;
            material.SetFloat("_Q", lensQ);
        }
        if (lensEinsteinRadius != lensParameters.einsteinRadius || hasXCoordChanged)
        {
            lensEinsteinRadius = lensParameters.einsteinRadius;

            // Convert in UV
            float einsteinRadiusUV = lensEinsteinRadius / GetXCoordinateMax();
            material.SetFloat("_ThetaE", einsteinRadiusUV);
        }
        if (lensAngle != lensParameters.angle)
        {
            lensAngle = lensParameters.angle;

            // Convert in radians
            float radAngle = Mathf.Deg2Rad * lensAngle;
            material.SetFloat("_Angle", radAngle);
        }
        if (lensCenterPosition != lensParameters.centerPosition || hasXCoordChanged || hasYCoordChanged)
        {
            lensCenterPosition = lensParameters.centerPosition;

            // Convert in UV
            Vector2 centerPositionUV = new Vector2(lensCenterPosition.x / GetXCoordinateMax(), lensCenterPosition.y / GetYCoordinateMax());
            material.SetVector("_CenterPosition", centerPositionUV);
        }
    }

    private void ForceUpdateLensMaterialParameters(Material material)
    {
        // Force the update of every parameters related to the lensing material
        SetXCoordinateMax(lensParameters.xCoordinateMax, true);
        SetYCoordinateMax(lensParameters.yCoordinateMax, true);

        lensQ = lensParameters.q;
        material.SetFloat("_Q", lensQ);

        lensEinsteinRadius = lensParameters.einsteinRadius;
        // Convert in UV
        material.SetFloat("_ThetaE", lensEinsteinRadius / GetXCoordinateMax());

        lensAngle = lensParameters.angle;
        // Convert in radians
        material.SetFloat("_Angle", Mathf.Deg2Rad * lensAngle);

        lensCenterPosition = lensParameters.centerPosition;
        // Convert in UV
        Vector2 forcedCenterPositionUV = new Vector2(lensCenterPosition.x / GetXCoordinateMax(), lensCenterPosition.y / GetYCoordinateMax());
        material.SetVector("_CenterPosition", forcedCenterPositionUV);
    }

    private void UpdateLensLightMap()
    {
        if (!lensLightMapImage) return;

        Material mat = lensLightMapImage.material;

        float xRange = GetXCoordinateMax() * 2f;
        float yRange = GetYCoordinateMax() * 2f;

        mat.SetVector("_AxisRange", new Vector2(xRange, yRange));
        mat.SetFloat("_Amplitude", lensAmplitude);
        mat.SetFloat("_SersicIndex", lensSersicIndex);
        mat.SetFloat("_Q", lensQ);
        mat.SetFloat("_ThetaEff", lensEinsteinRadius);

        // Convert in radians
        float radAngle = Mathf.Deg2Rad * (lensAngle + 90f);
        mat.SetFloat("_Angle", radAngle);

        // Convert in UV
        Vector2 centerPositionUV = new Vector2(lensCenterPosition.x / xRange, lensCenterPosition.y / yRange);
        mat.SetVector("_CenterPosition", centerPositionUV);

        mat.SetColor("_Color", colorLensLightMap);

        lensLightMapImage.material = mat;
        lensLightMapImage.SetMaterialDirty();
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

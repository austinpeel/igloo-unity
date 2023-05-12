using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LensedImagePlane : Plane
{
    [Header("Scriptable Object")]
    [SerializeField] private LensParameters lensParameters;
    [SerializeField] private SourceParameters sourceParameters;
    [SerializeField] private SourceLightMap sourceLightMap;

    [Header("Source Map")]
    [SerializeField] private Image sourceLightMapImage;
/*
    // Parameters of the lens
    private float lensQ = 0.5f;
    private float lensEinsteinRadius = 1f;
    private float lensAngle = 0f;
    private Vector2 lensCenterPosition = Vector2.zero;

    // Parameters of the source
    private float sourceAmplitude = 1f;
    private float sourceSersicIndex = 1f;
    private float sourceQ = 0.5f;
    private float sourceEinsteinRadius = 1f;
    private float sourceAngle = 0f;
    private Vector2 sourceCenterPosition = Vector2.zero;
*/

    protected void Start() 
    {
        sourceParameters.OnSourceParametersChanged += UpdateSourceParameters;
        lensParameters.OnLensParametersChanged += UpdateLensParameters;
        sourceLightMap.OnSourceLightMapChanged += UpdateSourceLightMap;
    }

    protected void OnDestroy() 
    {
        sourceParameters.OnSourceParametersChanged -= UpdateSourceParameters;
        lensParameters.OnLensParametersChanged -= UpdateLensParameters;
        sourceLightMap.OnSourceLightMapChanged -= UpdateSourceLightMap;
    }

    public void UpdateSourceLightMap()
    {
        if (!sourceLightMapImage) return;

        sourceLightMapImage.sprite = sourceLightMap.sourceLightMap;
    }

    public void UpdateLensParameters()
    {
        if (!sourceLightMapImage ||!lensParameters) return;

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

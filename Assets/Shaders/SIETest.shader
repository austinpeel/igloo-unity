Shader "Lensing/SIETest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LensQ ("Lens Q", Float) = 0
        _LensThetaE ("Lens ThetaE", Float) = 0
        _LensAngle ("Lens Angle", Float) = 0
        _LensCenterPosition ("Lens Center Position", Vector) = (0.0, 0.0, 0, 0)
        _SourceAxisRange ("Source Axis Range", Vector) = (4.0 , 4.0, 0, 0) // The range of each axis in arcsec
        _SourceAmplitude ("Source Amplitude", Float) = 0
        _SourceSersicIndex ("Source Sersic Index", Float) = 0
        _SourceQ ("Source Q", Range(0.0, 1.0)) = 0
        _SourceThetaEff ("Source ThetaEff", Float) = 0
        _SourceAngle ("Source Angle", Float) = 0
        _SourceCenterPosition ("Source Center Position", Vector) = (0.0, 0.0, 0, 0)
        _SourceColor ("Source Color", Color) = (0.0, 0.0, 0.0, 0.0)
        _SourceIsLog10 ("Source Is Log10", Integer) = 0 // Used as a boolean : 0 == false and 1 == true
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        LOD 100

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_TexelSize;
            float _LensQ;
            float _LensThetaE;
            float _LensAngle;
            float2 _LensCenterPosition;
            float2 _SourceAxisRange;
            float _SourceAmplitude;
            float _SourceSersicIndex;
            float _SourceQ;
            float _SourceThetaEff;
            float _SourceAngle;
            float2 _SourceCenterPosition;
            float4 _SourceColor;
            int _SourceIsLog10;
        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        struct VertexInput
        {
            float4 position : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct VertexOutput
        {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        ENDHLSL

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float bnCoolest(float n)
            {
                return 1.9992f*n - 0.3271f;
            }

            float bnWiki(float n)
            {
                float sersicIndexTwo = n * n;

                float bn = 2.0*n - 1.0f/3.0f + 4.0f/(405.0f*n) + 46.0f/(25515.0f*sersicIndexTwo) + 131.0f/(1148175.0f*sersicIndexTwo*n) - 2194697.0f/(30690717750.0f*sersicIndexTwo*sersicIndexTwo);
                return bn;
            }

            float2 rotate(float2 pos, float angle)
            {
                float newX =  pos.x * cos(angle) + pos.y * sin(angle);
                float newY = -pos.x * sin(angle) + pos.y * cos(angle);
                return float2(newX, newY);
            }

            VertexOutput vert(VertexInput i)
            {
                VertexOutput o;
                o.position = TransformObjectToHClip(i.position.xyz);
                o.uv = i.uv;
                return o;
            }

            float4 frag(VertexOutput i) : SV_Target
            {
                // _ThetaEff should be in coordinate (arcsec)
                // If the half light radius is equals to 0, then return no color
                if (_SourceThetaEff == 0.0f) return float4(0.0f, 0.0f, 0.0f, 0.0f);

                // Position in arcsec from the centerPosition
                float2 pos = (i.uv - 0.5 - _SourceCenterPosition) * _SourceAxisRange;
                
                float2 rotatedPos = rotate(pos, _SourceAngle);
                float bn = bnCoolest(_SourceSersicIndex);//bnWiki(_SersicIndex);

                float ratio = pow(sqrt((_SourceQ * (rotatedPos.x * rotatedPos.x) + (rotatedPos.y * rotatedPos.y) / _SourceQ)) / _SourceThetaEff, 1.0f/_SourceSersicIndex);
                float result = _SourceAmplitude * exp(-bn * (ratio - 1.0f));

                if (_SourceIsLog10 == 1)
                {
                    return float4(_SourceColor.xyz, log10(result));
                }

                return float4(_SourceColor.xyz, result);
            }

            ENDHLSL
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            Tags
            {
                // Specify LightMode correctly.
                "LightMode" = "UseColorTexture"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _GrabbedTexture;

            float atanh(float x)
            {
                float result = 0;
                if (abs(x) < 1)
                {
                    result = 0.5 * (log(1 + x) - log(1 - x));
                }
                return result;
                // return 0.5 * (log(1 + x) - log(1 - x));
            }

            float2 rotate(float2 pos, float angle)
            {
                float newX =  pos.x * cos(angle) + pos.y * sin(angle);
                float newY = -pos.x * sin(angle) + pos.y * cos(angle);
                return float2(newX, newY);
            }

            VertexOutput vert(VertexInput i)
            {
                VertexOutput o;
                o.position = TransformObjectToHClip(i.position.xyz);
                o.uv = i.uv;
                return o;
            }

            float4 frag(VertexOutput i) : SV_Target
            {
                // Convert ThetaE (which is in UV (between 0 and 1)) to pixel space
                float thetaEpix = _LensThetaE / _MainTex_TexelSize.y;

                // Parameter conversions
                float q2 = pow(_LensQ, 2);
                float thetaEeff = thetaEpix / sqrt((1 + q2) / (2 * _LensQ));
                float b = thetaEeff * sqrt(0.5 * (1 + q2));
                float s = 0.001 * sqrt((1 + q2) / (2 * q2));

                // Texture space (pixel) position
                float2 pos = (i.uv - 0.5 - _LensCenterPosition) / _MainTex_TexelSize.xy;
                
                // Rotate
                pos = rotate(pos, _LensAngle);
                
                // Compute deflection angle in pixel space
                float psi = sqrt(q2 * (pow(s, 2) + pow(pos.x, 2)) + pow(pos.y, 2));
                float p = sqrt(1 - q2);
                float alphaX = b / p * atan2(p * pos.x, psi + s);
                float alphaY = b / p * atanh(p * pos.y / (psi + q2 * s));

                // Rotate back
                float2 alpha = rotate(float2(alphaX, alphaY), -_LensAngle);
                
                // Convert deflection angle back to normalized UV space
				//float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv - alpha * _MainTex_TexelSize.xy);
                float4 color = float4(0.0, 0.0, 0.0, 0.0);//tex2D(_GrabbedTexture,  i.uv - alpha * _MainTex_TexelSize.xy);
				return color;
            }

            ENDHLSL
        }
    }
}
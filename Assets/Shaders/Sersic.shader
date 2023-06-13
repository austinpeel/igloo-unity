Shader "Lensing/Sersic"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AxisRange ("Axis Range", Vector) = (4.0 , 4.0, 0, 0) // The range of each axis in arcsec
        _Amplitude ("Amplitude", Float) = 0
        _SersicIndex ("Sersic Index", Float) = 0
        _Q ("Q", Range(0.0, 1.0)) = 0
        _ThetaEff ("ThetaEff", Float) = 0
        _Angle ("Angle", Float) = 0
        _CenterPosition ("Center Position", Vector) = (0.0, 0.0, 0, 0)
        _Color ("Color", Color) = (0.0, 0.0, 0.0, 0.0)
        _IsLog10 ("Is Log10", Integer) = 0 // Used as a boolean : 0 == false and 1 == true
        _IsScaled ("Is Scaled", Integer) = 0 // Used as a boolean : 0 == false and 1 == true
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        LOD 100

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float2 _AxisRange;
            float _Amplitude;
            float _SersicIndex;
            float _Q;
            float _ThetaEff;
            float _Angle;
            float2 _CenterPosition;
            float4 _Color;
            int _IsLog10;
            int _IsScaled;
        CBUFFER_END

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

            float sersicBrightness(float2 position, float angle, float amplitude, float sersicIndex, float halfLightRadius, float q)
            {
                float2 rotatedPos = rotate(position, angle);
                float bn = bnCoolest(sersicIndex);//bnWiki(sersicIndex);

                //float ratio = pow(sqrt((q * (rotatedPos.x * rotatedPos.x) + (rotatedPos.y * rotatedPos.y) / q)) / halfLightRadius, 1.0f/sersicIndex);
                float ratio = pow(sqrt(((rotatedPos.x * rotatedPos.x) + (rotatedPos.y * rotatedPos.y) / (q*q))) / halfLightRadius, 1.0f/sersicIndex);
                float result = amplitude * exp(-bn * (ratio - 1.0f));

                return result;
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
                if (_ThetaEff == 0.0f) return float4(0.0f, 0.0f, 0.0f, 0.0f);

                // Position in arcsec from the centerPosition
                float2 pos = (i.uv - 0.5 - _CenterPosition) * _AxisRange;

                float2 maxPos = float2(0.0f, 0.0f);
                float maxSersicValue = sersicBrightness(maxPos, _Angle, _Amplitude, _SersicIndex, _ThetaEff, _Q);

                float sersicValue = sersicBrightness(pos, _Angle, _Amplitude, _SersicIndex, _ThetaEff, _Q);
                float ratioValue = sersicValue / maxSersicValue;

                if (_IsScaled == 1)
                {
                    //return float4(_Color.xyz, log10(result));
                    return float4(ratioValue, ratioValue, ratioValue, 1.0f);
                }

                return float4(_Color.xyz, sersicValue);
                //return float4(ratioValue, ratioValue, ratioValue, 1.0f);
            }

            ENDHLSL
        }
    }
}
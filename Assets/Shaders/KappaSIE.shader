Shader "Lensing/KappaSIE"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AxisRange ("Axis Range", Vector) = (4.0 , 4.0, 0, 0) // The range of each axis in arcsec
        _Q ("Q", Range(0.0, 1.0)) = 0
        _ThetaE ("ThetaE", Float) = 0
        _Angle ("Angle", Float) = 0
        _Color ("Color", Color) = (0.0, 0.0, 0.0, 0.0)
        _CenterPosition ("Center Position", Vector) = (0.0, 0.0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        LOD 100

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_TexelSize;
            float2 _AxisRange;
            float _Q;
            float _ThetaE;
            float _Angle;
            float4 _Color;
            float2 _CenterPosition;
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
                // Position in arcsec from the centerPosition
                float2 pos = (i.uv - 0.5 - _CenterPosition) * _AxisRange;

                if (pos.x == 0.0 && pos.y == 0.0) return float4(_Color.xyz, 1.0);
                
                float2 rotatedPos = rotate(pos, _Angle);

                // einsteinRadius / (2f * Mathf.Sqrt(q * (rotatedX*rotatedX) + (rotatedY*rotatedY) / q))
                float result = _ThetaE / (2.0 * sqrt(_Q * (rotatedPos.x * rotatedPos.x) + (rotatedPos.y*rotatedPos.y) / _Q));
                return float4(_Color.xyz, result);
            }

            ENDHLSL
        }
    }
}
Shader "Lensing/SIE"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Q ("Q", Float) = 0
        _ThetaE ("ThetaE", Float) = 0
        _Angle ("Angle", Float) = 0
        _CenterPosition ("Center Position", Vector) = (0.5, 0.5, 0, 0)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        LOD 100

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_TexelSize;
            float _Q;
            float _ThetaE;
            float _Angle;
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
                float thetaEpix = _ThetaE / _MainTex_TexelSize.y;

                // Parameter conversions
                float q2 = pow(_Q, 2);
                float thetaEeff = thetaEpix / sqrt((1 + q2) / (2 * _Q));
                float b = thetaEeff * sqrt(0.5 * (1 + q2));
                float s = 0.001 * sqrt((1 + q2) / (2 * q2));

                // Texture space (pixel) position
                float2 pos = (i.uv - 0.5 - _CenterPosition) / _MainTex_TexelSize.xy;
                
                // Rotate
                pos = rotate(pos, _Angle);
                
                // Compute deflection angle in pixel space
                float psi = sqrt(q2 * (pow(s, 2) + pow(pos.x, 2)) + pow(pos.y, 2));
                float p = sqrt(1 - q2);
                float alphaX = b / p * atan2(p * pos.x, psi + s);
                float alphaY = b / p * atanh(p * pos.y / (psi + q2 * s));

                // Rotate back
                float2 alpha = rotate(float2(alphaX, alphaY), -_Angle);
                
                // Convert deflection angle back to normalized UV space
				float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv - alpha * _MainTex_TexelSize.xy);
				return color;
            }

            ENDHLSL
        }
    }
}
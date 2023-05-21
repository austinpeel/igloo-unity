Shader "Lensing/SIS"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ThetaE ("ThetaE", Float) = 0
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
            float _ThetaE;
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

            VertexOutput vert(VertexInput i)
            {
                VertexOutput o;
                o.position = TransformObjectToHClip(i.position.xyz);
                o.uv = i.uv;
                return o;
            }

            float4 frag(VertexOutput i) : SV_Target
            {
                // Texture space (pixel) position
                float2 pos = (i.uv - 0.5 - _CenterPosition) / _MainTex_TexelSize.xy;

                float r = sqrt(pow(pos.x, 2.0) + pow(pos.y, 2.0));
                float2 direction = pos / r;

                // Convert ThetaE (which is in UV (between 0 and 1)) to pixel space
                float thetaEpix = _ThetaE / _MainTex_TexelSize.y;

                // Deflection angle back in normalized UV space
                float2 alpha = thetaEpix * direction;

                // Shift pixels keeping colors the same
			    float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv - alpha * _MainTex_TexelSize.xy);
			    return color;
            }

            ENDHLSL
        }
    }
}
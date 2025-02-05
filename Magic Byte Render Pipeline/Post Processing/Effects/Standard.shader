﻿Shader "Hidden/Standard"
{
    Properties
    {
        //_MainTexCamera ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        ZTest Always Cull Off ZWrite Off

        HLSLINCLUDE
        #include "../../ShaderLibrary/Common.hlsl"
        ENDHLSL

        Pass
        {
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex DefaultPassVertex
            #pragma fragment frag

            #include "../../ShaderLibrary/UnityInput.hlsl"

            TEXTURE2D(_PostFXSource);
            SAMPLER(sampler_PostFXSource);
            SAMPLER(sampler_linear_clamp);

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 fxUV : VAR_FX_UV;
            };

            Varyings DefaultPassVertex(uint vertexID : SV_VertexID) {
                Varyings output;
                output.positionCS = float4(
                    vertexID <= 1 ? -1.0 : 3.0,
                    vertexID == 1 ? 3.0 : -1.0,
                    0.0, 1.0
                    );
                output.fxUV = float2(
                    vertexID <= 1 ? 0.0 : 2.0,
                    vertexID == 1 ? 2.0 : 0.0
                    );
                if (_ProjectionParams.x < 0.0) {
                    output.fxUV.y = 1.0 - output.fxUV.y;
                }
                return output;
            }

float4 GetSource(float2 fxUV) {
    return SAMPLE_TEXTURE2D(_PostFXSource, sampler_PostFXSource, fxUV);
}
float4 frag (Varyings i) : SV_Target
{
                float4 color = GetSource(i.fxUV);
                return SAMPLE_TEXTURE2D_LOD(_PostFXSource, sampler_linear_clamp, i.fxUV, 0);
                //return color;
            }
            ENDHLSL
        }
    }

}


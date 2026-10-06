Shader "Hidden/CannoliCat/Stylized/Sharpness" {
    SubShader {
        Pass {
            Cull Off
            Blend Off
            ZTest Always
            ZWrite Off

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment frag
            
            #include "Include/StylizedCommon.hlsl"

            float _Amount;
            
            half4 frag (Varyings i) : SV_Target {
                float2 uv = i.texcoord;
                float neighbor = _Amount * -1;
                float center = _Amount * 4 + 1;

                float4 col = SZ_SamplePoint(uv);
                float4 n = SZ_SamplePoint(uv + _BlitTexture_TexelSize.xy * float2(0, 1));
                float4 e = SZ_SamplePoint(uv + _BlitTexture_TexelSize.xy * float2(1, 0));
                float4 s = SZ_SamplePoint(uv + _BlitTexture_TexelSize.xy * float2(0, -1));
                float4 w = SZ_SamplePoint(uv + _BlitTexture_TexelSize.xy * float2(-1, 0));

                float4 output = n * neighbor + e * neighbor + col * center + s * neighbor + w * neighbor;
                
                return half4(saturate(output.rgb), 1.0);
            }
            
            ENDHLSL
        }
    }
}
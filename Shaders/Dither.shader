Shader "Hidden/CannoliCat/Stylized/Dither"{
    SubShader{
        Pass{
            Cull Off
            Blend Off
            ZTest Always
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Include/StylizedCommon.hlsl"
            #include "Include/StylizedNoise.hlsl"
            #include "Include/StylizedColor.hlsl"

            float _Spread;
            int _RedColorCount, _GreenColorCount, _BlueColorCount, _BayerLevel;
            int _DitherQuantization, _DitherAnchor, _DitherPattern;
            TEXTURE2D(_BlueNoiseTex);
            float4 _BlueNoiseTex_TexelSize;
            
            half4 frag(Varyings i) : SV_Target
            {
                float4 col = SZ_SamplePoint(i.texcoord);

                int2 screenPx = (int2)floor(i.positionCS.xy);
                uint2 px = (uint2)screenPx;
                
                // 0 = Bayer, 1 = BlueNoise (matches DitherPattern in C#)
                float patternSize = _DitherPattern == 1 ? _BlueNoiseTex_TexelSize.z : 16.0;

                // 0 = Screen, 1 = ViewDirection (matches DitherAnchor in C#)
                if (_DitherAnchor == 1)
                {
                    float3 forward = -UNITY_MATRIX_V[2].xyz;
                    float2 angles = float2(atan2(forward.x, forward.z), asin(clamp(forward.y, -1.0, 1.0)));

                    float outputHeight = 1.0 / abs(ddy(i.texcoord.y));
                    float pixelsPerRadian = outputHeight * 0.5 * abs(UNITY_MATRIX_P._m11);
                    
                    float pixelsPerTurn = max(round(TWO_PI * pixelsPerRadian / patternSize), 1.0) * patternSize;
                    pixelsPerRadian = pixelsPerTurn / TWO_PI;

                    int2 offset = (int2)floor(float2(angles.x, angles.y) * pixelsPerRadian);
                    px = (uint2)(screenPx + offset);
                }
                
                float pattern;
                
                // 0 = Bayer, 1 = BlueNoise (matches DitherPattern in C#)
                if (_DitherPattern == 1)
                {
                    uint size = (uint)_BlueNoiseTex_TexelSize.z;
                    pattern = LOAD_TEXTURE2D(_BlueNoiseTex, px % size).r - 0.5;
                }
                else
                {
                    uint bits = (uint)clamp(_BayerLevel, 0, 3) + 1u;
                    pattern = SZ_Bayer(px.x, px.y, bits);
                }
                
                float threshold = _Spread * pattern;

                float3 q;
                
                // 0 = RgbLevels, 1 = None (matches DitherQuantization in C#)
                if (_DitherQuantization == 0)
                {
                    q.r = floor((_RedColorCount - 1.0) * col.r + threshold + 0.5) / (_RedColorCount - 1.0);
                    q.g = floor((_GreenColorCount - 1.0) * col.g + threshold + 0.5) / (_GreenColorCount - 1.0);
                    q.b = floor((_BlueColorCount - 1.0) * col.b + threshold + 0.5) / (_BlueColorCount - 1.0);
                }
                else
                {
                    float3 lab = SZ_LinearToOklab(col.rgb);
                    lab.x += threshold; // L (lightness)
                    q = SZ_OklabToLinear(lab);
                }

                return half4(saturate(q), 1.0);
            }
            ENDHLSL
        }
    }
}

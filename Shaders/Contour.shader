Shader "Hidden/CannoliCat/Stylized/Contour"{
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
            #include "Include/StylizedDepth.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

            float _DepthSensitivity;
            float _NormalSensitivity;
            float _Thickness;
            float _Threshold;
            float4 _PaperColor;
            float4 _LineColor;
            float _PaperMix;
            float _ColorSensitivity;
            float _BandCount;
            float _ContourWidth;
            float _WobbleAmount;
            float _WobbleFrequency;
            float _WidthVariation;
            float _WidthFrequency;
            float _BoilRate;
            float _PaperGrain;

            float luma(float2 uv)
            {
                return log(1.0 + SZ_Luma(SZ_SampleBox4(uv).rgb));
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 np = float2(i.texcoord.x * _ScreenParams.x / _ScreenParams.y, i.texcoord.y);
                
                float seed = _BoilRate > 0.0 ? fmod(floor(_Time.y * _BoilRate), 1024.0) : 0.0;
                float2 seedOffset = float2(SZ_Hash21(seed.xx), SZ_Hash21(seed.xx + 13.7)) * 100.0;

                float2 p = np * _WobbleFrequency + seedOffset;
                float2 wobble = float2(SZ_ValueNoise(p), SZ_ValueNoise(p + 37.0)) - 0.5;
                float2 uv = i.texcoord + wobble * _BlitTexture_TexelSize.xy * _WobbleAmount;
                float2 texel = _BlitTexture_TexelSize.xy * _Thickness;

                float2 oN = texel * float2(0, 1);
                float2 oE = texel * float2(1, 0);
                float2 oS = texel * float2(0, -1);
                float2 oW = texel * float2(-1, 0);

                bool sky = SZ_IsSky(SampleSceneDepth(uv));
                
                float dC = SZ_DepthSignal(uv);
                float lap = SZ_DepthSignal(uv + oN) + SZ_DepthSignal(uv + oE) + SZ_DepthSignal(uv + oS) + SZ_DepthSignal(uv + oW)
                    - 4.0 * dC;
                float depthEdge = abs(lap) / max(dC, 1e-6) * _DepthSensitivity;
                
                float3 normC = SampleSceneNormals(uv);
                float normalDiff = max(max(1.0 - dot(normC, SampleSceneNormals(uv + oN)),
                                           1.0 - dot(normC, SampleSceneNormals(uv + oE))),
                                       max(1.0 - dot(normC, SampleSceneNormals(uv + oS)),
                                           1.0 - dot(normC, SampleSceneNormals(uv + oW))));
                float normalEdge = sky ? 0.0 : normalDiff * _NormalSensitivity;
                
                float lc = luma(uv);
                float lN = luma(uv + oN);
                float lE = luma(uv + oE);
                float lS = luma(uv + oS);
                float lW = luma(uv + oW);
                float colorEdge = (abs(lc - lN) + abs(lc - lE) + abs(lc - lS) + abs(lc - lW)) * _ColorSensitivity;

                float widthNoise = SZ_ValueNoise(np * _WidthFrequency + 71.0 + seedOffset);
                float t = max(_Threshold + (widthNoise - 0.5) * _WidthVariation * 0.3, 0.0);

                float result = max(max(depthEdge, normalEdge), colorEdge);
                result = smoothstep(t, t + 0.1, result);

                float band = lc * _BandCount;
                float d = abs(frac(band + 0.5) - 0.5);
                float bandPerPx = max(length(float2(lE - lW, lN - lS)) / (2.0 * _Thickness) * _BandCount, 1e-5);
                float widthScale = lerp(1.0 - 0.6 * _WidthVariation, 1.0 + 0.6 * _WidthVariation, widthNoise);
                float halfWidth = bandPerPx * _ContourWidth * widthScale * 0.5;
                float contour = 1.0 - smoothstep(halfWidth - bandPerPx * 0.5, halfWidth + bandPerPx * 0.5, d);
                contour *= smoothstep(0.01, 0.04, lc);
                contour *= sky ? 0.0 : 1.0;

                result = max(result, contour);
                
                float grain = SZ_ValueNoise(i.texcoord * _ScreenParams.xy * 0.35);
                result *= lerp(1.0, grain * 0.6 + 0.4, _PaperGrain);

                float4 col = SZ_SamplePoint(i.texcoord);
                float3 base = lerp(col.rgb, _PaperColor.rgb, _PaperMix);
                base *= lerp(1.0, 0.92 + 0.08 * grain, _PaperGrain);

                float3 final = lerp(base, _LineColor.rgb, saturate(result));
                return half4(final, 1);
            }
            ENDHLSL
        }
    }
}

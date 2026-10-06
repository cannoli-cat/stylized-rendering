Shader "Hidden/CannoliCat/Stylized/PaletteSwapper"{
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
            #include "Include/StylizedColor.hlsl"
            
            int _Invert;
            int _PaletteMode;
            float _LightnessWeight;
            // Must match the MaxPaletteSize in PaletteSwapperSettings.cs
            float4 _PaletteColors[64];
            float4 _PaletteLab[64];
            int _PaletteCount;

            half4 frag(Varyings i) : SV_Target
            {
                float4 src = SZ_SamplePoint(i.texcoord);
                if (_PaletteCount <= 0)
                {
                    return src;
                }

                float3 color = src.rgb;

                if (_Invert == 1)
                {
                    color = 1.0 - color;
                }
                
                // 0 = Nearest, 1 = RampSmooth, 2 = RampStepped (matches PaletteMode in C#)
                if (_PaletteMode == 1)
                {
                    float gray = saturate(SZ_LinearToOklab(color).x);
                    float index = gray * (_PaletteCount - 1);
                    
                    float4 color_a = _PaletteColors[(int)floor(index)];
                    float4 color_b = _PaletteColors[(int)ceil(index)];

                    return float4(lerp(color_a.rgb, color_b.rgb, frac(index)), 1.0);
                }
                
                if (_PaletteMode == 2)
                {
                    float gray = saturate(SZ_LinearToOklab(color).x);
                    int index = (int)round(gray * (_PaletteCount - 1));
                    
                    return float4(_PaletteColors[index].rgb, 1.0);
                }

                float3 inputLab = SZ_LinearToOklab(color);
                int bestIdx = 0;
                float bestDist = 1e9;

                for (int idx = 0; idx < _PaletteCount; idx++)
                {
                    float3 d = inputLab - _PaletteLab[idx].xyz;
                    d.x *= _LightnessWeight;
                    float dist = dot(d, d);

                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestIdx = idx;
                    }
                }

                return float4(_PaletteColors[bestIdx].rgb, 1.0);
            }
            ENDHLSL
        }
    }
}
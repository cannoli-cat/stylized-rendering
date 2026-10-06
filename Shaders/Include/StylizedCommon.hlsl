#ifndef STYLIZED_COMMON_INCLUDED
#define STYLIZED_COMMON_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

static const float3 SZ_LumaWeights = float3(0.299, 0.587, 0.114);

float SZ_Luma(float3 color)
{
    return dot(color, SZ_LumaWeights);
}

float4 SZ_SamplePoint(float2 uv)
{
    return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv);
}

float4 SZ_SampleLinear(float2 uv)
{
    return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
}

float4 SZ_SampleBox4(float2 uv)
{
    float2 h = _BlitTexture_TexelSize.xy * 0.5;
    float4 c = SZ_SampleLinear(uv + h * float2(-1, -1))
        + SZ_SampleLinear(uv + h * float2( 1, -1))
        + SZ_SampleLinear(uv + h * float2(-1,  1))
        + SZ_SampleLinear(uv + h * float2( 1,  1));
    return c * 0.25;
}

#endif

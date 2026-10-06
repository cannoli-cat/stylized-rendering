#ifndef STYLIZED_DEPTH_INCLUDED
#define STYLIZED_DEPTH_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

bool SZ_IsSky(float rawDepth)
{
    #if UNITY_REVERSED_Z
    return rawDepth <= 1e-6;
    #else
    return rawDepth >= 1.0 - 1e-6;
    #endif
}

float SZ_DepthSignal(float2 uv)
{
    float raw = SampleSceneDepth(uv);
    if (unity_OrthoParams.w > 0.5)
    {
        #if UNITY_REVERSED_Z
        raw = 1.0 - raw;
        #endif
        return lerp(_ProjectionParams.y, _ProjectionParams.z, raw);
    }
    return _ZBufferParams.z * raw + _ZBufferParams.w;
}

#endif

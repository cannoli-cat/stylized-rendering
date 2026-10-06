#ifndef STYLIZED_COLOR_INCLUDED
#define STYLIZED_COLOR_INCLUDED

// must be linear RGB
// must match OklabUtility.LinearToOklab
float3 SZ_LinearToOklab(float3 c) 
{
    float3 lms = float3(
        dot(c, float3(0.4122214708, 0.5363325363, 0.0514459929)),
        dot(c, float3(0.2119034982, 0.6806995451, 0.1073969566)),
        dot(c, float3(0.0883024619, 0.2817188376, 0.6299787005))
    );
    float3 lms_cbrt = sign(lms) * pow(abs(lms), float3(1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0));
    return float3(
        dot(lms_cbrt, float3(0.2104542553, 0.7936177850, -0.0040720468)),
        dot(lms_cbrt, float3(1.9779984951, -2.4285922050, 0.4505937099)),
        dot(lms_cbrt, float3(0.0259040371, 0.7827717662, -0.8086757660))
    );
}

float3 SZ_OklabToLinear(float3 lab) 
{
    float3 lms_cbrt = float3(
        dot(lab, float3(1.0, 0.3963377774, 0.2158037573)),
        dot(lab, float3(1.0, -0.1055613458, -0.0638541728)),
        dot(lab, float3(1.0, -0.0894841775, -1.2914855480))
    );
    float3 lms = lms_cbrt * lms_cbrt * lms_cbrt;
    return float3(
        dot(lms, float3(4.0767416621, -3.3077115913, 0.2309699292)),
        dot(lms, float3(-1.2684380046, 2.6097574011, -0.3413193965)),
        dot(lms, float3(-0.0041960863, -0.7034186147, 1.7076147010))
    );
}

#endif
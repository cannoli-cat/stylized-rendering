#ifndef STYLIZED_NOISE_INCLUDED
#define STYLIZED_NOISE_INCLUDED

uint2 SZ_Pcg2d(uint2 v)
{
    v = v * 1664525u + 1013904223u;
    v.x += v.y * 1664525u;
    v.y += v.x * 1664525u;
    v ^= v >> 16u;
    v.x += v.y * 1664525u;
    v.y += v.x * 1664525u;
    v ^= v >> 16u;
    return v;
}

float SZ_Hash21(float2 p)
{
    uint2 v = asuint(p);
    v ^= v >> 16u;
    return float(SZ_Pcg2d(v).x >> 8u) * (1.0 / 16777216.0);
}

float SZ_ValueNoise(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);

    float a = SZ_Hash21(i);
    float b = SZ_Hash21(i + float2(1.0, 0.0));
    float c = SZ_Hash21(i + float2(0.0, 1.0));
    float d = SZ_Hash21(i + float2(1.0, 1.0));

    float2 u = f * f * (3.0 - 2.0 * f);

    return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

float SZ_Bayer(uint x, uint y, uint bits)
{
    uint v = 0u;
    for (uint i = 0u; i < bits; i++)
    {
        uint xi = (x >> i) & 1u;
        uint yi = (y >> i) & 1u;
        v = (v << 2u) | (((xi ^ yi) << 1u) | yi);
    }
    return float(v) / float(1u << (bits * 2u)) - 0.5f;
}

#endif

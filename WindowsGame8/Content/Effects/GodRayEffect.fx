//-----------------------------------------------------------------------------
//
//-----------------------------------------------------------------------------
#include "Common.fxh"

#define MAX_SAMPLE_COUNT 2

uniform const float4x4 View;
uniform const float4x4 ViewProjection;
uniform const float3 CameraPosition;

//uniform const float3 LightPosition;
uniform const float3 LightDirection;

uniform const float Exposure = 0.1f;
uniform const int SampleCount = MAX_SAMPLE_COUNT;

uniform const float SampleDensityInverse = 1.0f / 0.8f * MAX_SAMPLE_COUNT;
uniform const float IlluminationDecays[MAX_SAMPLE_COUNT];

uniform const texture SceneMap;
uniform const sampler SceneMapSampler : register(s0) = sampler_state
{
    Texture = <SceneMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

uniform const texture OcclusionMap;
uniform const sampler OcclusionMapSampler = sampler_state
{
    Texture = <OcclusionMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

//-----------------------------------------------------------------------------
//
// Section: Structure
//
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
//
// Section: Vertex shader
//
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
//
// Section: Pixel shader
//
//-----------------------------------------------------------------------------
float4 PS(float2 texCoord : TEXCOORD0) : COLOR0
{
//    float4 lightPosition = mul(LightPosition, ViewProjection);
//    float2 lightTexCoord = ProjectionToTexCoord(lightPosition);
    float4 positionToLight = mul(float4(CameraPosition - LightDirection, 1), ViewProjection);
    float2 lightTexCoord = ProjectionToTexCoord(positionToLight);

    float2 deltaTexCoord = (texCoord - lightTexCoord);
    deltaTexCoord *= SampleDensityInverse;

    float3 color = tex2D(OcclusionMapSampler, texCoord);

    float2 sampleTexCoord = texCoord;
    float3 sample = 0;

    for (int i = 0; i < SampleCount; i++)
    {
        sampleTexCoord -= deltaTexCoord;

        sample = tex2D(OcclusionMapSampler, sampleTexCoord);
        sample *= IlluminationDecays[i];
        color += sample;
    }

    float amount = dot(normalize(mul(-LightDirection, View)), float3(0, 0, -1));
    amount -= 0.5;
    if (amount < 0)
    {
        amount = 0;
    }

    float4 sceneColor = tex2D(SceneMapSampler, texCoord);
    return float4(color * Exposure, 1) + sceneColor * amount;
}

//-----------------------------------------------------------------------------
//
// Section: Technique
//
//-----------------------------------------------------------------------------
technique Default
{
    pass P0
    {
        PixelShader = compile ps_3_0 PS();
    }
}

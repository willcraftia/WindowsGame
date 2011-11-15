//-----------------------------------------------------------------------------
//
// Shadow Scene Common Definitions
//
//-----------------------------------------------------------------------------
#if !defined (SHADOW_SCENE_FXH)
#define SHADOW_SCENE_FXH

//-----------------------------------------------------------------------------
// Section:  Fields
//-----------------------------------------------------------------------------
uniform const float4x4 World;
uniform const float4x4 View;
uniform const float4x4 Projection;

uniform const float4x4 LightViewProjection;
uniform const float DepthBias = 0.001f;

uniform const texture ShadowMap;
uniform const sampler ShadowMapSampler = sampler_state
{
    Texture = <ShadowMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
//    AddressU = Border;
//    AddressV = Border;
//    BorderColor = 0xFFFFFFFF;
};

//-----------------------------------------------------------------------------
//
// Section: Vertex shader
//
//-----------------------------------------------------------------------------
struct VSInput
{
    float4 Position : POSITION;
};

struct VSOutput
{
    float4 Position         : POSITION;
    float4 LightingPosition : TEXCOORD0;
};

VSOutput VS(VSInput input)
{
    VSOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);

    output.Position = mul(viewPosition, Projection);
    output.LightingPosition = mul(worldPosition, LightViewProjection);

    return output;
}

#endif // !defined (SHADOW_SCENE_FXH)

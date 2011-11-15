//-----------------------------------------------------------------------------
//
// Draw sky dome model on scene.
//
//-----------------------------------------------------------------------------
#include "Common.fxh"

uniform const float4x4 World;
uniform const float4x4 View;
uniform const float4x4 Projection;
uniform const float4x4 WorldInvertTranspose;

uniform const float3 LightDirection;
uniform const float3 LightDiffuseColor;
uniform const bool LightingEnabled = true;

uniform const float SunPower = 500;
uniform const float Time;

uniform const texture SkyMap;
uniform const sampler SkyMapSampler = sampler_state
{
    Texture = <SkyMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Mirror;
    AddressV = Clamp;
};

//-----------------------------------------------------------------------------
//
// Section: Structure
//
//-----------------------------------------------------------------------------
struct VsInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal   : NORMAL0;
};

struct VsOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal   : TEXCOORD1;
};

//-----------------------------------------------------------------------------
//
// Section: Vertex shader
//
//-----------------------------------------------------------------------------
VsOutput VS(VsInput input)
{
    VsOutput output;

    output.Position = mul(input.Position, World);
    output.Position = mul(output.Position, View);
    output.Position = mul(output.Position, Projection);

    output.TexCoord = input.TexCoord;

    output.Normal = mul(normalize(input.Normal), WorldInvertTranspose);

    return output;
}

//-----------------------------------------------------------------------------
//
// Section: Pixel shader
//
//-----------------------------------------------------------------------------
float4 PS(VsOutput input) : COLOR0
{
    float2 texCoord = float2(Time, input.TexCoord.y * 2.0f);

    float4 color = tex2D(SkyMapSampler, texCoord);

    if (LightingEnabled)
    {
        float nDotL = max(0, dot(input.Normal, -LightDirection));
//        color.rgb += LightDiffuseColor * pow(nDotL, SunPower);
        // check zero because pow(0, ...) occurs NaN and infinity value.
        if (nDotL != 0)
        {
            color.rgb += LightDiffuseColor * pow(nDotL, SunPower);
        }
    }

    return color;
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
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
    }
}

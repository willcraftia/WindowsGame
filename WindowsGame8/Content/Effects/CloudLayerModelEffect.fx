//-----------------------------------------------------------------------------
//
// Draw sky dome model on scene.
//
//-----------------------------------------------------------------------------
#include "Common.fxh"

//-----------------------------------------------------------------------------
// world matrix
//-----------------------------------------------------------------------------
float4x4 World;

//-----------------------------------------------------------------------------
// View * projection matrix
//-----------------------------------------------------------------------------
float4x4 ViewProjection;

texture CloudLayerMap;
sampler CloudLayerMapSampler = sampler_state
{
    Texture = <CloudLayerMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};


//-----------------------------------------------------------------------------
//
// Section: Structure
//
//-----------------------------------------------------------------------------
struct VsInput
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD0;
};

struct VsOutput
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD0;
};

//-----------------------------------------------------------------------------
//
// Section: Vertex shader
//
//-----------------------------------------------------------------------------
VsOutput VS(VsInput input)
{
    VsOutput output;

    output.Position = mul(mul(input.Position, World), ViewProjection);
    output.TexCoord = input.TexCoord;

    return output;
}

//-----------------------------------------------------------------------------
//
// Section: Pixel shader
//
//-----------------------------------------------------------------------------
float4 PS(VsOutput input) : COLOR0
{
    float2 texCoord = float2(input.TexCoord.x * 16.0f, input.TexCoord.y  * 8.0f);
    return tex2D(CloudLayerMapSampler, texCoord);
//        return tex2D(CloudLayerMapSampler, input.TexCoord);
}

technique Default
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
    }
}

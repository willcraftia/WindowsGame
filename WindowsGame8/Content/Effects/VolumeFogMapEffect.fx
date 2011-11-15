//-----------------------------------------------------------------------------
//
// Shader Model: 2.0
//
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
// Section:  Fields
//-----------------------------------------------------------------------------
uniform const float4x4 World;
uniform const float4x4 View;
uniform const float4x4 Projection;

uniform const float3 FogColor = float3(1, 1, 1);
uniform const float FogScale = 0.01f;

uniform const texture FogDepthCWMap;
uniform const sampler FogDepthCWMapSampler = sampler_state
{
    Texture = <FogDepthCWMap>;
    MipFilter = Point;
    MinFilter = Point;
    MagFilter = Point;
};

uniform const texture FogDepthCCWMap;
uniform const sampler FogDepthCCWMapSampler = sampler_state
{
    Texture = <FogDepthCCWMap>;
    MipFilter = Point;
    MinFilter = Point;
    MagFilter = Point;
};

//-----------------------------------------------------------------------------
// Section:  Vertex shader
//-----------------------------------------------------------------------------
struct VSInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position     : POSITION0;
    float2 TexCoord     : TEXCOORD0;
    float4 PositionWVP  : TEXCOORD1;
};

VSOutput VS(VSInput input)
{
    VSOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);

    output.Position = mul(viewPosition, Projection);
    output.PositionWVP = output.Position;

    output.TexCoord = input.TexCoord;

    return output;
}

//-----------------------------------------------------------------------------
// Section:  Pixel shader
//-----------------------------------------------------------------------------
float4 PS(VSOutput input) : COLOR0
{
    float2 texCoord = input.PositionWVP.xy / input.PositionWVP.w * float2(0.5f, -0.5f) + 0.5f;

    float4 t0 = tex2D(FogDepthCWMapSampler, texCoord);
    float4 t1 = tex2D(FogDepthCCWMapSampler, texCoord);

    float deltaDepth = saturate((t0.x - t1.x) * FogScale);

    return float4(FogColor, deltaDepth);
}

//-----------------------------------------------------------------------------
// Section:  Technique
//-----------------------------------------------------------------------------
technique Default
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
    }
}

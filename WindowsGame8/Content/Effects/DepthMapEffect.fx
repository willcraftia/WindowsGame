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

//-----------------------------------------------------------------------------
// Section:  Vertex shader
//-----------------------------------------------------------------------------
struct VSInput
{
    float4 Position : POSITION;
};

struct VSOutput
{
    float4 Position     : POSITION;
    float4 PositionWVP  : TEXCOORD0;
};

VSOutput VS(VSInput input)
{
    VSOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);

    output.Position = mul(viewPosition, Projection);
    output.PositionWVP = output.Position;

    return output;
}

//-----------------------------------------------------------------------------
// Section:  Pixel shader
//-----------------------------------------------------------------------------
float4 PS(VSOutput input) : COLOR0
{
    float depth = input.PositionWVP.z / input.PositionWVP.w;
    return float4(depth, depth, depth, 1);
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

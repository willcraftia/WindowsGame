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

uniform const texture SceneDepthMap;
uniform const sampler SceneDepthMapSampler = sampler_state
{
    Texture = <SceneDepthMap>;
    MipFilter = Point;
    MinFilter = Point;
    MagFilter = Point;
};

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
    float Depth         : TEXCOORD1;
};

VSOutput VS(VSInput input)
{
    VSOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);

    output.Position = mul(viewPosition, Projection);
    output.PositionWVP = output.Position;

    output.Depth = output.Position.w;

    return output;
}

//-----------------------------------------------------------------------------
// Section:  Pixel shader
//-----------------------------------------------------------------------------
float4 PS(VSOutput input) : COLOR0
{
    float2 texCoord = input.PositionWVP.xy / input.PositionWVP.w * float2(0.5f, -0.5f) + 0.5f;
    float sceneDepth = tex2D(SceneDepthMapSampler, texCoord).x;

    float depth = input.Depth;
    return float4(min(depth, sceneDepth), 0, 0, 0);
}

//-----------------------------------------------------------------------------
// Category: FogDepthCW
// Section:  Technique
//-----------------------------------------------------------------------------
technique FogDepthCW
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
        CullMode = CW;
        ZFunc = Greater;
    }
}

//-----------------------------------------------------------------------------
// Category: FogDepthCCW
// Section:  Technique
//-----------------------------------------------------------------------------
technique FogDepthCCW
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
        CullMode = CCW;
        ZFunc = Less;
    }
}

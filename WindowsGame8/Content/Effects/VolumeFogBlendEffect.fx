//-----------------------------------------------------------------------------
//
// Shader Model: 2.0
//
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
// Section:  Fields
//-----------------------------------------------------------------------------
uniform const float2 ViewportSize;

uniform const texture SceneMap;
uniform const sampler SceneMapSampler = sampler_state
{
    Texture = <SceneMap>;
};

uniform const texture FogMap;
uniform const sampler FogMapSampler = sampler_state
{
    Texture = <FogMap>;
};

//-----------------------------------------------------------------------------
// Section:  Vertex shader
//-----------------------------------------------------------------------------
void VS(
    inout float4 position : POSITION0,
    inout float2 texCoord : TEXCOORD0)
{
    position.xy -= float2(1.0, -1.0) / ViewportSize;
/*
    // Half pixel offset for correct texel centering.
    position.xy -= 0.5;

    // Viewport adjustment.
    position.xy /= ViewportSize;
    position.xy *= float2(2, -2);
    position.xy -= float2(1, -1);
*/
}

//-----------------------------------------------------------------------------
// Section:  Pixel shader
//-----------------------------------------------------------------------------
float4 PS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 scene = tex2D(SceneMapSampler, texCoord);
    float4 fog = tex2D(FogMapSampler, texCoord);

    return lerp(scene, float4(fog.rgb, 1), fog.a);
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

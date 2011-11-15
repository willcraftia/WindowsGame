//-----------------------------------------------------------------------------
//
// Shader Model: 2.0
//
// Draw scene contains only shadow using classic shadow technique.
//
//-----------------------------------------------------------------------------
#include "Common.fxh"
#include "Shadow.fxh"
#include "ShadowScene.fxh"

//-----------------------------------------------------------------------------
// Section: Pixel shader
//-----------------------------------------------------------------------------
float4 PS(VSOutput input) : COLOR0
{
    float4 lightingPosition = input.LightingPosition;

    float2 shadowTexCoord = ProjectionToTexCoord(lightingPosition);
    float shadow = TestClassicShadowMap(
        ShadowMapSampler,
        shadowTexCoord,
        lightingPosition,
        DepthBias);

    return float4(shadow, shadow, shadow, 1);
}

//-----------------------------------------------------------------------------
// Section: Technique
//-----------------------------------------------------------------------------
technique Default
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
    }
}

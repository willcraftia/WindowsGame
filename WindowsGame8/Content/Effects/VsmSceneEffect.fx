//-----------------------------------------------------------------------------
//
// Draw scene contains only shadow using VSM shadow technique.
//
//-----------------------------------------------------------------------------
#include "Common.fxh"
#include "Shadow.fxh"
#include "ShadowScene.fxh"

//-----------------------------------------------------------------------------
//
// Section: Pixel shader
//
//-----------------------------------------------------------------------------
float4 PS(VSOutput input) : COLOR0
{
    float4 lightingPosition = input.LightingPosition;
    float2 shadowTexCoord = ProjectionToTexCoord(lightingPosition);

    float4 result = float4(1, 0, 0, 1);
    float shadow = TestVarianceShadowMap(ShadowMapSampler, shadowTexCoord, lightingPosition, DepthBias);
    return float4(shadow, shadow, shadow, 1);
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

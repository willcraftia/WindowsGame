//-----------------------------------------------------------------------------
//
// Draw scene contains only shadow using PCF shadow technique.
//
//-----------------------------------------------------------------------------
#include "Common.fxh"
#include "Shadow.fxh"
#include "ShadowScene.fxh"

float TapCount;
float2 Offsets[MAX_PCF_TAP_COUNT];

//-----------------------------------------------------------------------------
//
// Section: Pixel shader
//
//-----------------------------------------------------------------------------
float4 PS(VSOutput input) : COLOR
{
    float4 lightingPosition = input.LightingPosition;
    float2 shadowTexCoord = ProjectionToTexCoord(lightingPosition);

    float shadow = TestPcfShadowMap(ShadowMapSampler, shadowTexCoord, lightingPosition, DepthBias, TapCount, Offsets);
    return float4(shadow, shadow, shadow, 1);
}

//-----------------------------------------------------------------------------
//
// Section: Technique
//
//
// NOTE:
// I want to use more PCF taps but compiler occurs error when i define it.
//
//-----------------------------------------------------------------------------
technique Default
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS();
    }
}

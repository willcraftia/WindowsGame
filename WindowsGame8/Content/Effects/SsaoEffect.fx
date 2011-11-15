//-----------------------------------------------------------------------------
//
// Draw SSAO
//
//-----------------------------------------------------------------------------
float3 ShadowColor = float3(0, 0, 0);

texture SceneMap;
sampler2D SceneMapSampler : register(s0) = sampler_state
{
    Texture = <SceneMap>;
    AddressU = Clamp;
    AddressV = Clamp;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
};

texture SsaoMap;
sampler2D SsaoMapSampler = sampler_state
{
    Texture = <SsaoMap>;
    AddressU = Clamp;
    AddressV = Clamp;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
};

//-----------------------------------------------------------------------------
//
// Section: Structure
//
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
//
// Section: Vertex shader
//
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
//
// Section: Pixel shader
//
//-----------------------------------------------------------------------------
float4 PS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float ao = tex2D(SsaoMapSampler, texCoord).r;
    float3 sceneColor = tex2D(SceneMapSampler, texCoord);
//    float3 mixedLightColor = lerp(ShadowColor, sceneColor, ao);
//    return float4(sceneColor * mixedLightColor, 1);
//    return float4(sceneColor * ao, 1);
//    float3 shadowSceneColor = ShadowColor * sceneColor;
//    return float4(lerp(sceneColor, shadowSceneColor, (1.0 - ao)), 1);
    return float4(sceneColor * ao, 1);
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
        PixelShader = compile ps_2_0 PS();
    }
}

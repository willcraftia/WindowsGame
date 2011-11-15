//-----------------------------------------------------------------------------
//
// Draw Screen Space Shadow
//
//-----------------------------------------------------------------------------
float3 ShadowColor = float3(0, 0, 0);

texture SceneMap;
sampler2D SceneMapSampler : register(s0) = sampler_state
{
    Texture = <SceneMap>;
    AddressU = Clamp;
    AddressV = Clamp;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = None;
};

texture ShadowSceneMap;
sampler2D ShadowSceneMapSampler = sampler_state
{
    Texture = <ShadowSceneMap>;
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
    float shadow = tex2D(ShadowSceneMapSampler, texCoord).r;
    float3 sceneColor = tex2D(SceneMapSampler, texCoord);
    float3 shadedColor = sceneColor * ShadowColor;
    float3 finalColor = lerp(shadedColor, sceneColor, shadow);
    return float4(finalColor, 1);
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

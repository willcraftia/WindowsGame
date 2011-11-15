//-----------------------------------------------------------------------------
//
// Draw DoF
//
//-----------------------------------------------------------------------------

float FocusRange;
float FocusDistance;
float NearPlaneDistance;
//
// modified far plane distance
//
// far / (far - near)
//
float FarPlaneDistance;

texture SceneMap;
sampler SceneMapSampler : register(s0) = sampler_state
{
    Texture = <SceneMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture DepthMap;
sampler DepthMapSampler = sampler_state
{
    Texture = <DepthMap>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture BluredSceneMap;
sampler BluredSceneMapSampler = sampler_state
{
    Texture = <BluredSceneMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

//-----------------------------------------------------------------------------
// Section: Vertex shader
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Section: Pixel shader
//-----------------------------------------------------------------------------
float4 PS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 scene = tex2D(SceneMapSampler, texCoord);
    float4 bluredScene = tex2D(BluredSceneMapSampler, texCoord);
    float depth = tex2D(DepthMapSampler, texCoord).x;

    float sceneDistance = (-NearPlaneDistance * FarPlaneDistance) / (depth - FarPlaneDistance);
    float blurFactor = saturate(abs(sceneDistance - FocusDistance) / FocusRange);

    return lerp(scene, bluredScene, blurFactor);
}

//-----------------------------------------------------------------------------
// Section: Technique
//-----------------------------------------------------------------------------
technique Default
{
    pass P0
    {
        PixelShader = compile ps_2_0 PS();
    }
}

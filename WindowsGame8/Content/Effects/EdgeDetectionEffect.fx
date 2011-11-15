//-----------------------------------------------------------------------------
//
// Edge detection
//
//-----------------------------------------------------------------------------
float2 EdgeOffset;
float EdgeIntensity = 1;

float NormalThreshold = 0.5;
//float NormalThreshold = 0.001;
//float DepthThreshold = 0.1;
float DepthThreshold = 0.0;

float NormalSensitivity = 1;
float DepthSensitivity = 10;

float3 EdgeColor = float3(0, 0, 0);

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

texture NormalDepthMap;
sampler NormalDepthMapSampler = sampler_state
{
    Texture = <NormalDepthMap>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
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
    float4 color = tex2D(SceneMapSampler, texCoord);

    float4 n = tex2D(NormalDepthMapSampler, texCoord);
    float4 n1 = tex2D(NormalDepthMapSampler, texCoord + float2(-1, -1) * EdgeOffset);
    float4 n2 = tex2D(NormalDepthMapSampler, texCoord + float2( 1,  1) * EdgeOffset);
    float4 n3 = tex2D(NormalDepthMapSampler, texCoord + float2(-1,  1) * EdgeOffset);
    float4 n4 = tex2D(NormalDepthMapSampler, texCoord + float2( 1, -1) * EdgeOffset);

    float4 diagonalDelta = abs(n1 - n2) + abs(n3 - n4);

    float normalDelta = dot(diagonalDelta.xyz, 1);
    float depthDelta = diagonalDelta.w;

    normalDelta = saturate((normalDelta - NormalThreshold) * NormalSensitivity);
    depthDelta = saturate((depthDelta - DepthThreshold) * DepthSensitivity);

    float edgeAmount = saturate(normalDelta + depthDelta) * EdgeIntensity;
//    edgeAmount *= (1 - n.w);
    edgeAmount *= (1 + log(n.w));
//    edgeAmount *= (1 - exp(n.w - 1));

//    float4 c1 = tex2D(SceneMapSampler, texCoord + float2(-1, -1) * EdgeOffset);
//    float4 c2 = tex2D(SceneMapSampler, texCoord + float2( 1,  1) * EdgeOffset);
//    float4 c3 = tex2D(SceneMapSampler, texCoord + float2(-1,  1) * EdgeOffset);
//    float4 c4 = tex2D(SceneMapSampler, texCoord + float2( 1, -1) * EdgeOffset);

//    float ec = (c1 + c2 + c3 + c4) * 0.25f;
//    ec *= EdgeColor;

//    color.rgb *= (1 - edgeAmount);
    color.rgb = lerp(color.rgb, color.rgb * EdgeColor, edgeAmount);
//    color.rgb = lerp(color.rgb, color.rgb * ec, edgeAmount);

    return color;
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

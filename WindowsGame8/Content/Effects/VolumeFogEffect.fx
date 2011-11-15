//-----------------------------------------------------------------------------
//
// Shader Model: 2.0
//
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
// Section:  Fields
//-----------------------------------------------------------------------------
uniform const float2 ViewportSize;
uniform const float3 FogColor = float3(1, 1, 1);
uniform const float FogScale = 0.01f;

uniform const texture SceneMap;
uniform const sampler SceneMapSampler = sampler_state
{
    Texture = <SceneMap>;
};

uniform const texture FogDepthCWMap;
uniform const sampler FogDepthCWMapSampler = sampler_state
{
    Texture = <FogDepthCWMap>;
    MipFilter = Point;
    MinFilter = Point;
    MagFilter = Point;
};

uniform const texture FogDepthCCWMap;
uniform const sampler FogDepthCCWMapSampler = sampler_state
{
    Texture = <FogDepthCCWMap>;
    MipFilter = Point;
    MinFilter = Point;
    MagFilter = Point;
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
    float4 t0 = tex2D(FogDepthCWMapSampler, texCoord);
    float4 t1 = tex2D(FogDepthCCWMapSampler, texCoord);

    float deltaDepth = saturate((t0.x - t1.x) * FogScale);

    // Z値の差が大きいほどフォグを濃く描画する
    // ただし裏向きと表向きの両方のZ値を書き込んだときだけ
    // フォグを描画する
//    return float4(FogColor, FogAlpha * deltaDepth * t0.y * t1.y);

    float4 scene = tex2D(SceneMapSampler, texCoord);
    return lerp(scene, float4(FogColor, 1), deltaDepth);
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

//-----------------------------------------------------------------------------
//
// Draw the wave map and the normal map to render a fluid surface.
//
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
// Category: WaveMap
// Section:  Fields
//-----------------------------------------------------------------------------
//uniform const texture WaveMap;
uniform const sampler WaveMapSampler : register(s0) = sampler_state
{
//    Texture = <WaveMap>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    //
    // NOTE:
    // Set texture addresses into GraphicsDevice.SamplerState[0]
    // via SpriteBatch.Begin(...).
};

uniform const float2 SampleOffset;
uniform const float SpringPower = 0.5f;

uniform const float2 AddWavePosition;
uniform const float AddWaveRadius;
uniform const float AddWaveVelocity;

//-----------------------------------------------------------------------------
// Category: WaveMap
// Section:  Vertex shader
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Category: WaveMap
// Section:  Pixel shader
//-----------------------------------------------------------------------------
float4 WaveMapPS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 t0 = tex2D(WaveMapSampler, texCoord);
    float4 t1 = tex2D(WaveMapSampler, texCoord + float2(SampleOffset.x, 0));
    float4 t2 = tex2D(WaveMapSampler, texCoord + float2(-SampleOffset.x, 0));
    float4 t3 = tex2D(WaveMapSampler, texCoord + float2(0, SampleOffset.y));
    float4 t4 = tex2D(WaveMapSampler, texCoord + float2(0, -SampleOffset.y));

    float velocity = t0.g + SpringPower * (t1.r + t2.r + t3.r + t4.r - t0.r * 4);
    float height = t0.r + velocity;

    if (distance(AddWavePosition, texCoord) <= AddWaveRadius)
    {
        velocity += AddWaveVelocity;
    }

    return float4(height, velocity, 0, 0);
}

//-----------------------------------------------------------------------------
// Category: WaveMap
// Section:  Technique
//-----------------------------------------------------------------------------
technique CreateWaveMap
{
    pass P0
    {
        PixelShader = compile ps_2_0 WaveMapPS();
    }
}

//-----------------------------------------------------------------------------
// Category: NormalMap
// Section:  Fields
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Category: NormalMap
// Section:  Vertex shader
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Category: NormalMap
// Section:  Pixel shader
//-----------------------------------------------------------------------------
float4 NormalMapPS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 t1 = tex2D(WaveMapSampler, texCoord + float2(SampleOffset.x, 0));
    float4 t2 = tex2D(WaveMapSampler, texCoord + float2(-SampleOffset.x, 0));
    float4 t3 = tex2D(WaveMapSampler, texCoord + float2(0, SampleOffset.y));
    float4 t4 = tex2D(WaveMapSampler, texCoord + float2(0, -SampleOffset.y));

    float u = (t2.r - t1.r) * 0.5f;
    float v = (t4.r - t3.r) * 0.5f;

    return float4(u, v, 0, 0);
}

//-----------------------------------------------------------------------------
// Category: NormalMap
// Section:  Technique
//-----------------------------------------------------------------------------
technique CreateNormalMap
{
    pass P0
    {
        PixelShader = compile ps_2_0 NormalMapPS();
    }
}

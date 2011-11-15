//-----------------------------------------------------------------------------
//
// Draw cloud map with 2D Perlin Noise.
// (Is this Perlin Noise really ?)
//
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Cloudiness
//-----------------------------------------------------------------------------
//float Cloudiness = 1.5f;
float Cloudiness = 1.0f;

//-----------------------------------------------------------------------------
// Time to move tex coord
//-----------------------------------------------------------------------------
float Time = 0.49f;

//-----------------------------------------------------------------------------
// Velocity to move tex coord
//-----------------------------------------------------------------------------
float2 Velocity = float2(1, 0);

//-----------------------------------------------------------------------------
// Offset
//-----------------------------------------------------------------------------
float2 Offset;

texture NoiseMap;
sampler NoiseMapSampler : register(s0) = sampler_state
{
    texture = <NoiseMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
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
float4 BACKUP_PixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(NoiseMapSampler, texCoord + Time * Velocity) / 2;
    color += tex2D(NoiseMapSampler, texCoord * 2   + Time * Velocity) / 4;
    color += tex2D(NoiseMapSampler, texCoord * 4   + Time * Velocity) / 8;
    color += tex2D(NoiseMapSampler, texCoord * 8   + Time * Velocity) / 16;
    color += tex2D(NoiseMapSampler, texCoord * 16  + Time * Velocity) / 32;
    color += tex2D(NoiseMapSampler, texCoord * 32  + Time * Velocity) / 32;
    return 1.0f - pow(color, Cloudiness) * 2.0f;
}

float4 PS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(NoiseMapSampler, texCoord + Offset) / 2;
    color += tex2D(NoiseMapSampler, texCoord * 2   + Offset) / 4;
    color += tex2D(NoiseMapSampler, texCoord * 4   + Offset) / 8;
    color += tex2D(NoiseMapSampler, texCoord * 8   + Offset) / 16;
    color += tex2D(NoiseMapSampler, texCoord * 16  + Offset) / 32;
    color += tex2D(NoiseMapSampler, texCoord * 32  + Offset) / 32;
    return 1.0f - pow(color, Cloudiness) * 2.0f;
}

technique Default
{
    pass P0
    {
        PixelShader = compile ps_2_0 PS();
    }
}

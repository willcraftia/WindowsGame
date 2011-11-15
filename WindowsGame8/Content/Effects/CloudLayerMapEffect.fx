//-----------------------------------------------------------------------------
//
// Draw cloud map with 2D Perlin Noise.
// (Is this Perlin Noise really ?)
//
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Index of the current layer
//-----------------------------------------------------------------------------
int LayerIndex;

//-----------------------------------------------------------------------------
// Offset
//-----------------------------------------------------------------------------
float Offset;

//-----------------------------------------------------------------------------
// The absorption of the light by the previous layer's cloud
//-----------------------------------------------------------------------------
float LightAbsorption;

//-----------------------------------------------------------------------------
// The color of clouds
//-----------------------------------------------------------------------------
float3 CloudColor = float3(1, 1, 1);

texture CloudMap;
sampler CloudMapSampler : register(s0) = sampler_state
{
    texture = <CloudMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture PreviousCloudLayerMap;
sampler PreviousCloudLayerMapSampler = sampler_state
{
    texture = <PreviousCloudLayerMap>;
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
float4 PS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 mapColor = tex2D(CloudMapSampler, texCoord);
    float4 color;

    if (LayerIndex == 0)
    {
        // first layer is unicolor
        color =  float4(CloudColor, mapColor.r);
    }
    else
    {
        // layer are derived from previous layer
        float2 layerOffset = float2(LayerIndex * Offset, 0);
        float4 prevLayerColor = tex2D(PreviousCloudLayerMapSampler, texCoord - layerOffset);
        float decrement = float(mapColor.r * LightAbsorption);
        color = float4(prevLayerColor.r - decrement,
                       prevLayerColor.g - decrement,
                       prevLayerColor.b - decrement,
                       mapColor.r);
    }
    return color;
}

technique Default
{
    pass P0
    {
        PixelShader = compile ps_2_0 PS();
    }
}

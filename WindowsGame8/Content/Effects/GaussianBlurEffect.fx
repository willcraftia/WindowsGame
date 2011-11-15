//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------
// for ps_3_0
//#define MAX_RADIUS 7
// for ps_2_0
#define MAX_RADIUS 4
#define KERNEL_SIZE (MAX_RADIUS * 2 + 1)

float KernelSize = KERNEL_SIZE;
float Weights[KERNEL_SIZE];
float2 OffsetsH[KERNEL_SIZE];
float2 OffsetsV[KERNEL_SIZE];

texture ColorMap;
sampler ColorMapSampler : register(s0) = sampler_state
{
    Texture = <ColorMap>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

//----------------------------------------------------------------------------
// As the distance from the origin in the horizontal axis
//----------------------------------------------------------------------------
float4 HorizontalBlurPixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 c = 0;
    for (int i = 0; i < KernelSize; i++)
    {
        c += tex2D(ColorMapSampler, texCoord + OffsetsH[i]) * Weights[i];
    }
    return c;
}

technique HorizontalBlur
{
    pass P0
    {
        PixelShader = compile ps_2_0 HorizontalBlurPixelShader();
    }
}

//----------------------------------------------------------------------------
// As the distance from the origin in the vertical axis
//----------------------------------------------------------------------------
float4 VerticalBlurPixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 c = 0;
    for (int i = 0; i < KernelSize; i++)
    {
        c += tex2D(ColorMapSampler, texCoord + OffsetsV[i]) * Weights[i];
    }
    return c;
}

technique VerticalBlur
{
    pass P0
    {
        PixelShader = compile ps_2_0 VerticalBlurPixelShader();
    }
}

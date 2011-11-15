//-----------------------------------------------------------------------------
//
// Draw SSAO
//
//-----------------------------------------------------------------------------
#define MAX_RADIUS 2
#define KERNEL_SIZE (MAX_RADIUS * 2 + 1)

float KernelSize = KERNEL_SIZE;
float Weights[KERNEL_SIZE];
float2 OffsetsH[KERNEL_SIZE];
float2 OffsetsV[KERNEL_SIZE];

texture SsaoMap;
sampler2D SsaoMapSampler : register(s0) = sampler_state
{
    Texture = <SsaoMap>;
    AddressU = Clamp;
    AddressV = Clamp;
    MipFilter = None;
    MinFilter = Linear;
    MagFilter = Linear;
};

texture NormalDepthMap;
sampler2D NormalDepthMapSampler = sampler_state
{
    Texture = <NormalDepthMap>;
    AddressU = Clamp;
    AddressV = Clamp;
    MipFilter = None;
    MinFilter = Linear;
    MagFilter = Linear;
};

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
float4 HorizontalBlurPixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 normalDepth = tex2D(NormalDepthMapSampler, texCoord);
    float3 normal = normalize(normalDepth.rgb * 2.0f - 1.0f);
    float ssao = tex2D(SsaoMapSampler, texCoord).r;

    float4 c = 0;
    for (int i = 0; i < KernelSize; i++)
    {
        float2 sampleTexCoord = texCoord + OffsetsH[i];
        float4 sampleNormalDepth = tex2D(NormalDepthMapSampler, sampleTexCoord);
        float3 sampleNormal = normalize(sampleNormalDepth.rgb * 2.0f - 1.0f);

        float coeff = dot(sampleNormal, normal);
        c += tex2D(SsaoMapSampler, sampleTexCoord) * Weights[i] * coeff;
    }
    return c;
}

float4 VerticalBlurPixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 normalDepth = tex2D(NormalDepthMapSampler, texCoord);
    float3 normal = normalize(normalDepth.rgb * 2.0f - 1.0f);
    float ssao = tex2D(SsaoMapSampler, texCoord).r;

    float4 c = 0;
    for (int i = 0; i < KernelSize; i++)
    {
        float2 sampleTexCoord = texCoord + OffsetsV[i];
        float4 sampleNormalDepth = tex2D(NormalDepthMapSampler, sampleTexCoord);
        float3 sampleNormal = normalize(sampleNormalDepth.rgb * 2.0f - 1.0f);

        float coeff = dot(sampleNormal, normal);
        c += tex2D(SsaoMapSampler, sampleTexCoord) * Weights[i] * coeff;
    }
    return c;
}

//-----------------------------------------------------------------------------
//
// Section: Technique
//
//-----------------------------------------------------------------------------
technique HorizontalBlur
{
    pass P0
    {
        PixelShader = compile ps_2_0 HorizontalBlurPixelShader();
    }
}

technique VerticalBlur
{
    pass P0
    {
        PixelShader = compile ps_2_0 VerticalBlurPixelShader();
    }
}

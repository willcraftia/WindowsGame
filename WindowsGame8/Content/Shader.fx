//-----------------------------------------
//
//-----------------------------------------

float4x4 LightViewProjection;
float4x4 World;
float MaxDepth = 1000.0f;

struct DepthMapVSOutput
{
    float4 Position : POSITION;
    float Depth     : TEXCOORD0;
};

DepthMapVSOutput DepthMapVertexShader(float4 position : POSITION)
{
    DepthMapVSOutput output = (DepthMapVSOutput) 0;

    output.Position = mul(mul(position, World), LightViewProjection);
    output.Depth = output.Position.z / MaxDepth;

    return output;
}

float4 DepthMapPixelShader(DepthMapVSOutput input) : COLOR0
{
    return float4(input.Depth, input.Depth, input.Depth, 1.0f);
}

technique DepthMap
{
    pass P0
    {
          VertexShader = compile vs_1_1 DepthMapVertexShader();
          PixelShader = compile ps_1_1 DepthMapPixelShader();
    }
}

//-----------------------------------------
//
//-----------------------------------------

float4x4 ViewProjection;
float2 PCFSamples[9];
float DepthBias = 0.0030f;

texture DepthMapTexture;
sampler DepthMapSampler = sampler_state
{
    Texture = <DepthMapTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct PCFShadowVSInput
{
    float4 Position : POSITION;
};

struct PCFShadowVSOutput
{
    float4 Position     : POSITION;
    float4 ShadowPos    : TEXCOORD0;
    float4 RealDistance : TEXCOORD1;
};

PCFShadowVSOutput PCFShadowVertexShader(PCFShadowVSInput input)
{
    PCFShadowVSOutput output;

    float4 worldPosition = mul(input.Position, World);
    output.Position = mul(worldPosition, ViewProjection);
    output.ShadowPos = mul(worldPosition, LightViewProjection);
    output.RealDistance = output.ShadowPos.z / MaxDepth;

    return output;
}

float4 PCFShadowPixelShader(PCFShadowVSOutput input) : COLOR
{
    float2 projectedTexCoords;

    projectedTexCoords[0] = input.ShadowPos.x / input.ShadowPos.w / 2.0f + 0.5f;
    projectedTexCoords[1] = -input.ShadowPos.y / input.ShadowPos.w / 2.0f + 0.5f;

    float4 result = {1, 1, 1, 1};
    if ((saturate(projectedTexCoords.x) == projectedTexCoords.x) &&
        (saturate(projectedTexCoords.y) == projectedTexCoords.y))
    {
        result = float4(0, 0, 0, 0);
        float shadowTerm = 0.0f;
        for (int i = 0; i < 9; i++)
        {
            float storedDepthInShadowMap = tex2D(DepthMapSampler, projectedTexCoords + PCFSamples[i]).x;
            if ((input.RealDistance.x - DepthBias) <= storedDepthInShadowMap)
            {
                shadowTerm++;
            }
        }

        shadowTerm /= 9.0f;

        result = 1.0f * shadowTerm;
    }

    return result;
}

technique PCFShadow
{
    pass P0
    {
          VertexShader = compile vs_2_0 PCFShadowVertexShader();
          PixelShader = compile ps_2_0 PCFShadowPixelShader();
    }
}

//-----------------------------------------
//
//-----------------------------------------

float4 LightPosition;

float AmbientIntensity = 0.8f;
float4 AmbientColor = { 0.5f, 0.5f, 0.5f, 1.0f };
float DiffuseIntensity = 1.0f;

texture ShadowTexture;
sampler ShadowTextureSampler = sampler_state
{
    Texture = <ShadowTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = <Texture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
//    AddressU = Mirror;
//    AddressV = Mirror;
};

struct ShadeVSInput
{
    float4 Position : POSITION0;
    float3 Normal   : NORMAL;
    float2 TexCoord : TEXCOORD0;
};

struct ShadeVSOutput
{
    float4 Position         : POSITION0;
    float2 TexCoord         : TEXCOORD0;
    float2 ProjCoord        : TEXCOORD1;
    float4 ScreenCoord      : TEXCOORD2;
    float4 WorldPosition    : TEXCOORD3;
    float3 Normal           : TEXCOORD4;
};

ShadeVSOutput ShadeVertexShader(ShadeVSInput input)
{
    ShadeVSOutput output;

    output.WorldPosition = mul(input.Position, World);
    output.Position = mul(output.WorldPosition, ViewProjection);

    output.TexCoord = input.TexCoord;

    float4 shadowPos = mul(output.WorldPosition, LightViewProjection);
    output.ProjCoord[0] = shadowPos.x / shadowPos.w / 2.0f + 0.5f;
    output.ProjCoord[1] = -shadowPos.y / shadowPos.w / 2.0f + 0.5f;

    output.ScreenCoord.x = (output.Position.x * 0.5f + output.Position.w * 0.5f);
    output.ScreenCoord.y = (output.Position.w * 0.5f - output.Position.y * 0.5f);
    output.ScreenCoord.z = output.Position.w;
    output.ScreenCoord.w = output.Position.w;

    output.Normal = normalize(mul(input.Normal, (float3x3) World));

    return output;
}

float4 ShadePixelShader(ShadeVSOutput input) : COLOR0
{
    float4 color = tex2D(TextureSampler, input.TexCoord);
    float4 shadowFactor = tex2Dproj(ShadowTextureSampler, input.ScreenCoord);

    float3 lightDirection = normalize(LightPosition - input.WorldPosition);
    float LDotN = saturate(dot(lightDirection, input.Normal));

    return AmbientIntensity * AmbientColor * color + DiffuseIntensity * LDotN * color * shadowFactor;
}

technique Shade
{
    pass P0
    {
        VertexShader = compile vs_2_0 ShadeVertexShader();
        PixelShader = compile ps_2_0 ShadePixelShader();
    }
}

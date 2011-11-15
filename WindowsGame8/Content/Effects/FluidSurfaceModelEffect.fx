//-----------------------------------------------------------------------------
//
// Shader Model: 3.0
//
// Draw fluid surface.
//
//-----------------------------------------------------------------------------
#include "Common.fxh"

//-----------------------------------------------------------------------------
// Section:  Fields
//-----------------------------------------------------------------------------
uniform const float4x4 World;
uniform const float4x4 View;
uniform const float4x4 Projection;
uniform const float3 EyePosition;

uniform const float FogEnabled;
uniform const float FogStart;
uniform const float FogEnd;
uniform const float3 FogColor;

uniform const float3 DiffuseColor;
uniform const float3 EmissiveColor;
uniform const float3 SpecularColor;
uniform const float SpecularPower;

uniform const float3 AmbientLightColor;

uniform const float3 DirLight0Direction;
uniform const float3 DirLight0DiffuseColor;
uniform const float3 DirLight0SpecularColor;

uniform const float3 DirLight1Direction;
uniform const float3 DirLight1DiffuseColor;
uniform const float3 DirLight1SpecularColor;

uniform const float3 DirLight2Direction;
uniform const float3 DirLight2DiffuseColor;
uniform const float3 DirLight2SpecularColor;

uniform const bool LightingEnabled;

uniform const float MinAlpha = 0.7f;
uniform const float MaxAlpha = 0.95f;
uniform const float DistanceAlphaFactor = 0.01f;

static const float R0 = 0.02037f;

uniform const float2 SampleOffset;
uniform const float2 TextureSize;
uniform const float TextureScale = 1.0f;

uniform const texture NormalMap;
uniform const sampler NormalMapSampler = sampler_state
{
    Texture = <NormalMap>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU  = Wrap;
    AddressV  = Wrap;
};

//-----------------------------------------------------------------------------
// Section:  Structure
//-----------------------------------------------------------------------------
struct VSInput
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position         : POSITION;
    float3 ToEye            : TEXCOORD0;
    float2 TexCoord         : TEXCOORD1;
    float4 WorldPosition    : TEXCOORD3;
    float FogFactor         : TEXCOORD4;
};

//
// REFERENCE: BasicEffect shader
//
struct ColorPair
{
    float3 Diffuse;
    float3 Specular;
};

//-----------------------------------------------------------------------------
// Section:  Vertex shader
//-----------------------------------------------------------------------------

// REFERENCE: I copied BasicEffect
float ComputeFogFactor(float d)
{
    return clamp((d - FogStart) / (FogEnd - FogStart), 0, 1) * FogEnabled;
}

VSOutput VS(VSInput input)
{
    VSOutput output;

    output.WorldPosition = mul(input.Position, World);
    output.Position = mul(output.WorldPosition, View);
    output.Position = mul(output.Position, Projection);

    output.ToEye = EyePosition - output.WorldPosition;

    output.TexCoord = input.TexCoord * TextureScale;

    output.FogFactor = ComputeFogFactor(length(output.ToEye));

    return output;
}

//-----------------------------------------------------------------------------
// Section:  Pixel shader
//-----------------------------------------------------------------------------
float3 ComputeNormal(float2 texCoord)
{
    float4 t0 = tex2D(NormalMapSampler, texCoord);
    float4 t1 = tex2D(NormalMapSampler, texCoord + float2(SampleOffset.x, 0));
    float4 t2 = tex2D(NormalMapSampler, texCoord + float2(0, SampleOffset.y));
    float4 t3 = tex2D(NormalMapSampler, texCoord + float2(SampleOffset.x, SampleOffset.y));

    float2 f = frac(texCoord * TextureSize);
    float4 t = lerp(lerp(t0, t1, f.x), lerp(t2, t3, f.x), f.y);

    float3 n = normalize(cross(float3(0, t.y, 1), float3(1, t.x, 0)));
    return normalize(mul(n, World));
}

float ComputeFresnel(float3 E, float3 position)
{
    float3 surfaceNormal = float3(0, 1, 0);
    float angle = saturate(dot(E, surfaceNormal));
    float fresnel = R0 + (1.0f - R0) * pow(1.0f - angle, 5.0);

    //also based on distance
    float dist = distance(EyePosition, position);
    return min(1.0f, fresnel + DistanceAlphaFactor * dist);
}

//
// REFERENCE: I copied and modified BasicEffect shader
//
ColorPair ComputePerPixelLights(float3 E, float3 N, float3 R)
{
    ColorPair result;

    result.Diffuse = AmbientLightColor;
    result.Specular = 0;

    // Light0
    float3 L = -DirLight0Direction;
    float dt = max(0,dot(L,N));
    result.Diffuse += DirLight0DiffuseColor * dt;
    if (dt != 0)
    {
        result.Specular += DirLight0SpecularColor * pow(max(0.00001f,dot(R,L)), SpecularPower);
    }

    // Light1
    L = -DirLight1Direction;
    dt = max(0,dot(L,N));
    result.Diffuse += DirLight1DiffuseColor * dt;
    if (dt != 0)
    {
        result.Specular += DirLight1SpecularColor * pow(max(0.00001f,dot(R,L)), SpecularPower);
    }

    // Light2
    L = -DirLight2Direction;
    dt = max(0,dot(L,N));
    result.Diffuse += DirLight2DiffuseColor * dt;
    if (dt != 0)
    {
        result.Specular += DirLight2SpecularColor * pow(max(0.00001f,dot(R,L)), SpecularPower);
    }

    result.Diffuse *= DiffuseColor;
    result.Diffuse += EmissiveColor;
    result.Specular *= SpecularColor;

    return result;
}

float4 PS(VSOutput input) : COLOR
{
    float3 E = normalize(input.ToEye);
    float3 N = ComputeNormal(input.TexCoord);
    float3 R = reflect(-E, N);
    float fresnel = 0;

    if (input.WorldPosition.y <= EyePosition.y)
    {
        fresnel = ComputeFresnel(E, input.WorldPosition);
    }
    else
    {
        R.y = -R.y;
    }

    ColorPair lightResult = ComputePerPixelLights(E, N, R);

    float alpha = max(MinAlpha, fresnel);
    alpha = min(MaxAlpha, alpha);

    float4 diffuse = float4(lightResult.Diffuse * DiffuseColor, alpha);
    float4 color = diffuse + float4(lightResult.Specular, 0);
    color.rgb = lerp(color.rgb, FogColor, input.FogFactor);

    return color;
}

//-----------------------------------------------------------------------------
// Section:  Technique
//-----------------------------------------------------------------------------
technique Default
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}


texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = <Texture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VSInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VSOutput VS(VSInput input)
{
    VSOutput output;

    output.Position = input.Position;
    output.TexCoord = input.TexCoord;

    return output;
}

float4 PS(VSOutput input) : COLOR0
{
    float4 color = tex2D(TextureSampler, input.TexCoord);

    float4 result = 1.0f - color;
    result.a = color.a;

    return result;
}

technique Default
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
    }
}

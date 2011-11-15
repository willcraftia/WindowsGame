//-----------------------------------------
//
//-----------------------------------------

float4x4 World;
float4x4 LightViewProjection;

struct VS_OUTPUT
{
    float4 Position     : POSITION;
    float4 PositionWVP  : TEXCOORD0;
};

VS_OUTPUT VS(float4 position : POSITION)
{
    VS_OUTPUT output = (VS_OUTPUT) 0;

    output.Position = mul(mul(position, World), LightViewProjection);
    output.PositionWVP = output.Position;

    return output;
}

float4 PS(VS_OUTPUT input) : COLOR0
{
    float depth = input.PositionWVP.z / input.PositionWVP.w;
    return float4(depth, depth * depth, 0.0f, 0.0f);
}

technique Default
{
    pass P0
    {
          VertexShader = compile vs_2_0 VS();
          PixelShader = compile ps_2_0 PS();
    }
}

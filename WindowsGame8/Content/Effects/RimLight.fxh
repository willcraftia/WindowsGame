#if !defined (RIM_LIGHT_FXH)
#define RIM_LIGHT_FXH

float3 RimLighting(float3 lightColor, float3 normal, float3 cameraDirection, float lightPower)
{
    return pow(1 - saturate(dot(normal, cameraDirection)), lightPower) * lightColor;
}

#endif // !defined (RIM_LIGHT_FXH)

//-----------------------------------------------------------------------------
//
// Shadow Scene Common Definitions
//
//-----------------------------------------------------------------------------
#if !defined (TOON_FXH)
#define TOON_FXH


//bool ToonEnabled = false;
//float ToonThresholds[2] = { 0.8, 0.4 };
//float ToonThresholds[2] = { 0.4, 0.05 };
//float ToonBrightnessLevels[3] = { 1.0, 0.5, 0.0 };
//float ToonBrightnessLevels[3] = { 1.0, 0.6, 0.3 };

struct TOON
{
    bool Enabled;
    float Thresholds[2];
    float BrightnessLevels[3];
};

TOON Toon =
{
    true,
    { 0.4, 0.05 },
    { 1.0, 0.5, 0.0 }
};


//float ToonFilter(float shininess)
//{
//    float bias = 0.1;
//    float result = shininess;
//    result *= 3;
//    result = floor(result);
//    result /= 3;
//    result += bias;
//    return result;
//}

float ToonFilter(float shininess)
{
    if (Toon.Thresholds[0] < shininess)
    {
        return Toon.BrightnessLevels[0];
    }
    else if (Toon.Thresholds[1] < shininess)
    {
        return Toon.BrightnessLevels[1];
    }
    else
    {
        return Toon.BrightnessLevels[2];
    }
}

#endif // !defined (TOON_FXH)

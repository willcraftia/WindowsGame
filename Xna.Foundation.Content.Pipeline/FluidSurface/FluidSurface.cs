#region Using

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.FluidSurface
{
    public sealed class FluidSurface
    {
        public ExternalReference<EffectContent> Effect { get; set; }

        public Vector3 DiffuseColor { get; set; }
        public Vector3 EmissiveColor { get; set; }
        public Vector3 SpecularColor { get; set; }
        public float SpecularPower { get; set; }

        public float MinAlpha { get; set; }
        public float MaxAlpha { get; set; }
        public float DistanceAlphaFactor { get; set; }
        public float SpringPower { get; set; }

        public FluidSurface()
        {
            Effect = new ExternalReference<EffectContent>();
            DiffuseColor = Vector3.One;
            EmissiveColor = Vector3.Zero;
            SpecularColor = Vector3.One;
            SpecularPower = 1.0f;
            MinAlpha = 0.5f;
            MaxAlpha = 0.9f;
            DistanceAlphaFactor = 0.01f;
            SpringPower = 0.2f;
        }
    }
}

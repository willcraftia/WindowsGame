
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials
{
    public sealed class CubeMaterial
    {
        [ContentSerializer(Optional = true)]
        public ExternalReference<EffectContent> Effect { get; set; }

        [ContentSerializer(Optional = true)]
        public CubeTexture Diffuse { get; private set; }

        public float Alpha { get; set; }

        public Vector3 DiffuseColor { get; set; }
        public Vector3 EmissiveColor { get; set; }
        public Vector3 SpecularColor { get; set; }
        public float SpecularPower { get; set; }

        public bool VertexColorEnabled { get; set; }

        public CubeMaterial()
        {
            Diffuse = new CubeTexture();
            Alpha = 1;
            DiffuseColor = Vector3.One;
            EmissiveColor = Vector3.Zero;
            SpecularColor = Vector3.Zero;
            SpecularPower = 1;
        }
    }
}

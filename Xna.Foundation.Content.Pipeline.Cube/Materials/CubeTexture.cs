
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials
{
    public sealed class CubeTexture
    {
        [ContentSerializer(Optional = true)]
        public ExternalReference<TextureContent> Texture { get; set; }

        public float Scale { get; set; }
        public bool GenerateMipmaps { get; set; }
        public TextureProcessorOutputFormat TextureFormat { get; set; }

        public CubeTexture()
        {
            Scale = 8;
            GenerateMipmaps = true;
            TextureFormat = TextureProcessorOutputFormat.DxtCompressed;
        }
    }
}

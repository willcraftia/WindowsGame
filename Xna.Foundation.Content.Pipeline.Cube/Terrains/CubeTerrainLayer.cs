#region Using

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Terrains
{
    public sealed class CubeTerrainLayer
    {
        public ExternalReference<TextureContent> Alphamap;
        public ExternalReference<CubeMaterial> Material;
    }
}

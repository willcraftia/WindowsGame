#region Using

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Terrains
{
    [ContentSerializerRuntimeType("Willcraftia.Xna.Foundation.Cube.Scenes.CubeTerrain, Willcraftia.Xna.Foundation.Cube")]
    public sealed class CubeTerrainContent
    {
        public ModelContent Model;
        public CubeHeightmapContent Heightmap;
    }
}
